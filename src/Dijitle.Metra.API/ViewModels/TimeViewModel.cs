using Dijitle.Metra.API.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dijitle.Metra.API.ViewModels
{
    public class TimeViewModel
    {
        public string Start { get; set; }
        public string Destination { get; set; }
        public IEnumerable<Trip> Trips { get; set; }
    }
}
