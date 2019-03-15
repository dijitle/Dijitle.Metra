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
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("distance")]
        public double DistanceAway { get; set; }

        [JsonProperty("arrivalTime")]
        public DateTime ArrivalTime { get; set; }

        [JsonProperty("departureTime")]
        public DateTime DepartureTime { get; set; }
    }
}
