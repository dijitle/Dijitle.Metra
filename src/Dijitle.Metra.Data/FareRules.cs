using System.Collections.Generic;

namespace Dijitle.Metra.Data
{
    public class FareRules
    {
        public string fare_id { get; private set; }
        public string route_id { get; private set; }
        public string origin_id { get; private set; }
        public string destination_id { get; private set; }
        public string contains_id { get; private set; }

        public FareAttributes Fare { get; private set; }

        public FareRules(Dictionary<string, string> dictData)
        {
            fare_id = dictData["fare_id"];
            route_id = dictData["route_id"];
            origin_id = dictData["origin_id"];
            destination_id = dictData["destination_id"];
            contains_id = dictData["contains_id"];
        }

        public void Link(IDictionary<string, FareAttributes> fareAttributes)
        {
            Fare = fareAttributes[fare_id];
        }

        public override string ToString()
        {
            return $"{origin_id} -> {destination_id}";
        }
    }
}
