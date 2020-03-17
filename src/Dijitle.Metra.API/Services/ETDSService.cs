using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Compression;
using Dijitle.Metra.Data;
using Dijitle.Metra.API.Models.Output;
using Prometheus;

namespace Dijitle.Metra.API.Services
{
    public class ETDSService : IETDSService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGTFSService _gtfs;
        private readonly IMetraService _metra;

        public ETDSService(IHttpClientFactory httpClientFactory, IGTFSService gtfs, IMetraService metra)
        {
            _gtfs = gtfs;
            _metra = metra;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Speed>> GetSpeed(string tripId)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }

            var data = (await getData()).Where(d => d.tripId == tripId);

            var speeds = new List<Speed>();
            var trip = await _metra.GetTrip(tripId);
            var shapes = await _metra.GetShapes(trip.ShapeId);
            var currentSpeed = 0d;
            var i = 0;

            foreach(var s in data)
            {
                var found = false;

                currentSpeed = s.AVG_SPEED_FROM_PREV_SEQ;

                while (!found)
                {
                    var p = shapes.Points[i];

                    if (p.Sequence == s.sequence)
                    {
                        found = true;
                    }

                    speeds.Add(new Speed()
                    {
                        Lat = p.Lat,
                        Lon = p.Lon,
                        Sequence = p.Sequence,
                        AvgSpeed = currentSpeed
                    });

                    i++;
                }
            }

            while(i < shapes.Points.Count)
            {
                var p = shapes.Points[i];

                speeds.Add(new Speed()
                {
                    Lat = p.Lat,
                    Lon = p.Lon,
                    Sequence = p.Sequence,
                    AvgSpeed = currentSpeed
                });
            }
            return speeds;
        }

        public async Task<IEnumerable<Trip>> GetTrips()
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }
            var speeds = await getData();

            var trips = new List<Trip>();

            foreach(var tripId in speeds.Select(s => s.tripId).Distinct())
            {
                var t = await _metra.GetTrip(tripId);
                if(t != null)
                {
                    trips.Add(t);
                }
            }
            return trips;
        }

        private async  Task<IEnumerable<Speeds>> getData()
        {
            return JsonConvert.DeserializeObject<IEnumerable<Speeds>>(await File.ReadAllTextAsync("./wwwroot/etds/inbound.json"));
        }
    }
}
