using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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

        [JsonProperty("number")]
        public string Number
        {
            get
            {
                if (Id.Contains("_"))
                {
                    return Regex.Match(Id.Split('_')[1], @"\d+$").Value;
                }
                return Id;
            }
        }

        [JsonProperty("inbound")]
        public bool Inbound { get; set; }
        
        [JsonProperty("destinationStop")]
        public Stop DestinationStop { get; set; }

        [JsonProperty("orginStop")]
        public Stop OriginStop { get; set; }

        [JsonProperty("route")]
        public Route Route { get; set; }

        [JsonProperty("shapeId")]
        public string ShapeId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
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
