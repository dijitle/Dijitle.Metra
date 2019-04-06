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
                    Label = p.Vehicle.VehicleVehicle.Label,
                    Latitude = p.Vehicle.Position.Latitude,
                    Longitude = p.Vehicle.Position.Longitude
                });
            }

            return retPositions;
        }

        public async Task<object> GetAlerts()
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
            
            var response = await client.GetAsync("/gtfs/raw/schedule.zip");
            var content = await response.Content.ReadAsStreamAsync();
            string line;

            using (ZipArchive za = new ZipArchive(content, ZipArchiveMode.Read))
            {
                foreach(ZipArchiveEntry entry in za.Entries)
                {
                    using (StreamReader rdr = new StreamReader(entry.Open()))
                    {

                        rdr.ReadLine();

                        switch (Path.GetFileNameWithoutExtension(entry.FullName))
                        {
                            case "agency":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.Agencies.Add(line.Split(",").FirstOrDefault().Trim(), new Agency(line.Split(",")));
                                }
                                break;
                            case "calendar":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.Calendars.Add(line.Split(",").FirstOrDefault().Trim(), new Calendar(line.Split(",")));
                                }
                                break;
                            case "calendar_dates":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.CalendarDates.Add(new CalendarDate(line.Split(",")));
                                }
                                break;
                            case "fare_attributes":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.FareAttributes.Add(Convert.ToInt32(line.Split(",").FirstOrDefault().Trim()), new FareAttributes(line.Split(",")));
                                }
                                break;
                            case "fare_rules":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.FareRules.Add(new FareRules(line.Split(",")));
                                }
                                break;
                            case "routes":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.Routes.Add(line.Split(",").FirstOrDefault().Trim(), new Routes(line.Split(",")));
                                }
                                break;
                            case "shapes":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    string key = line.Split(",").FirstOrDefault().Trim();
                                    if(!Data.Shapes.ContainsKey(key))
                                    {
                                        Data.Shapes.Add(key, new List<Shapes>());
                                    }
                                    Data.Shapes[key].Add(new Shapes(line.Split(",")));
                                }
                                break;
                            case "stop_times":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
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
                                    line = rdr.ReadLine();
                                    Data.Stops.Add(line.Split(",").FirstOrDefault().Trim(), new Stops(line.Split(",")));
                                }
                                break;
                            case "trips":
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.Trips.Add(line.Split(",")[2].Trim(), new Trips(line.Split(",")));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            Data.LinkItems();
            return;
        }
    }
}
