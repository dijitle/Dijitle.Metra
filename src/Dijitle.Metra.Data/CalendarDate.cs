using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
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

        public CalendarDate(string[] csv)
        {
            service_id = csv[0].Trim();
            date = DateTime.ParseExact(csv[1].Trim(),"yyyyMMdd", CultureInfo.InvariantCulture);
            exception_type = (Exception_Type)Convert.ToInt32(csv[2].Trim());
        }

        public void LinkCalendar(IEnumerable<Calendar> calendars)
        {
            foreach (Calendar c in calendars)
            {
                if (c.service_id == service_id)
                {
                    Calendar = c;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return service_id;
        }
    }
}
