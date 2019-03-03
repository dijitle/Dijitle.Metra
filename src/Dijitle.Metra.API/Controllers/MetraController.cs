using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dijitle.Metra.API.Services;
using Dijitle.Metra.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Dijitle.Metra.API.Controllers
{
    [Route("api/[controller]")]
    public class MetraController : Controller
    {
        private IMetraService _metra;
        private readonly IGTFSService _gtfs;

        public MetraController(IMetraService metra, IGTFSService gtfs)
        {
            _metra = metra;
            _gtfs = gtfs;
        }

        [HttpGet()]
        [Route("Routes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLines()
        {
            return Ok(await _metra.GetRoutes());
        }

        [HttpGet()]
        [Route("Shapes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShapes(string route = "BNSF")
        {
            if (!_gtfs.Data.Routes.ContainsKey(route))
            {
                return NotFound($"No route named {route} was found!");
            }


            return Ok(await _metra.GetShapes(_gtfs.Data.Routes[route]));
        }

        [HttpGet()]
        [Route("Times")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTimes(string start = "ROUTE59", string dest = "CUS", bool expressOnly = false)
        {
            if (!_gtfs.Data.Stops.ContainsKey(start))
            {
                return NotFound($"No stop named {start} was found!");
            }

            if (!_gtfs.Data.Stops.ContainsKey(dest))
            {
                return NotFound($"No stop named {dest} was found!");
            }

            return Ok(await _metra.GetTimes(_gtfs.Data.Stops[start], _gtfs.Data.Stops[dest], expressOnly));
        }

        [HttpGet()]
        [Route("Stops")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStops(decimal lat = 41.769649m, decimal lon = -88.215297m, int distance = 5)
        {
            return Ok(await _metra.GetStops(lat, lon, distance));
        }
    }
}