using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dijitle.Metra.API.Models.Output
{
    public class Stop
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("distance")]
        public double DistanceAway { get; set; }

        [JsonPropertyName("arrivalTime")]
        public DateTime ArrivalTime { get; set; }

        [JsonPropertyName("departureTime")]
        public DateTime DepartureTime { get; set; }

        [JsonPropertyName("routes")]
        public IEnumerable<string> Routes { get; set; }
    }
}
