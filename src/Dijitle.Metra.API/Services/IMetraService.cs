using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dijitle.Metra.Data;

using Dijitle.Metra.API.Models.Output;

namespace Dijitle.Metra.API.Services
{
    public interface IMetraService
    {
        DateTime CurrentTime { get; }
        Task<IEnumerable<Route>> GetRoutes();
        Task<Fare> GetFare(string origin, string destination);
        Task<IEnumerable<Fare>> GetFares();
        Task<Trip> GetTrip(string id);
        Task<IEnumerable<Trip>> GetTrips(Stops origin, Stops destination, DateTime selectedDate);
        Task<IEnumerable<string>> GetTripsEnroute();
        Task<IEnumerable<Stop>> GetStopsByDistance(double lat, double lon, int milesAway);
        Task<IEnumerable<Stop>> GetAllStops();
        Task<IEnumerable<Stop>> GetStopsByRoute(string route, bool sortAsc);
        Task<IEnumerable<Shape>> GetShapes(Routes route);
        Task<Shape> GetShapes(string id);
        Task<IEnumerable<Models.Output.Calendar>> GetCalendars();
        Task<IEnumerable<Position>> GetAllEstimatedPositions(bool withRealTime = false);
        Task<Position> GetEstimatedPosition(string tripId);
    }
}
