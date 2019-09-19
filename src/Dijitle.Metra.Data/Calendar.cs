using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Dijitle.Metra.Data
{
    public class Calendar
    {
        public string service_id { get; private set; }
        public bool monday { get; private set; }
        public bool tuesday { get; private set; }
        public bool wednesday { get; private set; }
        public bool thursday { get; private set; }
        public bool friday { get; private set; }
        public bool saturday { get; private set; }
        public bool sunday { get; private set; }
        public DateTime start_date { get; private set; }
        public DateTime end_date { get; private set; }

        public List<CalendarDate> CalendarDates { get; private set; }

        public Calendar(Dictionary<string, string> dictData)
        {
            service_id = dictData["service_id"];
            monday = (dictData["monday"] == "1");
            tuesday = (dictData["tuesday"] == "1");
            wednesday = (dictData["wednesday"] == "1");
            thursday = (dictData["thursday"] == "1");
            friday = (dictData["friday"] == "1");
            saturday = (dictData["saturday"] == "1");
            sunday = (dictData["sunday"] == "1");
            start_date = DateTime.ParseExact(dictData["start_date"], "yyyyMMdd", CultureInfo.InvariantCulture);
            end_date = DateTime.ParseExact(dictData["end_date"], "yyyyMMdd", CultureInfo.InvariantCulture);

            CalendarDates = new List<CalendarDate>();
        }

        public bool IsDay(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return monday;
                case DayOfWeek.Tuesday:
                    return tuesday;
                case DayOfWeek.Wednesday:
                    return wednesday;
                case DayOfWeek.Thursday:
                    return thursday;
                case DayOfWeek.Friday:
                    return friday;
                case DayOfWeek.Saturday:
                    return saturday;
                case DayOfWeek.Sunday:
                    return sunday;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return service_id;
        }
    }
}
