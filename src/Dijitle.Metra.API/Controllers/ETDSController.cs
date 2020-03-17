using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dijitle.Metra.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dijitle.Metra.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ETDSController : Controller
    {
        private IETDSService _etds;

        public ETDSController(IETDSService etds)
        {
            _etds = etds;
        }

        [HttpGet()]
        [Route("Trips")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Trip>>> GetTrips()
        {
            return Ok(await _etds.GetTrips());
        }

        [HttpGet()]
        [Route("Speed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Speed>>> GetSpeed(string tripId)
        {
            return Ok(await _etds.GetSpeed(tripId));
        }
    }
}