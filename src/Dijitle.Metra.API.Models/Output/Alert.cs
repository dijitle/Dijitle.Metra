using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dijitle.Metra.API.Models.Output
{
    public class Alert
    {
        public Alert()
        {
            AffectedIds = new List<string>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("affectedIds")]
        public List<string> AffectedIds { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("header")]
        public string Header { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
