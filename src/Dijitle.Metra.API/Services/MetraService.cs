﻿using Dijitle.Metra.Data;
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
        private AllData _data;

        public MetraService()
        {
            _data = new AllData(@"X:\SourceCode\Dijitle.Metra\data");
        }

        public IEnumerable<Route> GetRoutes()
        {
            List<Route> routes = new List<Route>();

            foreach(Routes r in _data._routes)
            {
                routes.Add(new Route()
                {
                    route_id = r.route_id,
                    route_long_name = r.route_long_name
                });
            }

            return routes;
        }

        public IEnumerable<Time> GetTimes(string origin, string destination, bool expressOnly)
        {
            DateTime selectedDate = DateTime.Now;

            Stops originStop = _data.FindStop(origin);
            Stops destinationStop = _data.FindStop(destination);

            List<Time> times = new List<Time>();

            IEnumerable<Routes> routes = _data._routes.Where(r => r.Stops.Contains(originStop) && r.Stops.Contains(destinationStop));

            IEnumerable<Calendar> currentCalendars = _data._calendars.Where(c => c.start_date < selectedDate && c.end_date.AddDays(1) >= selectedDate && c.IsDay(selectedDate.DayOfWeek) == true);

            
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
    }
}
