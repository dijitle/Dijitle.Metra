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

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("number")]
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

        [JsonPropertyName("inbound")]
        public bool Inbound { get; set; }
        
        [JsonPropertyName("destinationStop")]
        public Stop DestinationStop { get; set; }

        [JsonPropertyName("originStop")]
        public Stop OriginStop { get; set; }

        [JsonPropertyName("route")]
        public Route Route { get; set; }

        [JsonPropertyName("shapeId")]
        public string ShapeId { get; set; }

        [JsonIgnore]
        public List<Stop> RouteStops { get; set; }

        [JsonPropertyName("tripStops")]
        public List<Stop> TripStops { get; set; }

        [JsonPropertyName("isExpress")]
        public bool IsExpress { get; set; }

        [JsonPropertyName("stopsIn")]
        public int StopsIn
        {
            get
            {
                return TripStops.IndexOf(OriginStop);
            }
        }

        [JsonPropertyName("stopsUntil")]
        public int StopsUntil
        {
            get
            {
                return TripStops.IndexOf(DestinationStop) - TripStops.IndexOf(OriginStop) - 1;
            }
        }

        [JsonPropertyName("minutes")]
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
