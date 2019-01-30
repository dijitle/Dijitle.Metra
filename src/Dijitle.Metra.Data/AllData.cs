using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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


        public AllData(string folderPath)
        {
            foreach (string file in Directory.GetFiles(folderPath, "*.txt", SearchOption.TopDirectoryOnly))
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
                        default:
                            break;
                    }

                }
            }

            foreach(CalendarDate cd in _calendarDates)
            {
                cd.LinkCalendar(_calendars);
            }
            foreach(FareRules fr in _fareRules)
            {
                fr.LinkFair(_fareAttributes);
            }

            foreach(Routes r in _routes)
            {
                r.LinkAgency(_agencies);
            }
        }
    }
}
