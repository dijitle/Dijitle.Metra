using System;
using System.Collections.Generic;
using System.Text;

namespace Dijitle.Metra.Data
{
    public class Stops
    {
        public string stop_id {  get; private set; }
        public string stop_name { get; private set; }
        public string stop_desc { get; private set; }
        public double stop_lat { get; private set; }
        public double stop_lon { get; private set; }
        public string zone_id { get; private set; }
        public string stop_url { get; private set; }
        public bool wheelchair_boarding { get; private set; }

        public List<Trips> Trips { get; private set; }

        public Stops(string[] csv)
        {
            stop_id = csv[0].Trim();
            stop_name = csv[1].Trim();
            stop_desc = csv[2].Trim();
            stop_lat = Convert.ToDouble(csv[3].Trim());
            stop_lon = Convert.ToDouble(csv[4].Trim());
            zone_id = csv[5].Trim();
            stop_url = csv[6].Trim();
            wheelchair_boarding = (csv[7].Trim() == "1");

            Trips = new List<Trips>();
        }

        public override string ToString()
        {
            return stop_name;
        }
    }
}
