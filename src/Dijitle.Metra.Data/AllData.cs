using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dijitle.Metra.Data
{
    public class AllData
    {
        public List<Agency> _agencies { get; set; } = new List<Agency>();
        public List<Calendar> _calendars { get; set; } = new List<Calendar>();
        public List<CalendarDate> _calendarDates { get; set; } = new List<CalendarDate>();
        public List<FareAttributes> _fareAttributes { get; set; } = new List<FareAttributes>();
        public List<FareRules> _fareRules { get; set; } = new List<FareRules>();
        public List<Routes> _routes { get; set; } = new List<Routes>();
        public List<Shapes> _shapes { get; set; } = new List<Shapes>();
        public List<StopTimes> _stopTimes { get; set; } = new List<StopTimes>();
        public List<Stops> _stops { get; set; } = new List<Stops>();
        public List<Trips> _trips { get; set; } = new List<Trips>();


        public AllData(string folderPath)
        {
            Parallel.ForEach(Directory.GetFiles(folderPath, "*.txt", SearchOption.TopDirectoryOnly), file =>
            {
                List<string> lines = File.ReadAllLines(file).ToList();

                lines.RemoveAt(0); //remove header row

                foreach (string line in lines)
                {
                    switch (Path.GetFileNameWithoutExtension(file))
                    {
                        case "agency":
                            _agencies.Add(new Agency(line.Split(",")));
                            break;
                        case "calendar":
                            _calendars.Add(new Calendar(line.Split(",")));
                            break;
                        case "calendar_dates":
                            _calendarDates.Add(new CalendarDate(line.Split(",")));
                            break;
                        case "fare_attributes":
                            _fareAttributes.Add(new FareAttributes(line.Split(",")));
                            break;
                        case "fare_rules":
                            _fareRules.Add(new FareRules(line.Split(",")));
                            break;
                        case "routes":
                            _routes.Add(new Routes(line.Split(",")));
                            break;
                        case "shapes":
                            _shapes.Add(new Shapes(line.Split(",")));
                            break;
                        case "stop_times":
                            _stopTimes.Add(new StopTimes(line.Split(",")));
                            break;
                        case "stops":
                            _stops.Add(new Stops(line.Split(",")));
                            break;
                        case "trips":
                            _trips.Add(new Trips(line.Split(",")));
                            break;
                        default:
                            break;
                    }
                }
            });

            foreach (CalendarDate cd in _calendarDates)
            {
                cd.LinkCalendar(_calendars);
            }
            foreach (FareRules fr in _fareRules)
            {
                fr.LinkFare(_fareAttributes);
            }

            foreach (Routes r in _routes)
            {
                r.LinkAgency(_agencies);
            }

            foreach (Trips t in _trips)
            {
                t.LinkRouteAndService(_routes, _calendars);
            }

            foreach (StopTimes st in _stopTimes)
            {
                st.LinkTripAndStop(_trips, _stops);
            }
        }

        public Stops FindStop(string id)
        {
            return _stops.SingleOrDefault(s => s.stop_id == id);
        }

        public IEnumerable<Calendar> GetCurrentCalendars(DateTime date)
        {
            return _calendars.Where(c => c.start_date < date && c.end_date.AddDays(1) >= date && c.IsDay(date.DayOfWeek) == true);
        }

        private void GetItems<IMetraData>(IEnumerable<string> lines)
        {
            List<IMetraData> items = new List<IMetraData>();

            foreach (string line in lines)
            {
               //items.Add(new IMetraData(line.Split(",")));
            }

        }
    }
}
