using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dijitle.Metra.Data;

using Dijitle.Metra.API.Models.Output;

namespace Dijitle.Metra.API.Services
{
    public interface IETDSService
    {
        Task<IEnumerable<Trip>> GetTrips();
        Task<IEnumerable<Speed>> GetSpeed(string tripId);
    }
}
