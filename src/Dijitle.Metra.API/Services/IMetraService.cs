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
        Task<IEnumerable<Time>> GetTimes(Stops origin, Stops destination, bool expressOnly);
        Task<IEnumerable<Stop>> GetStopsByDistance(decimal lat, decimal lon, int milesAway);
        Task<IEnumerable<Stop>> GetAllStops();
        Task<IEnumerable<Stop>> GetStopsByRoute(string route, bool sortAsc);
        Task<IEnumerable<Shape>> GetShapes(Routes route);
    }
}
