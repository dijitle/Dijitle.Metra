using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dijitle.Metra.API.Models.Output
{
    public class Shape
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("textcolor")]
        public string TextColor { get; set; }

        [JsonPropertyName("points")]
        public List<ShapePoint> Points { get; set; }
    }

    public class ShapePoint
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }
        [JsonPropertyName("lon")]
        public double Lon { get; set; }
        [JsonPropertyName("sequence")]
        public int Sequence { get; set; }

        public override string ToString()
        {
            return $"{Sequence} - {Lat},{Lon}";
        }
    }
}
