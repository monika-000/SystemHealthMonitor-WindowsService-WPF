using System.Net.NetworkInformation;
using System.Timers;
using SystemHealthMonitor.WindowsService.Interfaces;

namespace SystemHealthMonitor.WindowsService
{
    public class SystemHealthMonitorWorker : BackgroundService
    {
        private readonly ILogger<SystemHealthMonitorWorker> _logger;
        public readonly IHealthCheckService _healthCheckService;
        private readonly ISystemMetricsService _systemMetricsService;
        private int _poolingInterval;
        private bool _gettignInitialSampleFailed = false;
        private System.Timers.Timer _cleanUpTimer;
        private bool _pauseGatheringMetrics = false;

        public SystemHealthMonitorWorker(ILogger<SystemHealthMonitorWorker> logger, IHealthCheckService healthCheckService, ISystemMetricsService systemMetricsService)
        {
            _logger = logger;
            _healthCheckService = healthCheckService;
            _systemMetricsService = systemMetricsService;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Windows service started. Getting initial samples for network interface.");

            NetworkInterface activeInterface = _systemMetricsService.PopulateIntialNetworkIOUsageFields();
            _poolingInterval = await _healthCheckService.GetPoolingInterval();

            _logger.LogInformation("Retrieved pooling interval. Pooling interval: {0}s", _poolingInterval);
            _cleanUpTimer = new System.Timers.Timer(TimeSpan.FromDays(1).TotalMilliseconds);
            _cleanUpTimer.Elapsed += OnTimerElapsed;
            _cleanUpTimer.AutoReset = true; 
            _cleanUpTimer.Start(); 

            if (activeInterface == null)
            {
                _gettignInitialSampleFailed = true;
                _logger.LogWarning("Getting initial samples for network interface failed.");
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
                    if (!_pauseGatheringMetrics)
                    {
                        _logger.LogInformation("Gathering sysmet metric");
                        await _healthCheckService.GenerateSystemMetricsSampleAndSendNotifications(_gettignInitialSampleFailed);
                    }
                    
                }
                catch(Exception ex)
                {
                    _logger.LogError("Collecting system metrics and sendihng notifications failed. Exception: {0}", ex);
                }
            }
        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _pauseGatheringMetrics = true;
                _healthCheckService.CleanUpOldResults();
            }
            catch (Exception ex)  
            {
                _logger.LogError("Something went wrong when cleaning up old results. Exception: {0}", ex);
            }
            finally
            {
                _pauseGatheringMetrics = false;
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping windows service.");
            _cleanUpTimer?.Stop(); 
            _cleanUpTimer?.Dispose(); 
        }
    }

}
