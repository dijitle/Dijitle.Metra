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

        public enum Direction
        {
            Outbound = 0,
            Inbound = 1
        }

        public Trips(string[] csv)
        {
            route_id = csv[0].Trim();
            service_id = csv[1].Trim();
            trip_id = csv[2].Trim();
            trip_headsign = csv[3].Trim();
            block_id = csv[4].Trim();
            shape_id = csv[5].Trim();
            direction_id = (Direction)Convert.ToInt32(csv[6].Trim());

            StopTimes = new List<StopTimes>();
        }

        public override string ToString()
        {
            return trip_id;
        }

        public void LinkRouteAndService(IDictionary<string, Routes> routes, IDictionary<string, Calendar> calendars)
        {
            Routes r = routes[route_id];
            Route = r;
            r.Trips.Add(this);

            Calendar c = calendars[service_id];
            Calendar = c;
        }

        public bool IsExpress(StopTimes origin, StopTimes destination)
        {
            int indexOrigin = StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(origin);
            int indexDestination = StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(destination);

            

            return indexDestination - indexOrigin < 4;
        }
    }
}
