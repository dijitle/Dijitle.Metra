using Newtonsoft.Json;

namespace Dijitle.Metra.API.Models.Output
{
    public class Route
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("longName")]
        public string LongName { get; set; }

        [JsonProperty("routecolor")]
        public string RouteColor { get; set; }

        [JsonProperty("textColor")]
        public string TextColor { get; set; }
    }
}
