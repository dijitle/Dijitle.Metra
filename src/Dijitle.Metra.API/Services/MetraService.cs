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

            foreach(Routes r in _gtfs.Data.Routes)
            {
                routes.Add(new Route()
                {
                    route_id = r.route_id,
                    route_long_name = r.route_long_name
                });
            }

            return routes;
        }

        public async Task<IEnumerable<Time>> GetTimes(string origin, string destination, bool expressOnly)
        {
            DateTime selectedDate = DateTime.Now;

            Stops originStop = _gtfs.Data.FindStop(origin);
            Stops destinationStop = _gtfs.Data.FindStop(destination);

            List<Time> times = new List<Time>();

            IEnumerable<Routes> routes = _gtfs.Data.Routes.Where(r => r.Stops.Contains(originStop) && r.Stops.Contains(destinationStop));

            IEnumerable<Calendar> currentCalendars = _gtfs.Data.GetCurrentCalendars(selectedDate);
            
            foreach(Routes r in routes)
            {
                foreach (Trips t in r.Trips.Where(t => currentCalendars.Contains(t.Calendar) && 
                                                           t.StopTimes.Any(st => st.Stop == originStop) && 
                                                           t.StopTimes.Any(st => st.Stop == destinationStop) && 
                                                           t.StopTimes.Single(st => st.Stop == originStop).stop_sequence < t.StopTimes.Single(st => st.Stop == destinationStop).stop_sequence).OrderBy(t => t.StopTimes.Single(st => st.Stop == originStop).departure_time))
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
                            trip_id = t.trip_id,
                            arrival_time = destinationStopTime.departure_time,
                            departure_time = originStopTime.arrival_time,
                            IsExpress = t.IsExpress(originStopTime, destinationStopTime),
                            StopsIn = indexOrigin,
                            StopsUntil = indexDestination - indexOrigin - 1
                        });
                    }
                }
            }

            return times;
        }

        public async Task<IEnumerable<Stop>> GetStops(decimal lat, decimal lon, int milesAway)
        {
            List<Stop> stops = new List<Stop>();

            foreach(Stops s in _gtfs.Data.Stops)
            {
                stops.Add(new Stop()
                {
                    stop_id = s.stop_id,
                    stop_name = s.stop_name,
                    stop_lat = s.stop_lat,
                    stop_lon = s.stop_lon,
                    DistanceAway = GetDistance(lat, lon, s.stop_lat, s.stop_lon)
                });
            }

            return stops.Where(s => s.DistanceAway < milesAway).OrderBy(s => s.DistanceAway);
        }

        private decimal GetDistance(decimal startLat, decimal startLon, decimal destLat, decimal destLon)
        {
            const int EARTH_RADIUS = 3959;

            double startLatRadians = GetRadians((double)startLat);
            double destLatRadians = GetRadians((double)destLat);
            double deltaLatRadians = GetRadians((double)(destLat - startLat));
            double detlaLonRadians = GetRadians((double)(destLon - startLon));

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
