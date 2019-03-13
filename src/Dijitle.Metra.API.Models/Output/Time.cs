using Newtonsoft.Json;
using System;

namespace Dijitle.Metra.API.Models.Output
{
    public class Time
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("arrivalTimeAtDestination")]
        public DateTime ArrivalTime { get; set; }

        [JsonProperty("departureTimeAtOrgin")]
        public DateTime DepartureTime { get; set; }

        [JsonProperty("destinationStop")]
        public Stop DestinationStop { get; set; }

        [JsonProperty("orginStop")]
        public Stop OriginStop { get; set; }

        [JsonProperty("isExpress")]
        public bool IsExpress { get; set; }

        [JsonProperty("stopsIn")]
        public int StopsIn { get; set; }

        [JsonProperty("stopsUntil")]
        public int StopsUntil { get; set; }

        [JsonProperty("minutes")]
        public int Minutes
        {
            get
            {
                TimeSpan ts = new TimeSpan(ArrivalTime.Ticks - DepartureTime.Ticks);
                return (int)Math.Ceiling(ts.TotalMinutes);
            }
        }
    }
}
