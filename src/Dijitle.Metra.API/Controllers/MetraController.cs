using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dijitle.Metra.API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Dijitle.Metra.API.Controllers
{
    [Route("api/[controller]")]
    public class MetraController : Controller
    {
        private IMetraService _metra;

        public MetraController(IMetraService metra)
        {
            _metra = metra;
        }

        [HttpGet()]
        [Route("Routes")]
        public async Task<IActionResult> GetLines()
        {
            return Ok(_metra.GetRoutes());
        }

        [HttpGet()]
        [Route("Times")]
        public async Task<IActionResult> GetTimes(string start = "ROUTE59", string dest = "CUS", bool expressOnly = false)
        {
            return Ok(_metra.GetTimes(start, dest, expressOnly));
        }

    }
}