using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dijitle.Metra.API.Models.Output;
using Dijitle.Metra.API.Services;
using Dijitle.Metra.API.ViewModels;

namespace Dijitle.Metra.API.Controllers
{
    [Route("")]
    [Route("times")]
    public class WebController : Controller
    {
        private IMetraService _metra;
        private readonly IGTFSService _gtfs;

        public WebController(IMetraService metra, IGTFSService gtfs)
        {
            _metra = metra;
            _gtfs = gtfs;
        }

        [HttpGet()]
        public async Task<IActionResult> Index(string start = "ROUTE59", string dest = "CUS", bool expressOnly = false)
        {
            if(_gtfs.Data == null)
            {
                await _gtfs.RefreshData();
            }
            var tvm = new TimeViewModel()
            {
                Times = await _metra.GetTimes(_gtfs.Data.Stops[start], _gtfs.Data.Stops[dest], expressOnly),
                Start = _gtfs.Data.Stops[start].stop_name,
                Destination = _gtfs.Data.Stops[dest].stop_name
            };

            return View(tvm);
        }
    }
}