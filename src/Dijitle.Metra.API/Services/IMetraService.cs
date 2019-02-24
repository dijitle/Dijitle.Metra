using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dijitle.Metra.API.Services
{
    public interface IMetraService
    {
        string GetTimes(string start, string dest);
    }
}
