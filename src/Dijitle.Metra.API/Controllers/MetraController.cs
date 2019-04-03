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
        [Route("ShapesByRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShapesByRoute(string route = "BNSF")
        {
            if (_gtfs.Data == null)
            {
                await _gtfs.RefreshData();
            }

            if (!_gtfs.Data.Routes.ContainsKey(route))
            {
                return NotFound($"No route named {route} was found!");
            }

            return Ok(await _metra.GetShapes(_gtfs.Data.Routes[route]));
        }

        [HttpGet()]
        [Route("ShapesById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShapesById(string id = "BNSF_IB_1")
        {
            if (_gtfs.Data == null)
            {
                await _gtfs.RefreshData();
            }

            if (!_gtfs.Data.Shapes.ContainsKey(id))
            {
                return NotFound($"No shape with id {id} was found!");
            }

            return Ok(await _metra.GetShapes(id));
        }

        [HttpGet()]
        [Route("Trips")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTrips(string start = "ROUTE59", string dest = "CUS", bool expressOnly = false)
        {
            if (_gtfs.Data == null)
            {
                await _gtfs.RefreshData();
            }
            if (!_gtfs.Data.Stops.ContainsKey(start))
            {
                return NotFound($"No stop named {start} was found!");
            }

            if (!_gtfs.Data.Stops.ContainsKey(dest))
            {
                return NotFound($"No stop named {dest} was found!");
            }

            return Ok(await _metra.GetTrips(_gtfs.Data.Stops[start], _gtfs.Data.Stops[dest], expressOnly));
        }

        [HttpGet()]
        [Route("StopsByDistance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStopsByDistance(double lat = 41.882077d, double lon = -87.627807d, int distance = 5)
        {
            return Ok(await _metra.GetStopsByDistance(lat, lon, distance));
        }

        [HttpGet()]
        [Route("StopsByRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStopsByRoute(string route = "BNSF", bool sortAsc = true)
        {
            return Ok(await _metra.GetStopsByRoute(route, sortAsc));
        }

        [HttpGet()]
        [Route("AllStops")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStops()
        {
            return Ok(await _metra.GetAllStops());
        }
    }
}