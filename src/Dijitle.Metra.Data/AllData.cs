using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dijitle.Metra.Data
{
    public class AllData
    {
        public Dictionary<string, Agency> Agencies { get; set; }
        public Dictionary<string, Calendar> Calendars { get; set; }
        public List<CalendarDate> CalendarDates { get; set; }
        public Dictionary<int, FareAttributes> FareAttributes { get; set; }
        public List<FareRules> FareRules { get; set; }
        public Dictionary<string, Routes> Routes { get; set; }
        public Dictionary<string, List<Shapes>> Shapes { get; set; }
        public Dictionary<string, List<StopTimes>> StopTimes { get; set; }
        public Dictionary<string, Stops> Stops { get; set; }
        public Dictionary<string, Trips> Trips { get; set; }

        public AllData()
        {
            Agencies = new Dictionary<string, Agency>();
            Calendars = new Dictionary<string, Calendar>();
            CalendarDates = new List<CalendarDate>();
            FareAttributes = new Dictionary<int, FareAttributes>();
            FareRules = new List<FareRules>();
            Routes = new Dictionary<string, Routes>();
            Shapes = new Dictionary<string, List<Shapes>>();
            StopTimes = new Dictionary<string, List<StopTimes>>();
            Stops = new Dictionary<string, Stops>();
            Trips = new Dictionary<string, Trips>();
        }

        public void LinkItems()
        {
            foreach (CalendarDate cd in CalendarDates)
            {
                cd.LinkCalendar(Calendars);
            }

            foreach (FareRules fr in FareRules)
            {
                fr.LinkFare(FareAttributes);
            }

            foreach (Routes r in Routes.Values)
            {
                r.LinkAgency(Agencies);
            }

            foreach (Trips t in Trips.Values)
            {
                t.LinkRouteAndService(Routes, Calendars);
            }

            foreach (List<StopTimes> kvpst in StopTimes.Values)
            {
                foreach(StopTimes st in kvpst)
                {
                    st.LinkTripAndStop(Trips, Stops);
                }
            }
        }


        public IEnumerable<Calendar> GetCurrentCalendars(DateTime date)
        {
            return Calendars.Where(c => c.Value.start_date < date && c.Value.end_date.AddDays(1) >= date  && c.Value.IsDay(date.DayOfWeek) == true).Select(o => o.Value);
        }
    }
}
