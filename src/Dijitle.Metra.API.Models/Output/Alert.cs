using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dijitle.Metra.API.Models.Output
{
    public class Alert
    {
        public Alert()
        {
            AffectedIds = new List<string>();
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("affectedIds")]
        public List<string> AffectedIds { get; set; }

        [JsonPropertyName("url")]
        public string URL { get; set; }

        [JsonPropertyName("header")]
        public string Header { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
