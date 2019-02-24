using Dijitle.Metra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijitle.Metra.API.Services
{
    public class MetraService : IMetraService
    {
        private AllData _data;

        public MetraService()
        {
            _data = new AllData(@"X:\SourceCode\Dijitle.Metra\data");
        }

        public string GetTimes(string start, string dest)
        {
            DateTime selectedDate = DateTime.Now.AddDays(1);
            string selectedRoute = "BNSF";
            string selectedOrigin = "CUS";
            string selectedDestination = "ROUTE59";

            IEnumerable<Calendar> currentCalendars = _data._calendars.Where(c => c.start_date < selectedDate && c.end_date.AddDays(1) >= selectedDate && c.IsDay(selectedDate.DayOfWeek) == true);

            StringBuilder sb = new StringBuilder();

            Routes route = _data._routes.Single(r => r.route_id == selectedRoute);
            Stops origin = _data._stops.Single(s => s.stop_id == selectedOrigin);
            Stops destination = _data._stops.Single(s => s.stop_id == selectedDestination);

            foreach (Trips t in route.Trips.Where(t => currentCalendars.Contains(t.Calendar) && t.StopTimes.Any(st => st.Stop == origin) && t.StopTimes.Any(st => st.Stop == destination) && t.StopTimes.Single(st => st.Stop == origin).stop_sequence < t.StopTimes.Single(st => st.Stop == destination).stop_sequence).OrderBy(t => t.StopTimes.Single(st => st.Stop == origin).departure_time))
            {
                Stops firstStop = t.StopTimes.OrderBy(st => st.stop_sequence).FirstOrDefault().Stop;
                Stops lastStop = t.StopTimes.OrderBy(st => st.stop_sequence).LastOrDefault().Stop;

                StopTimes originStopTime = t.StopTimes.Single(st => st.Stop == origin);
                StopTimes destinationStopTime = t.StopTimes.Single(st => st.Stop == destination);

                int indexOrigin = t.StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(originStopTime);
                int indexDestination = t.StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(destinationStopTime);

                TimeSpan ts = new TimeSpan(destinationStopTime.arrival_time.Ticks - originStopTime.arrival_time.Ticks);

                sb.AppendLine($"{t.trip_id} : {firstStop.stop_name} to {lastStop.stop_name}{(t.IsExpress(originStopTime, destinationStopTime) ? " - Express" : "")}");
                sb.AppendLine($"   Total Stops: {t.StopTimes.Count}");
                sb.AppendLine($"   Stops In at {origin.stop_name}: {indexOrigin}");
                sb.AppendLine($"   Stops until {destination.stop_name}: {indexDestination - indexOrigin - 1}");
                sb.AppendLine($"   Time: {originStopTime.arrival_time.ToShortTimeString()} to {destinationStopTime.arrival_time.ToShortTimeString()} ({Math.Ceiling(ts.TotalMinutes)} minutes)");
            }

            return sb.ToString();            
        }
    }
}
