using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dijitle.Metra.Data
{
    public class AllData
    {
        public List<Agency> Agencies { get; set; }
        public List<Calendar> Calendars { get; set; }
        public List<CalendarDate> CalendarDates { get; set; }
        public List<FareAttributes> FareAttributes { get; set; }
        public List<FareRules> FareRules { get; set; }
        public List<Routes> Routes { get; set; }
        public List<Shapes> Shapes { get; set; }
        public List<StopTimes> StopTimes { get; set; }
        public List<Stops> Stops { get; set; }
        public List<Trips> Trips { get; set; }

        public AllData()
        {
            Agencies = new List<Agency>();
            Calendars = new List<Calendar>();
            CalendarDates = new List<CalendarDate>();
            FareAttributes = new List<FareAttributes>();
            FareRules = new List<FareRules>();
            Routes = new List<Routes>();
            Shapes = new List<Shapes>();
            StopTimes = new List<StopTimes>();
            Stops = new List<Stops>();
            Trips = new List<Trips>();
        }

        public void LinkItems()
        {
            Parallel.ForEach(CalendarDates, cd =>
            {
                cd.LinkCalendar(Calendars);
            });

            Parallel.ForEach(FareRules, fr =>
            {
                fr.LinkFare(FareAttributes);
            });

            Parallel.ForEach(Routes, r =>
            {
                r.LinkAgency(Agencies);
            });

            Parallel.ForEach(Trips, t =>
            {
                t.LinkRouteAndService(Routes, Calendars);
            });

            Parallel.ForEach(StopTimes, st =>
            {
                st.LinkTripAndStop(Trips, Stops);
            });
        }

        public Stops FindStop(string id)
        {
            return Stops.SingleOrDefault(s => s.stop_id == id);
        }

        public IEnumerable<Calendar> GetCurrentCalendars(DateTime date)
        {
            return Calendars.Where(c => c.start_date < date && c.end_date.AddDays(1) >= date  && c.IsDay(date.DayOfWeek) == true);
        }
    }
}
