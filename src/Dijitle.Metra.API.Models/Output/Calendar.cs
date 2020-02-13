using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dijitle.Metra.API.Models.Output
{
    public class Calendar
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("monday")]
        public bool Monday { get; set; }

        [JsonProperty("tueday")]
        public bool Tuesday { get; set; }

        [JsonProperty("wednesday")]
        public bool Wednesday { get; set; }

        [JsonProperty("thurday")]
        public bool Thursday { get; set; }

        [JsonProperty("friday")]
        public bool Friday { get; set; }

        [JsonProperty("satday")]
        public bool Saturday { get; set; }

        [JsonProperty("sunday")]
        public bool Sunday { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("calendarDates")]
        public List<CalendarDate> CalendarDates { get; set; }
    }

    public class CalendarDate
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("exceptionType")]
        public string ExceptionType { get; set; }
    }
}
