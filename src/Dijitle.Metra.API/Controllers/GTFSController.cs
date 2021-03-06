﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dijitle.Metra.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Dijitle.Metra.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GTFSController : Controller
    {
        private IGTFSService _gtfs;

        public GTFSController(IGTFSService gtfs)
        {
            _gtfs = gtfs;
        }

        [HttpGet()]
        [Route("Positions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Position>>> GetPositions()
        {
            return Ok(await _gtfs.GetPositions());
        }

        [HttpGet()]
        [Route("Alerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.Output.Alert>>> GetAlerts()
        {
            return Ok(await _gtfs.GetAlerts());
        }

        [HttpPost()]
        [Route("RefreshData")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RefreshData()
        {
            await _gtfs.RefreshData();
            return NoContent();
        }

    }
}