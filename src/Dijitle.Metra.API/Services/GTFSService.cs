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

namespace Dijitle.Metra.API.Services
{
    public class GTFSService : IGTFSService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AllData Data { get; private set; }

        public GTFSService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Positions>> GetPositions()
        {
            HttpClient client = _httpClientFactory.CreateClient("GTFSClient");

            var response = await client.GetAsync("/gtfs/positions");
            var content = await response.Content.ReadAsStringAsync();
            
            List<Positions> pos = JsonConvert.DeserializeObject<List<Positions>>(content);

            return pos;
        }

        public async Task RefreshData()
        {
            HttpClient client = _httpClientFactory.CreateClient("GTFSClient");

            Data = new AllData();

            List<string> entries = new List<string>();

            var response = await client.GetAsync("/gtfs/raw/schedule.zip");
            var content = await response.Content.ReadAsStreamAsync();
            string line;

            using (ZipArchive za = new ZipArchive(content, ZipArchiveMode.Read))
            {
                foreach(ZipArchiveEntry entry in za.Entries)
                {
                    using (StreamReader rdr = new StreamReader(entry.Open()))
                    {
                        switch (Path.GetFileNameWithoutExtension(entry.FullName))
                        {
                            case "agency":
                                rdr.ReadLine();
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.Agencies.Add(line.Split(",").FirstOrDefault().Trim(), new Agency(line.Split(",")));
                                }
                                break;
                            case "calendar":
                                rdr.ReadLine();
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.Calendars.Add(line.Split(",").FirstOrDefault().Trim(), new Calendar(line.Split(",")));
                                }
                                break;
                            case "calendar_dates":
                                rdr.ReadLine();
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.CalendarDates.Add(new CalendarDate(line.Split(",")));
                                }
                                break;
                            case "fare_attributes":
                                rdr.ReadLine();
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.FareAttributes.Add(Convert.ToInt32(line.Split(",").FirstOrDefault().Trim()), new FareAttributes(line.Split(",")));
                                }
                                break;
                            case "fare_rules":
                                rdr.ReadLine();
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.FareRules.Add(new FareRules(line.Split(",")));
                                }
                                break;
                            case "routes":
                                rdr.ReadLine();
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.Routes.Add(line.Split(",").FirstOrDefault().Trim(), new Routes(line.Split(",")));
                                }
                                break;
                            case "shapes":
                                rdr.ReadLine();
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
                                rdr.ReadLine();
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
                                rdr.ReadLine();
                                while (!rdr.EndOfStream)
                                {
                                    line = rdr.ReadLine();
                                    Data.Stops.Add(line.Split(",").FirstOrDefault().Trim(), new Stops(line.Split(",")));
                                }
                                break;
                            case "trips":
                                rdr.ReadLine();
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
