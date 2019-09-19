using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dijitle.Metra.Data
{
    public class Trips
    {
        public string route_id { get; private set; }
        public string service_id { get; private set; }
        public string trip_id { get; private set; }
        public string trip_headsign { get; private set; }
        public string block_id { get; private set; }
        public string shape_id { get; private set; }
        public Direction direction_id { get; private set; }

        public Routes Route { get; private set; }
        public Calendar Calendar { get; private set; }
        public List<StopTimes> StopTimes { get; private set; }

        public List<Shapes> Shapes { get; private set; }

        public enum Direction
        {
            Outbound = 0,
            Inbound = 1
        }

        public Trips(Dictionary<string, string> dictData)
        {
            route_id = dictData["route_id"];
            service_id = dictData["service_id"];
            trip_id = dictData["trip_id"];
            trip_headsign = dictData["trip_headsign"];
            block_id = dictData["block_id"];
            shape_id = dictData["shape_id"];
            direction_id = (Direction)Convert.ToInt32(dictData["direction_id"]);
        }

        public override string ToString()
        {
            return trip_id;
        }

        public void Link(IDictionary<string, Routes> routes, IDictionary<string, Calendar> calendars, IDictionary<string, List<StopTimes>> stoptimes, IDictionary<string, Stops> stops, IDictionary<string, List<Shapes>> shapes)
        {
            Route = routes[route_id];
            Route.Trips.Add(this);

            Calendar = calendars[service_id];

            StopTimes = stoptimes[trip_id];

            Shapes = shapes[shape_id];
            
            foreach(StopTimes st in StopTimes)
            {
                st.Trip = this;
                Stops s = stops[st.stop_id];
                st.Stop = s;
                if (!Route.Stops.Contains(s))
                {
                    Route.Stops.Add(s);
                }
            }
        }

        public bool IsExpress(StopTimes origin, StopTimes destination)
        {
            int indexOrigin = StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(origin);
            int indexDestination = StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(destination);
            return indexDestination - indexOrigin < StopTimes.OrderBy(st => st.stop_sequence).LastOrDefault().stop_sequence / 2;
        }
    }
}
