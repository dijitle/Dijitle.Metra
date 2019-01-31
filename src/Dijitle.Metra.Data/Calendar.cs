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

        public Calendar(string[] csv)
        {
            service_id = csv[0].Trim();
            monday = (csv[1].Trim() == "1");
            tuesday = (csv[2].Trim() == "1");
            wednesday = (csv[3].Trim() == "1");
            thursday = (csv[4].Trim() == "1");
            friday = (csv[5].Trim() == "1");
            saturday = (csv[6].Trim() == "1");
            sunday = (csv[7].Trim() == "1");
            start_date = DateTime.ParseExact(csv[8].Trim(),"yyyyMMdd", CultureInfo.InvariantCulture);
            end_date = DateTime.ParseExact(csv[9].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
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
