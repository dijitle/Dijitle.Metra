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
        Task<IEnumerable<Route>> GetRoutes();
        Task<Trip> GetTrip(string id);
        Task<IEnumerable<Trip>> GetTrips(Stops origin, Stops destination, bool expressOnly);
        Task<IEnumerable<Trip>> GetTripsEnroute();
        Task<IEnumerable<Stop>> GetStopsByDistance(double lat, double lon, int milesAway);
        Task<IEnumerable<Stop>> GetAllStops();
        Task<IEnumerable<Stop>> GetStopsByRoute(string route, bool sortAsc);
        Task<IEnumerable<Shape>> GetShapes(Routes route);
        Task<Shape> GetShapes(string id);
        Task<IEnumerable<Models.Output.Calendar>> GetCalendars();
        Task<IEnumerable<Position>> GetAllEstimatedPositions();
        Task<Position> GetEstimatedPosition(string tripId);
    }
}
