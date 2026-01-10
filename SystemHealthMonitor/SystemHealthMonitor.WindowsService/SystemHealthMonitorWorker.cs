using Microsoft.Extensions.Configuration;
using System.Net.NetworkInformation;
using SystemHealthMonitor.WindowsService.Interfaces;
using SystemHealthMonitor.WindowsService.Services;

namespace SystemHealthMonitor.WindowsService
{
    internal class SystemHealthMonitorWorker : BackgroundService
    {
        private readonly ILogger<SystemHealthMonitorWorker> _logger;
        public readonly IHealthCheckService _healthCheckService;
        private readonly ISystemMetricsService _systemMetricsService;
        private int _poolingInterval;
        private bool _gettignInitialSampleFailed = false;

        public SystemHealthMonitorWorker(ILogger<SystemHealthMonitorWorker> logger, IHealthCheckService healthCheckService, ISystemMetricsService systemMetricsService)
        {
            _logger = logger;
            _healthCheckService = healthCheckService;
            _systemMetricsService = systemMetricsService;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            NetworkInterface activeInterface = _systemMetricsService.PopulateIntialNetworkIOUsageFields();
            _poolingInterval = await _healthCheckService.GetPoolingInterval();
            if (activeInterface == null)
            {
                _gettignInitialSampleFailed = true;
                //log
            }

            await base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_poolingInterval, stoppingToken);

                try
                {
                    await _healthCheckService.GenerateSystemMetricsSampleAndSendNotifications(_gettignInitialSampleFailed);
                }
                catch(Exception ex)
                {
                    //log
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
            }
        }
    }

}
