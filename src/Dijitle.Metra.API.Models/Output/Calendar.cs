using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dijitle.Metra.API.Models.Output
{
    public class Calendar
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("monday")]
        public bool Monday { get; set; }

        [JsonPropertyName("tueday")]
        public bool Tuesday { get; set; }

        [JsonPropertyName("wednesday")]
        public bool Wednesday { get; set; }

        [JsonPropertyName("thurday")]
        public bool Thursday { get; set; }

        [JsonPropertyName("friday")]
        public bool Friday { get; set; }

        [JsonPropertyName("satday")]
        public bool Saturday { get; set; }

        [JsonPropertyName("sunday")]
        public bool Sunday { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("calendarDates")]
        public List<CalendarDate> CalendarDates { get; set; }
    }

    public class CalendarDate
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("exceptionType")]
        public string ExceptionType { get; set; }
    }
}
