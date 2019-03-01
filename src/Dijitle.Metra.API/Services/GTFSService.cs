using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dijitle.Metra.API.Services
{
    public class GTFSService : IGTFSService
    {
        public async Task<object> GetPositions()
        {
            var byteArray = Encoding.ASCII.GetBytes("");
           

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("https://gtfsapi.metrarail.com")
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.GetAsync("/gtfs/positions");
            var content = await response.Content.ReadAsStringAsync();

            var ret = JsonConvert.DeserializeObject(content);

            return ret;
        }
    }
}
