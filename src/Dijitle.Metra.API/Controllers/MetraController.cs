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
            return Ok(await _metra.GetRoutes());
        }

        [HttpGet()]
        [Route("Times")]
        public async Task<IActionResult> GetTimes(string start = "ROUTE59", string dest = "CUS", bool expressOnly = false)
        {
            return Ok(await _metra.GetTimes(start, dest, expressOnly));
        }

        [HttpGet()]
        [Route("Stops")]
        public async Task<IActionResult> GetStops(decimal lat = 41.769649m, decimal lon = -88.215297m, int distance = 5)
        {
            return Ok(await _metra.GetStops(lat, lon, distance));
        }
    }
}