using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Dijitle.Metra.Data
{
    public class Routes
    {
        public string route_id {get; private set;}
        public string route_short_name {get; private set;}
        public string route_long_name {get; private set;}
        public string route_desc {get; private set;}
        public string agency_id {get; private set;}
        public string route_type {get; private set;}
        public string route_color {get; private set;}
        public string route_text_color {get; private set;}
        public string route_url {get; private set;}

        public Agency Agency { get; private set; }
        public List<Trips> Trips { get; private set; }
        public List<Stops> Stops { get; private set; }

        public Routes(string[] csv)
        {
            route_id = csv[0].Trim();
            route_short_name = csv[1].Trim();
            route_long_name = csv[2].Trim();
            route_desc = csv[3].Trim();
            agency_id = csv[4].Trim();
            route_type = csv[5].Trim();
            route_color = csv[6].Trim();
            route_text_color = csv[7].Trim();
            route_url = csv[8].Trim();

            Trips = new List<Trips>();
            Stops = new List<Stops>();
        }

        public void LinkAgency(IEnumerable<Agency> agencies)
        {
            foreach (Agency a in agencies)
            {
                if (a.agency_id == agency_id)
                {
                    Agency = a;
                    a.Routes.Add(this);
                    break;
                }
            }
        }

        public override string ToString()
        {
            return route_short_name;
        }
    }
}
