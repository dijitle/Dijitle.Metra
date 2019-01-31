using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Dijitle.Metra.Data;

namespace Dijitle.Metra.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            AllData data = new Data.AllData(@"X:\SourceCode\Dijitle.Metra\data");
            
            DateTime selectedDate = DateTime.Now;
            string selectedRoute = "BNSF";
            string selectedOrigin = "CUS";
            string selectedDestination = "ROUTE59";

            IEnumerable<Calendar> currentCalendars = data._calendars.Where(c => c.start_date < selectedDate && c.end_date.AddDays(1) >= selectedDate && c.IsDay(selectedDate.DayOfWeek) == true);

            StringBuilder sb = new StringBuilder();

            Routes route = data._routes.Single(r => r.route_id == selectedRoute);
            Stops origin = data._stops.Single(s => s.stop_id == selectedOrigin);
            Stops destination = data._stops.Single(s => s.stop_id == selectedDestination);

            foreach (Trips t in route.Trips.Where(t => currentCalendars.Contains(t.Calendar) && t.StopTimes.Any(st => st.Stop == origin) && t.StopTimes.Any(st => st.Stop == destination) && t.StopTimes.Single(st => st.Stop == origin).stop_sequence < t.StopTimes.Single(st => st.Stop == destination).stop_sequence).OrderBy(t => t.StopTimes.Single(st => st.Stop == origin).departure_time))
            {
                Stops firstStop = t.StopTimes.OrderBy(st => st.stop_sequence).FirstOrDefault().Stop;
                Stops lastStop = t.StopTimes.OrderBy(st => st.stop_sequence).LastOrDefault().Stop;

                StopTimes originStopTime = t.StopTimes.Single(st => st.Stop == origin);
                StopTimes destinationStopTime = t.StopTimes.Single(st => st.Stop == destination);

                int indexOrigin = t.StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(originStopTime);
                int indexDestination = t.StopTimes.OrderBy(st => st.stop_sequence).ToList().IndexOf(destinationStopTime);

                TimeSpan ts = new TimeSpan(destinationStopTime.arrival_time.Ticks - originStopTime.arrival_time.Ticks);

                sb.AppendLine($"{t.trip_id} : {firstStop.stop_name} to {lastStop.stop_name}{(t.IsExpress(originStopTime, destinationStopTime) ? " - Express" : "" )}");
                sb.AppendLine($"   Total Stops: {t.StopTimes.Count}");
                sb.AppendLine($"   Stops In at {origin.stop_name}: {indexOrigin}");
                sb.AppendLine($"   Stops until {destination.stop_name}: {indexDestination - indexOrigin - 1}");
                sb.AppendLine($"   Time: {originStopTime.arrival_time.ToShortTimeString()} to {destinationStopTime.arrival_time.ToShortTimeString()} ({Math.Ceiling(ts.TotalMinutes)} minutes)");
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(sb.ToString());
            });
        }
    }
}
