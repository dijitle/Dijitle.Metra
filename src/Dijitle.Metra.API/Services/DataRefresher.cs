﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public DataRefresher(IMetraService metra, IGTFSService gtfs)
        {
            _metra = metra;
            _gtfs = gtfs;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _refreshDataTimer = new Timer(RefreshData, null, TimeSpan.FromSeconds(5), TimeSpan.FromHours(1));
            _refreshEnrouteTimer = new Timer(RefreshEnroute, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1));
            _refreshAlertsTimer = new Timer(RefreshAlerts, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(15));
            _refreshGPSTimer = new Timer(RefreshGPS, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
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
            await _gtfs.GetPositions();
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