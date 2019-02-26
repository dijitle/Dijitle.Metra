using Newtonsoft.Json;
using System;

namespace Dijitle.Metra.API.Models.Output
{
    public class Time
    {
        [JsonProperty("Id")]
        public string trip_id { get; set; }

        [JsonProperty("ArrivalTimeAtDestination")]
        public DateTime arrival_time { get; set; }

        [JsonProperty("DepartureTimeAtOrgin")]
        public DateTime departure_time { get; set; }

        [JsonProperty("IsExpress")]
        public bool IsExpress { get; set; }

        [JsonProperty("StopsIn")]
        public int StopsIn { get; set; }

        [JsonProperty("StopsUntil")]
        public int StopsUntil { get; set; }

        [JsonProperty("Minutes")]
        public int Minutes
        {
            get
            {
                TimeSpan ts = new TimeSpan(arrival_time.Ticks - departure_time.Ticks);
                return (int)Math.Ceiling(ts.TotalMinutes);
            }
        }
    }
}
