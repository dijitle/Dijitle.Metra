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

        public List<Routes> Routes { get; private set; }

        public Agency(Dictionary<string,string> dictData)
        {
            agency_id = dictData["agency_id"];
            agency_name = dictData["agency_name"];
            agency_url = dictData["agency_url"];
            agency_timezone = dictData["agency_timezone"];
            agency_lang = dictData["agency_lang"];
            agency_phone = dictData["agency_phone"];

            if(dictData.ContainsKey("agency_phone"))
            {
                agency_fare_url = dictData["agency_phone"];
            }

            Routes = new List<Routes>();
        }

        public override string ToString()
        {
            return agency_name;
        }
    }
}
