using System.Text.Json.Serialization;

namespace Dijitle.Metra.API.Models.Output
{
    public class Route
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }

        [JsonPropertyName("longName")]
        public string LongName { get; set; }

        [JsonPropertyName("routecolor")]
        public string RouteColor { get; set; }

        [JsonPropertyName("textColor")]
        public string TextColor { get; set; }
    }
}
