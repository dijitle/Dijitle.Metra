using System;
using Newtonsoft.Json;

namespace Dijitle.Metra.Data
{
    public class Positions
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("trip_update")]
        public string TripUpdate { get; set; }

        [JsonProperty("vehicle")]
        public Vehicle Vehicle { get; set; }

        [JsonProperty("alert")]
        public string Alert { get; set; }
    }

    public class Vehicle
    {
        public enum Vehicle_Status
        {
            INCOMING_AT = 0,
            STOPPED_AT = 1,
            IN_TRANSIT_TO = 2
        }

        [JsonProperty("trip")]
        public VehicleTrip Trip { get; set; }

        [JsonProperty("vehicle")]
        public VehicleVehicle VehicleVehicle { get; set; }

        [JsonProperty("position")]
        public VehiclePosition Position { get; set; }

        [JsonProperty("current_stop_sequence")]
        public string CurrentStopSquence { get; set; }

        [JsonProperty("stop_id")]
        public string StopId { get; set; }

        [JsonProperty("current_status")]
        public Vehicle_Status Status { get; set; }

        [JsonProperty("timestamp")]
        public VehicleTimestamp Timestamp { get; set; }

        [JsonProperty("congestion_level")]
        public string CongestionLevel { get; set; }

        [JsonProperty("occupancy_status")]
        public string OccupancyStatus { get; set; }
    }

    public class VehicleTrip
    {
        public enum Schedule_Relationship
        {
            SCHEDULED = 0,
            SKIPPED = 1,
            NO_DATA = 2
        }

        [JsonProperty("trip_id")]
        public string TripId { get; set; }

        [JsonProperty("route_id")]
        public string RouteId { get; set; }

        [JsonProperty("direction_id")]
        public string DirectionId { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        [JsonProperty("schedule_relationship")]
        public Schedule_Relationship ScheduleRelationship { get; set; }
    }

    public class VehicleVehicle
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("license_plate")]
        public string LicensePlate { get; set; }
    }

    public class VehiclePosition
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("bearing")]
        public string Bearing { get; set; }

        [JsonProperty("odometer")]
        public string Odometer { get; set; }

        [JsonProperty("speed")]
        public string Speed { get; set; }
    }

    public class VehicleTimestamp
    {
        [JsonProperty("low")]
        public DateTime Time { get; set; }


        [JsonProperty("high")]
        public string High { get; set; }
        
        [JsonProperty("unsigned")]
        public bool Unsigned { get; set; }
    }
}
