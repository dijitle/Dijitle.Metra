using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dijitle.Metra.API.Models.Output
{
    public class Trip
    {
        public Trip()
        {
            RouteStops = new List<Stop>();
            TripStops = new List<Stop>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }
              
        [JsonProperty("destinationStop")]
        public Stop DestinationStop { get; set; }

        [JsonProperty("orginStop")]
        public Stop OriginStop { get; set; }

        [JsonIgnore]
        public List<Stop> RouteStops { get; set; }

        [JsonProperty("tripStops")]
        public List<Stop> TripStops { get; set; }

        [JsonProperty("isExpress")]
        public bool IsExpress { get; set; }

        [JsonProperty("stopsIn")]
        public int StopsIn
        {
            get
            {
                return TripStops.IndexOf(OriginStop);
            }
        }

        [JsonProperty("stopsUntil")]
        public int StopsUntil
        {
            get
            {
                return TripStops.IndexOf(DestinationStop) - TripStops.IndexOf(OriginStop) - 1;
            }
        }

        [JsonProperty("minutes")]
        public int Minutes
        {
            get
            {
                TimeSpan ts = new TimeSpan(DestinationStop.ArrivalTime.Ticks - OriginStop.DepartureTime.Ticks);
                return (int)Math.Ceiling(ts.TotalMinutes);
            }
        }
    }
}
