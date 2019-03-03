using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Dijitle.Metra.Data
{
    public class FareRules
    {
        public int fare_id { get; private set; }
        public string route_id { get; private set; }
        public string origin_id { get; private set; }
        public string destination_id { get; private set; }
        public string contains_id { get; private set; }

        public FareAttributes Fare { get; private set; }

        public FareRules(string[] csv)
        {
            fare_id = Convert.ToInt32(csv[0].Trim());
            route_id = csv[1].Trim();
            origin_id = csv[2].Trim();
            destination_id = csv[3].Trim();
            contains_id = csv[4].Trim();
        }

        public void LinkFare(IDictionary<int, FareAttributes> fareAttributes)
        {
            Fare = fareAttributes[fare_id];
        }

        public override string ToString()
        {
            return $"{origin_id} -> {destination_id}";
        }
    }
}
