using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Dijitle.Metra.API.Models.Output
{
    public class Fare
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        [JsonPropertyName("destination")]
        public string Destination { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
