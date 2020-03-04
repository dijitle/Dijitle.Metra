using Microsoft.Extensions.Hosting;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dijitle.Metra.API.Services
{
    public class DataRefresher : IHostedService, IDisposable
    {
        private Timer _refreshDataTimer;
        private Timer _refreshAlertsTimer;
        private Timer _refreshEnrouteTimer;
        private Timer _refreshGPSTimer;

        private readonly IMetraService _metra;
        private readonly IGTFSService _gtfs;

        private readonly IHttpClientFactory _httpClientFactory;


        private static readonly Counter ETDSCoutner = Metrics.CreateCounter("metra_calls_to_etds", 
            "number of times etds endpoint was called", new CounterConfiguration { LabelNames = new[] { "returnCode" }, SuppressInitialValue = true });

        public DataRefresher(IMetraService metra, IGTFSService gtfs, IHttpClientFactory httpClientFactory)
        {
            _metra = metra;
            _gtfs = gtfs;
            _httpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _refreshDataTimer = new Timer(RefreshData, null, TimeSpan.FromSeconds(15), TimeSpan.FromHours(1));
            _refreshEnrouteTimer = new Timer(RefreshEnroute, null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));
            _refreshAlertsTimer = new Timer(RefreshAlerts, null, TimeSpan.FromSeconds(45), TimeSpan.FromMinutes(15));
            _refreshGPSTimer = new Timer(RefreshGPS, null, TimeSpan.FromSeconds(45), TimeSpan.FromMinutes(5));
            _refreshDataTimer = new Timer(RefreshETDS, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(31));
            return Task.CompletedTask;
        }

        private async void RefreshETDS(object state)
        {
            HttpClient client = _httpClientFactory.CreateClient("Edwin");
            var response = await client.GetAsync("/etds/train_to_shapeid_log.php");
            
            ETDSCoutner.WithLabels(response.StatusCode.ToString()).Inc();
        }
        private async void RefreshData(object state)
        {
            if (_gtfs.Data.IsStale)
            {
                await _gtfs.RefreshData();
            }
        }

        private async void RefreshEnroute(object state)
        {
            await _metra.GetTripsEnroute();
        }

        private async void RefreshAlerts(object state)
        {
            await _gtfs.GetAlerts();
        }

        private async void RefreshGPS(object state)
        {
            await _metra.GetAllEstimatedPositions(true);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _refreshDataTimer?.Change(Timeout.Infinite, 0);
            _refreshEnrouteTimer?.Change(Timeout.Infinite, 0);
            _refreshAlertsTimer?.Change(Timeout.Infinite, 0);
            _refreshGPSTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _refreshDataTimer?.Dispose();
            _refreshEnrouteTimer?.Dispose();
            _refreshAlertsTimer?.Dispose();
            _refreshGPSTimer?.Dispose();
        }
    }
}
