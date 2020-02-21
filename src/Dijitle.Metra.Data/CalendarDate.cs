using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dijitle.Metra.Data
{
    public class CalendarDate
    {
        public string service_id { get; private set; }
        public DateTime date { get; private set; }
        public Exception_Type exception_type { get; private set; }

        public Calendar Calendar { get; private set; }

        public enum Exception_Type
        {
            Added = 1,
            Removed = 2
        }

        public CalendarDate(Dictionary<string, string> dictData)
        {
            service_id = dictData["service_id"];
            date = DateTime.ParseExact(dictData["date"], "yyyyMMdd", CultureInfo.InvariantCulture);
            exception_type = (Exception_Type)Convert.ToInt32(dictData["exception_type"]);
        }

        public void Link(IDictionary<string, Calendar> calendars)
        {
            Calendar c = calendars[service_id];

            Calendar = c;
            c.CalendarDates.Add(this);
            
        }

        public override string ToString()
        {
            return service_id;
        }
    }
}
