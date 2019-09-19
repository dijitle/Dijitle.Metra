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

        public Stops(Dictionary<string, string> dictData)
        {
            stop_id = dictData["stop_id"];
            stop_name = dictData["stop_name"];
            stop_desc = dictData["stop_desc"];
            stop_lat = Convert.ToDouble(dictData["stop_lat"]);
            stop_lon = Convert.ToDouble(dictData["stop_lon"]);
            zone_id = dictData["zone_id"];
            stop_url = dictData["stop_url"];
            wheelchair_boarding = (dictData["wheelchair_boarding"] == "1");

            Trips = new List<Trips>();
        }

        public override string ToString()
        {
            return stop_name;
        }
    }
}
