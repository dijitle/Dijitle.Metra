﻿using System;
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
        Task<IEnumerable<Time>> GetTimes(string origin, string destination, bool expressOnly);
        Task<IEnumerable<Stop>> GetStops(decimal lat, decimal lon, int milesAway);
    }
}
