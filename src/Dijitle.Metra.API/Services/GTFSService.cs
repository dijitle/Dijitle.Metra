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

namespace Dijitle.Metra.API.Services
{
    public class GTFSService : IGTFSService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        public AllData Data { get; private set; }

        public GTFSService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            Data = new AllData();
        }

        public async Task<IEnumerable<Position>> GetPositions()
        {
            HttpClient client = _httpClientFactory.CreateClient("GTFSClient");

            var response = await client.GetAsync("/gtfs/positions");
            var content = await response.Content.ReadAsStringAsync();
            
            List<Positions> pos = JsonConvert.DeserializeObject<List<Positions>>(content);

            List<Position> retPositions = new List<Position>();

            foreach (Positions p in pos)
            {
                retPositions.Add(new Position()
                {
                    Id = p.Id,
                    TripId = p.Vehicle.Trip.TripId,
                    Direction = Data.Trips[p.Vehicle.Trip.TripId].direction_id == Trips.Direction.Inbound,
                    Label = p.Vehicle.VehicleVehicle.Label,
                    Latitude = p.Vehicle.Position.Latitude,
                    Longitude = p.Vehicle.Position.Longitude
                }); ;
            }

            return retPositions;
        }

        public async Task<IEnumerable<Alert>> GetAlerts()
        {
            HttpClient client = _httpClientFactory.CreateClient("GTFSClient");

            var response = await client.GetAsync("/gtfs/alerts");
            var content = await response.Content.ReadAsStringAsync();

            List<Alerts> alerts = JsonConvert.DeserializeObject<List<Alerts>>(content);

            List<Alert> retAlerts = new List<Alert>();

            foreach(Alerts a in alerts)
            {
                Alert alert = new Alert()
                {
                    Id = a.id,
                    URL = a.alert.url.translation.FirstOrDefault().text,
                    Header = a.alert.header_text.translation.FirstOrDefault().text,
                    Description = a.alert.description_text.translation.FirstOrDefault().text
                };

                foreach(InformedEntity ie in a.alert.informed_entity)
                {
                    if(ie.trip == null)
                    {
                        alert.AffectedIds.Add(ie.route_id);
                    }
                    else
                    {
                        alert.AffectedIds.Add(ie.trip.trip_id);
                    }
                }

                retAlerts.Add(alert);
            }

            return retAlerts;
        }

        public async Task RefreshData()
        {
            HttpClient client = _httpClientFactory.CreateClient("GTFSClient");

            Data = new AllData();

            foreach (var dir in Directory.GetFiles("./wwwroot/nictd"))
            {
                using (StreamReader rdr = new StreamReader(dir))
                {

                    ParseFiles(rdr, Path.GetFileNameWithoutExtension(dir));
                }   
            }


            var response = await client.GetAsync("/gtfs/raw/schedule.zip");
            var content = await response.Content.ReadAsStreamAsync();


            using (ZipArchive za = new ZipArchive(content, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in za.Entries)
                {
                    using (StreamReader rdr = new StreamReader(entry.Open()))
                    {

                        ParseFiles(rdr, Path.GetFileNameWithoutExtension(entry.FullName));
                    }
                }
            }


            Data.LinkItems();
            return;
        }

        private void ParseFiles(StreamReader rdr, string fileName)
        {
            var keys = rdr.ReadLine().Split(",");

            switch (fileName)
            {
                case "agency":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine().Split(",");

                        var dictData = new Dictionary<string, string>();

                        for (int i = 0; i < keys.Length; i++)
                        {
                            dictData.Add(keys[i].Trim(), line[i].Trim());
                        }

                        Data.Agencies.Add(dictData["agency_id"], new Agency(dictData));
                    }
                    break;
                case "calendar":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine().Split(",");

                        var dictData = new Dictionary<string, string>();

                        for (int i = 0; i < keys.Length; i++)
                        {
                            dictData.Add(keys[i].Trim(), line[i].Trim());
                        }

                        Data.Calendars.Add(dictData["service_id"], new Data.Calendar(dictData));
                    }
                    break;
                case "calendar_dates":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine().Split(",");

                        var dictData = new Dictionary<string, string>();

                        for (int i = 0; i < keys.Length; i++)
                        {
                            dictData.Add(keys[i].Trim(), line[i].Trim());
                        }

                        Data.CalendarDates.Add(new Data.CalendarDate(dictData));
                    }
                    break;
                case "fare_attributes":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine().Split(",");

                        var dictData = new Dictionary<string, string>();

                        for (int i = 0; i < keys.Length; i++)
                        {
                            dictData.Add(keys[i].Trim(), line[i].Trim());
                        }

                        Data.FareAttributes.Add(dictData["fare_id"], new FareAttributes(dictData));
                    }
                    break;
                case "fare_rules":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine().Split(",");

                        var dictData = new Dictionary<string, string>();

                        for (int i = 0; i < keys.Length; i++)
                        {
                            dictData.Add(keys[i].Trim(), line[i].Trim());
                        }

                        Data.FareRules.Add(new FareRules(dictData));
                    }
                    break;
                case "routes":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine().Split(",");

                        var dictData = new Dictionary<string, string>();

                        for (int i = 0; i < keys.Length; i++)
                        {
                            dictData.Add(keys[i].Trim(), line[i].Trim());
                        }

                        Data.Routes.Add(dictData["route_id"], new Routes(dictData));
                    }
                    break;
                case "shapes":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine();
                        string key = line.Split(",").FirstOrDefault().Trim();
                        if (!Data.Shapes.ContainsKey(key))
                        {
                            Data.Shapes.Add(key, new List<Shapes>());
                        }
                        Data.Shapes[key].Add(new Shapes(line.Split(",")));
                    }
                    break;
                case "stop_times":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine();
                        string key = line.Split(",").FirstOrDefault().Trim();
                        if (!Data.StopTimes.ContainsKey(key))
                        {
                            Data.StopTimes.Add(key, new List<StopTimes>());
                        }
                        Data.StopTimes[key].Add(new StopTimes(line.Split(",")));
                    }
                    break;
                case "stops":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine().Split(",");

                        var dictData = new Dictionary<string, string>();

                        for (int i = 0; i < keys.Length; i++)
                        {
                            dictData.Add(keys[i].Trim(), line[i].Trim());
                        }

                        Data.Stops.Add(dictData["stop_id"], new Stops(dictData));
                    }
                    break;
                case "trips":
                    while (!rdr.EndOfStream)
                    {
                        var line = rdr.ReadLine().Split(",");

                        var dictData = new Dictionary<string, string>();

                        for (int i = 0; i < keys.Length; i++)
                        {
                            dictData.Add(keys[i].Trim(), line[i].Trim());
                        }

                        Data.Trips.Add(dictData["trip_id"], new Trips(dictData));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
