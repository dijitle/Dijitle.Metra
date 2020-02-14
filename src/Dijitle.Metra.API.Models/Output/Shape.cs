using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dijitle.Metra.API.Models.Output
{
    public class Shape
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("textcolor")]
        public string TextColor { get; set; }

        [JsonProperty("points")]
        public List<ShapePoint> Points { get; set; }
    }

    public class ShapePoint
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("lon")]
        public double Lon { get; set; }
        [JsonProperty("sequence")]
        public int Sequence { get; set; }
    }
}
