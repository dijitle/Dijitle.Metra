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
        private IGTFSService _gtfs;

        public MetraController(IMetraService metra, IGTFSService gtfs)
        {
            _metra = metra;
            _gtfs = gtfs;
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

        [HttpGet()]
        [Route("Positions")]
        public async Task<IActionResult> GetPositions()
        {
            return Ok(await _gtfs.GetPositions());
        }

        [HttpGet()]
        [Route("Data")]
        public async Task<IActionResult> GetData()
        {
            await _gtfs.RefreshData();
            return NoContent();
        }

    }
}