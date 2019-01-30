using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Dijitle.Metra.Data
{
    public class Agency
    {
        public string agency_id { get; private set; }
        public string agency_name { get; private set; }
        public string agency_url { get; private set; }
        public string agency_timezone { get; private set; }
        public string agency_lang { get; private set; }
        public string agency_phone { get; private set; }
        public string agency_fare_url { get; private set; }

        public Agency(string[] csv)
        {
            agency_id = csv[0].Trim();
            agency_name = csv[1].Trim();
            agency_url = csv[2].Trim();
            agency_timezone = csv[3].Trim();
            agency_lang = csv[4].Trim();
            agency_phone = csv[5].Trim();
            agency_fare_url = csv[6].Trim();
        }

        public override string ToString()
        {
            return agency_name;
        }
    }
}
