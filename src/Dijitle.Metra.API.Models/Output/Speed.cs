using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dijitle.Metra.API.Models.Output
{
    public class Speed
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }
        [JsonPropertyName("lon")]
        public double Lon { get; set; }
        [JsonPropertyName("sequence")]
        public int Sequence { get; set; }
        [JsonPropertyName("speed")]
        public double AvgSpeed { get; set; }
    }
}
