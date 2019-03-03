using Newtonsoft.Json;
using System;

namespace Dijitle.Metra.API.Models.Output
{
    public class Stop
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lat")]
        public decimal Lat { get; set; }

        [JsonProperty("lon")]
        public decimal Lon { get; set; }

        [JsonProperty("distance")]
        public decimal DistanceAway { get; set; }
    }
}
