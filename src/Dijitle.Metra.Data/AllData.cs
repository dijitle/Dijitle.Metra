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

        private DateTime _lastUpdated;

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

            _lastUpdated = new DateTime(1,1,1);
        }

        public bool IsStale
        {
            get
            {
                return _lastUpdated.AddHours(4) < DateTime.Now;
            }
        }

        public bool IsFresh
        {
            get
            {
                return !IsStale;
            }
        }

        public void LinkItems()
        {
            foreach (CalendarDate cd in CalendarDates)
            {
                cd.Link(Calendars);
            }

            foreach (FareRules fr in FareRules)
            {
                fr.Link(FareAttributes);
            }

            foreach (Routes r in Routes.Values)
            {
                r.Link(Agencies, Shapes);
            }

            foreach (Trips t in Trips.Values)
            {
                t.Link(Routes, Calendars, StopTimes, Stops, Shapes);
            }

            _lastUpdated = DateTime.Now;
        }


        public IEnumerable<Calendar> GetCurrentCalendars(DateTime date)
        {
            return Calendars.Where(c => c.Value.start_date < date)
                            .Where(c => c.Value.end_date.AddDays(1) >= date)
                            .Where(c => c.Value.IsDay(date.DayOfWeek) == true)
                            .Where(c => c.Value.CalendarDates.Any(cd => cd.exception_type != CalendarDate.Exception_Type.Removed || cd.date > date) ||
                                   c.Value.CalendarDates.Count == 0).Select(o => o.Value);
        }
    }
}
