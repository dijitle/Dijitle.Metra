using System.Text.Json.Serialization;

namespace Dijitle.Metra.API.Models.Output
{
    public class Position
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
               
        [JsonPropertyName("tripId")]
        public string TripId { get; set; }

        [JsonPropertyName("direction")]
        public bool Direction { get; set; }
        
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
