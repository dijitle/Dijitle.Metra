using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Dijitle.Metra.Data
{
    public class Routes
    {
        public string route_id {get; private set;}
        public string route_short_name {get; private set;}
        public string route_long_name {get; private set;}
        public string route_desc {get; private set;}
        public string agency_id {get; private set;}
        public Route_Type route_type {get; private set;}
        public string route_color {get; private set;}
        public string route_text_color {get; private set;}
        public string route_url {get; private set;}

        public Agency Agency { get; private set; }
        public List<Trips> Trips { get; private set; }
        public List<Stops> Stops { get; private set; }
        public Dictionary<string, List<Shapes>> Shapes { get; private set; }

        public enum Route_Type
        {
            Streetcar = 0,
            Subway = 1,
            Rail = 2,
            Bus = 3,
            Ferry = 4,
            CableTram = 5,
            AerialLift = 6,
            Funicular = 7
        }

        public Routes(Dictionary<string, string> dictData)
        {
            route_id = dictData["route_id"];
            route_short_name = dictData["route_short_name"];
            route_long_name = dictData["route_long_name"];
            route_desc = dictData["route_desc"];
            agency_id = dictData["agency_id"];
            route_type = (Route_Type)Convert.ToInt32(dictData["route_type"]);
            route_color = dictData["route_color"];
            route_text_color = dictData["route_text_color"];

            if(dictData.ContainsKey("route_url"))
            {
                route_url = dictData["route_url"];
            }

            Trips = new List<Trips>();
            Stops = new List<Stops>();
            Shapes = new Dictionary<string, List<Shapes>>();
        }

        public void Link(IDictionary<string, Agency> agencies,IDictionary<string, List<Shapes>> shapes)
        {
            Agency = agencies[agency_id];
            Agency.Routes.Add(this);

            foreach (var kvp in shapes)
            {
                if(kvp.Key.Split("_").FirstOrDefault() == route_id)
                {
                    Shapes.Add(kvp.Key, kvp.Value);
                }
                else if (kvp.Key.StartsWith("south_shore") && route_id == "so_shore")
                {
                    Shapes.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public override string ToString()
        {
            return route_short_name;
        }
    }
}
