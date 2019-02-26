using Newtonsoft.Json;

namespace Dijitle.Metra.API.Models.Output
{
    public class Route
    {
        [JsonProperty("id")]
        public string route_id { get; set; }

        [JsonProperty("Name")]
        public string route_long_name { get; set; }
    }
}
