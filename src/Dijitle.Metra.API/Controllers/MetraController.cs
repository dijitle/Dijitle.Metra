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
        private readonly IMetraService _metra;
        private readonly IGTFSService _gtfs;

        public MetraController(IMetraService metra, IGTFSService gtfs)
        {
            _metra = metra;
            _gtfs = gtfs;
        }

        [HttpGet()]
        [Route("Routes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Route>>> GetLines()
        {
            return Ok(await _metra.GetRoutes());
        }

        [HttpGet()]
        [Route("ShapesByRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Models.Output.Shape>>> GetShapesByRoute(string route = "BNSF")
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            if (!_gtfs.Data.Routes.ContainsKey(route))
            {
                return NotFound($"No route named '{route}' was found!");
            }

            return Ok(await _metra.GetShapes(_gtfs.Data.Routes[route]));
        }

        [HttpGet()]
        [Route("ShapesById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Models.Output.Shape>> GetShapesById(string id = "BNSF_IB_1")
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            if (!_gtfs.Data.Shapes.ContainsKey(id))
            {
                return NotFound($"No shape with id '{id}' was found!");
            }

            return Ok(await _metra.GetShapes(id));
        }

        [HttpGet()]
        [Route("Trips")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Models.Output.Trip>>> GetTrips(string start = "ROUTE59", string dest = "CUS", bool expressOnly = false)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }
            if (!_gtfs.Data.Stops.ContainsKey(start))
            {
                return NotFound($"No stop named '{start}' was found!");
            }

            if (!_gtfs.Data.Stops.ContainsKey(dest))
            {
                return NotFound($"No stop named '{dest}' was found!");
            }

            return Ok(await _metra.GetTrips(_gtfs.Data.Stops[start], _gtfs.Data.Stops[dest], expressOnly));
        }
        
        [HttpGet()]
        [Route("Trips/Enroute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<string>>> GetTripsEnroute()
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }
            
            return Ok(await _metra.GetTripsEnroute());
        }

        [HttpGet()]
        [Route("Trips/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Models.Output.Trip>> GetTrip(string id)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }
            if (!_gtfs.Data.Trips.ContainsKey(id))
            {
                return NotFound($"No trip with id '{id}' was found!");
            }
            
            return Ok(await _metra.GetTrip(id));
        }

        [HttpGet()]
        [Route("Trips/{id}/StopTimes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Models.Output.Stop>>> GetTripStops(string id)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }
            if (!_gtfs.Data.Trips.ContainsKey(id))
            {
                return NotFound($"No trip with id '{id}' was found!");
            }

            return Ok((await _metra.GetTrip(id)).TripStops);
        }

        [HttpGet()]
        [Route("StopsByDistance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Stop>>> GetStopsByDistance(double lat = 41.881276d, double lon = -87.635305d, int distance = 5)
        {
            return Ok(await _metra.GetStopsByDistance(lat, lon, distance));
        }

        [HttpGet()]
        [Route("StopsByRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Stop>>> GetStopsByRoute(string route = "BNSF", bool sortAsc = true)
        {
            return Ok(await _metra.GetStopsByRoute(route, sortAsc));
        }

        [HttpGet()]
        [Route("AllStops")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Stop>>> GetAllStops()
        {
            return Ok(await _metra.GetAllStops());
        }


        [HttpGet()]
        [Route("Calendars")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Calendar>>> GetCalendars()
        {
            return Ok(await _metra.GetCalendars());
        }

        [HttpGet()]
        [Route("EstimatedPositions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Models.Output.Position>> GetEstimatedPosition(string tripId)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }
            if (!_gtfs.Data.Trips.ContainsKey(tripId))
            {
                return NotFound($"No trip with id '{tripId}' was found!");
            }

            return Ok(await _metra.GetEstimatedPosition(tripId));
        }

        [HttpGet()]
        [Route("EstimatedPositions/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Position>>> GetAllEstimatedPositions()
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }
            return Ok(await _metra.GetAllEstimatedPositions());
        }
    }
}