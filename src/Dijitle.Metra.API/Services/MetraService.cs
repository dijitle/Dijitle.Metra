using Dijitle.Metra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dijitle.Metra.API.Models.Output;

namespace Dijitle.Metra.API.Services
{
    public class MetraService : IMetraService
    {
        private readonly IGTFSService _gtfs;

        public MetraService(IGTFSService gtfs)
        {
            _gtfs = gtfs;
        }

        public async Task<IEnumerable<Route>> GetRoutes()
        {
            List<Route> routes = new List<Route>();

            if (_gtfs.Data == null)
            {
                await _gtfs.RefreshData();
            }

            foreach (Routes r in _gtfs.Data.Routes.Values)
            {
                routes.Add(new Route()
                {
                    Id = r.route_id,
                    ShortName = r.route_short_name,
                    LongName = r.route_long_name
                });
            }

            return routes;
        }

        public async Task<IEnumerable<Time>> GetTimes(Stops originStop, Stops destinationStop, bool expressOnly)
        {
            DateTime selectedDate = DateTime.Now;
            
            List<Time> times = new List<Time>();

            if (_gtfs.Data == null)
            {
                await _gtfs.RefreshData();
            }

            IEnumerable<Routes> routes = _gtfs.Data.Routes.Values.Where(r => r.Stops.Contains(originStop) && r.Stops.Contains(destinationStop));

            IEnumerable<Calendar> currentCalendars = _gtfs.Data.GetCurrentCalendars(selectedDate);
            
            foreach(Routes r in routes)
            {
                IEnumerable<Trips> ts = r.Trips.Where(t => currentCalendars.Contains(t.Calendar));
                ts = ts.Where(t => t.StopTimes.Any(st => st.Stop == originStop));
                ts = ts.Where(t => t.StopTimes.Any(st => st.Stop == destinationStop));
                ts = ts.Where(t => t.StopTimes.Single(st => st.Stop == originStop).stop_sequence < t.StopTimes.Single(st => st.Stop == destinationStop).stop_sequence);
                ts = ts.OrderBy(t => t.StopTimes.Single(st => st.Stop == originStop).departure_time);

                foreach (Trips t in ts)
                {
                    Stops firstStop = t.StopTimes.OrderBy(st => st.stop_sequence).FirstOrDefault().Stop;
                    Stops lastStop = t.StopTimes.OrderBy(st => st.stop_sequence).LastOrDefault().Stop;

                    StopTimes originStopTime = t.StopTimes.Single(st => st.Stop == originStop);
                    StopTimes destinationStopTime = t.StopTimes.Single(st => st.Stop == destinationStop);

                    int indexOrigin = t.StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(originStopTime);
                    int indexDestination = t.StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(destinationStopTime);

                    if(t.IsExpress(originStopTime, destinationStopTime) || !expressOnly)
                    {
                        times.Add(new Time
                        {
                            Id = t.trip_id,
                            ArrivalTime = destinationStopTime.departure_time,
                            DepartureTime = originStopTime.arrival_time,
                            DestinationStop = new Stop()
                            {
                                Id = destinationStopTime.Stop.stop_id,
                                Name = destinationStopTime.Stop.stop_name,
                                Lat = destinationStopTime.Stop.stop_lat,
                                Lon = destinationStopTime.Stop.stop_lon

                            },
                            OriginStop = new Stop()
                            {
                                Id = originStopTime.Stop.stop_id,
                                Name = originStopTime.Stop.stop_name,
                                Lat = originStopTime.Stop.stop_lat,
                                Lon = originStopTime.Stop.stop_lon
                            },
                            IsExpress = t.IsExpress(originStopTime, destinationStopTime),
                            StopsIn = indexOrigin,
                            StopsUntil = indexDestination - indexOrigin - 1
                        });
                    }
                }
            }

            return times;
        }

        public async Task<IEnumerable<Stop>> GetStopsByDistance(decimal lat, decimal lon, int milesAway)
        {
            List<Stop> stops = new List<Stop>();

            if (_gtfs.Data == null)
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
            List<Stop> stops = new List<Stop>();

            if (_gtfs.Data == null)
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
                    DistanceAway = 0
                });
            }

            return stops.OrderBy(s => s.Name);
        }

        public async Task<IEnumerable<Stop>> GetStopsByRoute(string route, bool sortAsc)
        {
            List<Stop> stops = new List<Stop>();

            if (_gtfs.Data == null)
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
                    DistanceAway = GetDistance(41.882077m, -87.627807m, s.stop_lat, s.stop_lon)
                });
            }

            if (sortAsc)
            {
                return stops.OrderBy(s => s.DistanceAway);
            }
            return stops.OrderByDescending(s => s.DistanceAway);
        }

        public async Task<IEnumerable<Shape>> GetShapes(Routes route)
        {
            List<Shape> shapes = new List<Shape>();

            foreach (var skvp in route.Shapes)
            {
                Shape s = new Shape()
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

        public decimal GetDistance(decimal startLat, decimal startLon, decimal endLat, decimal endLon)
        {
            const int EARTH_RADIUS = 3959;

            double startLatRadians = GetRadians((double)startLat);
            double destLatRadians = GetRadians((double)endLat);
            double deltaLatRadians = GetRadians((double)(endLat - startLat));
            double detlaLonRadians = GetRadians((double)(endLon - startLon));

            double a = Math.Sin(deltaLatRadians / 2) * 
                       Math.Sin(deltaLatRadians / 2) + 
                       Math.Cos(startLatRadians) * 
                       Math.Cos(destLatRadians) * 
                       Math.Sin(detlaLonRadians / 2) * 
                       Math.Sin(detlaLonRadians / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (decimal)(EARTH_RADIUS * c);
        }

        private double GetRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
