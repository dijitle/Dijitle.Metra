using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dijitle.Metra.API.Models.Output
{
    public class Position
    {
        [JsonProperty("id")]
        public int Id { get; set; }
               
        [JsonProperty("tripId")]
        public string TripId { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }
    }
}
