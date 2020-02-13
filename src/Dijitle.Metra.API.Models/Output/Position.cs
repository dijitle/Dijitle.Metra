using Newtonsoft.Json;

namespace Dijitle.Metra.API.Models.Output
{
    public class Position
    {
        [JsonProperty("id")]
        public string Id { get; set; }
               
        [JsonProperty("tripId")]
        public string TripId { get; set; }

        [JsonProperty("direction")]
        public bool Direction { get; set; }
        
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }
    }
}
