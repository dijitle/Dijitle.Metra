using Dijitle.Metra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dijitle.Metra.API.Services
{
    public interface IGTFSService
    {
        AllData Data { get; }

        Task<IEnumerable<Positions>> GetPositions();
        Task RefreshData();
    }
}
