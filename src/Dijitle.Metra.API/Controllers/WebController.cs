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
    [Route("Trips")]
    [ApiExplorerSettings(IgnoreApi = true)]
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
        public async Task<IActionResult> Index(string start = "ROUTE59", string dest = "CUS", string selectedDate = "")
        {
            if(_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            DateTime d;
            if(!DateTime.TryParse(selectedDate, out d))
            {
                d = _metra.CurrentTime;
            }

            var tvm = new TimeViewModel()
            {
                Trips = await _metra.GetTrips(_gtfs.Data.Stops[start], _gtfs.Data.Stops[dest], d),
                StartID = start,
                DestID = dest,
                SelectedDate = d,
                Start = _gtfs.Data.Stops[start].stop_name.Replace("Chicago Union Station", "Union Station"),
                Destination = _gtfs.Data.Stops[dest].stop_name.Replace("Chicago Union Station", "Union Station")
            };

            return View(tvm);
        }

        [HttpGet()]
        [Route("map")]
        public async Task<IActionResult> Map()
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            return View();
        }
    }
}