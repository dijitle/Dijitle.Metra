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
        IEnumerable<Route> GetRoutes();
        IEnumerable<Time> GetTimes(string origin, string destination, bool expressOnly);
        IEnumerable<Stop> GetStops(decimal lat, decimal lon, int milesAway);
    }
}
