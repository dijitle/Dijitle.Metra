using Newtonsoft.Json;
using System;

namespace Dijitle.Metra.API.Models.Output
{
    public class Stop
    {
        [JsonProperty("Id")]
        public string stop_id { get; set; }

        [JsonProperty("Name")]
        public string stop_name { get; set; }

        [JsonProperty("Latitude")]
        public decimal stop_lat { get; set; }

        [JsonProperty("Longitude")]
        public decimal stop_lon { get; set; }

        [JsonProperty("DistanceAway")]
        public decimal DistanceAway { get; set; }
    }
}
