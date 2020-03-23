using Dijitle.Metra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dijitle.Metra.API.Models.Output;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Prometheus;

namespace Dijitle.Metra.API.Services
{
    public class MetraService : IMetraService
    {
        private readonly IGTFSService _gtfs;
        private static readonly Gauge TripsEnrouteGauge = Metrics.CreateGauge("metra_trips_enroute_total", 
            "Number of trips that are currently enroute based on schedule.", 
            new GaugeConfiguration 
            { 
                LabelNames = new[] { "direction", "route" }, 
                SuppressInitialValue = true 
            });

        private static readonly Histogram GPSVarianceHistogram = Metrics.CreateHistogram("metra_gps_variance_miles",
            "Differnce between GPS and Estimated location.", 
            new HistogramConfiguration 
            {
                LabelNames = new[] { "direction", "route" }, 
                Buckets = Histogram.LinearBuckets(1, 1, 9), 
                SuppressInitialValue = true 
            });

        public MetraService(IGTFSService gtfs)
        {
            _gtfs = gtfs;
        }

        public async Task<IEnumerable<Route>> GetRoutes()
        {
            List<Route> routes = new List<Route>();

            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            foreach (Routes r in _gtfs.Data.Routes.Values)
            {
                routes.Add(new Route()
                {
                    Id = r.route_id,
                    ShortName = r.route_short_name,
                    LongName = r.route_long_name,
                    RouteColor = r.route_color,
                    TextColor = r.route_text_color
                });
            }

            return routes;
        }

        public async Task<Trip> GetTrip(string id)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            var t = _gtfs.Data.Trips[id];

            if (t == null)
            {
                return null;
            }

            var route = _gtfs.Data.Routes[t.route_id];

            IEnumerable<Stop> routeStops = await GetStopsByRoute(route.route_id, t.direction_id == Trips.Direction.Outbound);

            var trip = new Trip()
            {
                Id = t.trip_id,
                IsExpress = route.Stops.Count >= t.StopTimes.Count * 2,
                Inbound = t.direction_id == Trips.Direction.Inbound,
                Route = new Route()
                {
                    Id = route.route_id,
                    
                    ShortName = route.route_short_name,
                    LongName = route.route_long_name,
                    RouteColor = route.route_color,
                    TextColor = route.route_text_color
                },
                ShapeId = t.shape_id
            };

            foreach (Stop st in routeStops)
            {
                trip.RouteStops.Add(st);
            }

            trip.OriginStop = trip.RouteStops.FirstOrDefault();
            trip.DestinationStop = trip.RouteStops.LastOrDefault();

            DateTime day = CurrentTime;

            foreach (StopTimes st in t.StopTimes.OrderBy(st => st.stop_sequence))
            {
                foreach (Stop s in trip.RouteStops)
                {
                    if (s.Id == st.stop_id)
                    {
                        s.ArrivalTime = GetTime(day, st.arrival_time);
                        s.DepartureTime = GetTime(day, st.departure_time);
                        trip.TripStops.Add(s);
                        break;
                    }
                }
            }

            return trip;
        }

        public async Task<IEnumerable<string>> GetTripsEnroute()
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            DateTime selectedDate = CurrentTime;
            
            var day = _gtfs.Data.GetCurrentCalendars(selectedDate);
            
            var tripsEnroute = _gtfs.Data.Trips.Values
                .Where(t => day.Contains(t.Calendar))
                .Where(t => GetTime(selectedDate, t.StopTimes.OrderBy(st => st.stop_sequence).First().departure_time) < selectedDate)
                .Where(t => GetTime(selectedDate, t.StopTimes.OrderBy(st => st.stop_sequence).Last().arrival_time) > selectedDate);

            foreach (var r in _gtfs.Data.Routes)
            {
                TripsEnrouteGauge.WithLabels("in", r.Key).Set(0);
                TripsEnrouteGauge.WithLabels("out", r.Key).Set(0);
            }

            foreach (var t in tripsEnroute)
            {
                TripsEnrouteGauge.WithLabels(t.direction_id == Trips.Direction.Inbound ? "in" : "out", t.route_id).Inc();
            }
            return tripsEnroute.Select(t => t.trip_id);
        }

        public async Task<IEnumerable<Trip>> GetTrips(Stops originStop, Stops destinationStop, DateTime selectedDate)
        {
            var trips = new List<Trip>();

            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            Routes route = _gtfs.Data.Routes.Values.Where(r => r.Stops.Contains(originStop) && r.Stops.Contains(destinationStop)).FirstOrDefault();

            if(route == null)
            {
                return trips;
            }

            var cal = _gtfs.Data.GetCurrentCalendars(selectedDate);

            IEnumerable<Trips> ts = route.Trips
                .Where(t => cal.Contains(t.Calendar))
                .Where(t => t.StopTimes.Any(st => st.Stop == originStop))
                .Where(t => t.StopTimes.Any(st => st.Stop == destinationStop))
                .Where(t => t.StopTimes.Single(st => st.Stop == originStop).stop_sequence < t.StopTimes.Single(st => st.Stop == destinationStop).stop_sequence)
                .OrderBy(t => t.StopTimes.Single(st => st.Stop == originStop).departure_time);

            foreach (Trips t in ts)
            {
                StopTimes originStopTime = t.StopTimes.Single(st => st.Stop == originStop);
                StopTimes destinationStopTime = t.StopTimes.Single(st => st.Stop == destinationStop);
                    
                IEnumerable<Stop> routeStops = await GetStopsByRoute(route.route_id, t.direction_id == Trips.Direction.Outbound);

                Trip trip = new Trip()
                {
                    Id = t.trip_id,
                    IsExpress = t.IsExpress(originStopTime, destinationStopTime),
                    Inbound = t.direction_id == Trips.Direction.Inbound,
                    Route = new Route()
                    {
                        Id = route.route_id,
                        ShortName = route.route_short_name,
                        LongName = route.route_long_name,
                        RouteColor = route.route_color,
                        TextColor = route.route_text_color
                    },
                    ShapeId = t.shape_id
                };

                foreach(Stop st in routeStops)
                {
                    trip.RouteStops.Add(st);

                    if(st.Id == destinationStopTime.stop_id)
                    {
                        trip.DestinationStop = st;
                    }
                    else if(st.Id == originStopTime.stop_id)
                    {
                        trip.OriginStop = st;
                    }
                }

                foreach(StopTimes st in t.StopTimes.OrderBy(st => st.stop_sequence))
                {
                    foreach(Stop s in trip.RouteStops)
                    {
                        if(s.Id == st.stop_id)
                        {
                            s.ArrivalTime = GetTime(selectedDate, st.arrival_time);
                            s.DepartureTime = GetTime(selectedDate, st.departure_time);
                            trip.TripStops.Add(s);
                            break;
                        }
                    }
                }
                trips.Add(trip);
            }

            return trips;
        }

        public async Task<IEnumerable<Stop>> GetStopsByDistance(double lat, double lon, int milesAway)
        {
            var stops = new List<Stop>();

            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            foreach (Stops s in _gtfs.Data.Stops.Values)
            {
                stops.Add(new Stop()
                {
                    Id = s.stop_id,
                    Name = s.stop_name,
                    Lat = s.stop_lat,
                    Lon = s.stop_lon,
                    DistanceAway = GetDistance(lat, lon, s.stop_lat, s.stop_lon)
                });
            }

            return stops.Where(s => s.DistanceAway < milesAway).OrderBy(s => s.DistanceAway);
        }

        public async Task<IEnumerable<Stop>> GetAllStops()
        {
            var stops = new List<Stop>();

            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            foreach (Stops s in _gtfs.Data.Stops.Values)
            {
                stops.Add(new Stop()
                {
                    Id = s.stop_id,
                    Name = s.stop_name,
                    Lat = s.stop_lat,
                    Lon = s.stop_lon,
                    DistanceAway = GetDistance(41.882077d, -87.627807d, s.stop_lat, s.stop_lon),
                    Routes = s.Routes.Select(r => r.route_id)
                });
            }

            return stops.OrderBy(s => s.Name);
        }

        public async Task<IEnumerable<Stop>> GetStopsByRoute(string route, bool sortAsc)
        {
            var stops = new List<Stop>();

            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            foreach (Stops s in _gtfs.Data.Routes.SingleOrDefault(r => r.Key == route).Value.Stops)
            {
                stops.Add(new Stop()
                {
                    Id = s.stop_id,
                    Name = s.stop_name,
                    Lat = s.stop_lat,
                    Lon = s.stop_lon,
                    DistanceAway = GetDistance(41.882077d, -87.627807d, s.stop_lat, s.stop_lon)
                });
            }

            if (sortAsc)
            {
                stops = stops.OrderBy(s => s.DistanceAway).ToList();
            }
            else
            {
                stops = stops.OrderByDescending(s => s.DistanceAway).ToList();
            }

            foreach(Stop s in stops)
            {
                s.DistanceAway = GetDistance(stops.First().Lat, stops.First().Lon, s.Lat, s.Lon);
            }

            return stops;
        }

        public async Task<IEnumerable<Shape>> GetShapes(Routes route)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            var shapes = new List<Shape>();

            foreach (var skvp in route.Shapes)
            {
                var s = new Shape()
                {
                    Id = skvp.Key,
                    Color = route.route_color,
                    TextColor = route.route_text_color,
                    Points = new List<ShapePoint>()
                };

                foreach (Shapes shape in skvp.Value)
                {
                    s.Points.Add(new ShapePoint()
                    {
                        Lat = shape.shape_pt_lat,
                        Lon = shape.shape_pt_lon,
                        Sequence = shape.shape_pt_sequence
                    });
                }

                shapes.Add(s);
            }

            return shapes;
        }

        public async Task<Shape> GetShapes(string id)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            Routes r = _gtfs.Data.Routes[id.StartsWith("south_shore") ? "so_shore" : id.Substring(0, id.IndexOf('_'))];

            var shapes = _gtfs.Data.Shapes[id];
                        
            Shape s = new Shape()
            {
                Id = id,
                Color = r.route_color,
                TextColor = r.route_text_color,
                Points = new List<ShapePoint>()
            };

            foreach (Shapes shape in shapes)
            {
                s.Points.Add(new ShapePoint()
                {
                    Lat = shape.shape_pt_lat,
                    Lon = shape.shape_pt_lon,
                    Sequence = shape.shape_pt_sequence
                });
            }
            
            return s;
        }

        public async Task<IEnumerable<Models.Output.Calendar>> GetCalendars()
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            var calendars = new List<Models.Output.Calendar>();

            foreach (var ckvp in _gtfs.Data.Calendars)
            {
                var c = new Models.Output.Calendar()
                {
                    Id = ckvp.Key,
                    Monday = ckvp.Value.monday,
                    Tuesday = ckvp.Value.tuesday,
                    Wednesday = ckvp.Value.wednesday,
                    Thursday = ckvp.Value.thursday,
                    Friday = ckvp.Value.friday,
                    Saturday = ckvp.Value.saturday,
                    Sunday = ckvp.Value.sunday,
                    StartDate = ckvp.Value.start_date,
                    EndDate = ckvp.Value.end_date,
                    CalendarDates = new List<Models.Output.CalendarDate>()
                };

                foreach (var cdate in ckvp.Value.CalendarDates)
                {
                    c.CalendarDates.Add(new Models.Output.CalendarDate()
                    {
                        Date = cdate.date,
                        ExceptionType = cdate.exception_type.ToString()
                    });
                }

                calendars.Add(c);
            }

            return calendars;
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const int EARTH_RADIUS = 3959;

            double startLatRadians = GetRadians(lat1);
            double destLatRadians = GetRadians(lat2);
            double deltaLatRadians = GetRadians(lat2 - lat1);
            double detlaLonRadians = GetRadians(lon2 - lon1);

            double a = Math.Sin(deltaLatRadians / 2) * 
                       Math.Sin(deltaLatRadians / 2) + 
                       Math.Cos(startLatRadians) * 
                       Math.Cos(destLatRadians) * 
                       Math.Sin(detlaLonRadians / 2) * 
                       Math.Sin(detlaLonRadians / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (EARTH_RADIUS * c);
        }

        private double GetRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private DateTime GetTime(DateTime date, string time)
        {
            MatchCollection matches = Regex.Matches(time, @"^(?<hour>\d{1,2}):(?<minute>\d{2}):(?<second>\d{2})$");
            
            int hour = Convert.ToInt32(matches[0].Groups["hour"].Value);
            int minute = Convert.ToInt32(matches[0].Groups["minute"].Value);
            int second = Convert.ToInt32(matches[0].Groups["second"].Value);

            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc).AddHours(hour).AddMinutes(minute).AddSeconds(second);
        }

        public DateTime CurrentTime
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }

                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            }
        }

        public async Task<IEnumerable<Position>> GetAllEstimatedPositions(bool withRealTime = false)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            var returnPos = new List<Position>();

            var trips = await GetTripsEnroute();

            foreach(var t in trips)
            {
                var p = await GetEstimatedPosition(t);
                if(p != null)
                {
                    returnPos.Add(p);                    
                }
            }

            if (withRealTime)
            {
                foreach(var rtp in await _gtfs.GetPositions())
                {
                    var pos = returnPos.FirstOrDefault(p => p.TripId == rtp.TripId);
                    if(pos == null)
                    {
                        returnPos.Add(rtp);
                    }
                    else
                    {
                        pos.RealTimeCoordinates = rtp.RealTimeCoordinates;

                        GPSVarianceHistogram.WithLabels(pos.Direction ? "in" : "out", _gtfs.Data.Trips[pos.TripId].route_id)
                            .Observe(GetDistance(pos.RealTimeCoordinates.Latitude, pos.RealTimeCoordinates.Longitude,
                                                 pos.EstimatedCoordinates.Latitude, pos.EstimatedCoordinates.Longitude));
                    }
                }
            }

            return returnPos;
        }

        public async Task<Position> GetEstimatedPosition(string tripId)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            if(!_gtfs.Data.Trips.ContainsKey(tripId))
            {
                return null;
            }

            var t = _gtfs.Data.Trips[tripId];

            if (t == null)
            {
                return null;
            }

            double lat = 0d;
            double lon = 0d;
            var currentTime = CurrentTime;
            var currentStop = t.StopTimes.FirstOrDefault(st => GetTime(currentTime, st.arrival_time) < currentTime.AddSeconds(30) && GetTime(currentTime, st.departure_time) > currentTime.AddSeconds(-30));
            if (currentStop != null)
            {
                lat = Convert.ToDouble(currentStop.Stop.stop_lat);
                lon = Convert.ToDouble(currentStop.Stop.stop_lon);
            }
            else
            {
                var previousStop = t.StopTimes.Where(st => GetTime(currentTime, st.departure_time) < currentTime.AddSeconds(-30)).OrderBy(st => st.stop_sequence).LastOrDefault();
                var nextStop = t.StopTimes.Where(st => GetTime(currentTime, st.arrival_time) > currentTime.AddSeconds(30)).OrderBy(st => st.stop_sequence).FirstOrDefault();

                double diffFromPrevious = currentTime.Ticks -  GetTime(currentTime, previousStop.departure_time).Ticks;
                double diffToNext = GetTime(currentTime, nextStop.arrival_time).Ticks - currentTime.Ticks;

                var percentTravelled = diffFromPrevious / (diffFromPrevious + diffToNext);

                int startIndex = 0;
                int endIndex = 0;

                for (int i = 0; i < t.Shapes.Count; i++)
                {
                    var d1 = GetDistance(t.Shapes[i].shape_pt_lat, t.Shapes[i].shape_pt_lon, nextStop.Stop.stop_lat, nextStop.Stop.stop_lon);
                    var d2 = GetDistance(previousStop.Stop.stop_lat, previousStop.Stop.stop_lon, nextStop.Stop.stop_lat, nextStop.Stop.stop_lon);

                    if(d1 < d2)
                    {
                        startIndex = i;
                        break;
                    }
                }

                for (int i = startIndex; i < t.Shapes.Count; i++)
                {
                    var d1 = GetDistance(t.Shapes[i].shape_pt_lat, t.Shapes[i].shape_pt_lon, previousStop.Stop.stop_lat, previousStop.Stop.stop_lon);
                    var d2 = GetDistance(nextStop.Stop.stop_lat, nextStop.Stop.stop_lon, previousStop.Stop.stop_lat, previousStop.Stop.stop_lon);

                    if (d1 > d2)
                    {
                        endIndex = i - 1;
                        break;
                    }
                }

                if (endIndex == 0)
                {
                    endIndex = t.Shapes.Count - 1;
                }

                double totalDistanceBetweenStops = 0d;

                for (int i = startIndex; i < endIndex; i++)
                {
                    totalDistanceBetweenStops += GetDistance(t.Shapes[i].shape_pt_lat, t.Shapes[i].shape_pt_lon, t.Shapes[i + 1].shape_pt_lat, t.Shapes[i + 1].shape_pt_lon);
                }

                double runningTotal = 0d;

                for (int i = startIndex; i < endIndex; i++)
                {
                    var d = GetDistance(t.Shapes[i].shape_pt_lat, t.Shapes[i].shape_pt_lon, t.Shapes[i + 1].shape_pt_lat, t.Shapes[i + 1].shape_pt_lon);

                    if ((runningTotal + d) / totalDistanceBetweenStops < percentTravelled)
                    {
                        runningTotal += d;
                    }
                    else
                    {
                        var percentOfDToGetToPercentTravelled = ((percentTravelled * totalDistanceBetweenStops) - runningTotal) / d;

                        lat = t.Shapes[i].shape_pt_lat * (1 - percentOfDToGetToPercentTravelled) + t.Shapes[i + 1].shape_pt_lat * percentOfDToGetToPercentTravelled;
                        lon = t.Shapes[i].shape_pt_lon * (1 - percentOfDToGetToPercentTravelled) + t.Shapes[i + 1].shape_pt_lon * percentOfDToGetToPercentTravelled;
                        break;
                    }
                }

            }


            var p = new Position()
            {
                Id = t.trip_id ,
                TripId = t.trip_id,
                Direction = t.direction_id == Trips.Direction.Inbound,
                Label = t.trip_id.Contains("_") ? Regex.Match(t.trip_id.Split('_')[1], @"\d+$").Value : t.trip_id,
                EstimatedCoordinates = new PositionCoordinates
                {
                    Latitude = lat,
                    Longitude = lon
                }
            };

            return p;
        }
    }
}
