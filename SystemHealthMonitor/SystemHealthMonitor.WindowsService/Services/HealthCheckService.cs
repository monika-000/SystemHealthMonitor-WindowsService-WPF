using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WindowsService.Interfaces;

namespace SystemHealthMonitor.WindowsService.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly ILogger<HealthCheckService> _logger;
        private readonly MetricsThresholds _thresholds;
        private readonly ISystemMetricsService _systemMetricsService;
        private INotificationPipeClient _notificationPipeClient;
        private readonly ISettingsService _settingsService;
        private readonly IResultsService _resultsService;

        public HealthCheckService(ILogger<HealthCheckService> logger, ISystemMetricsService systemMetricsService, INotificationPipeClient notificationPipeClient, ISettingsService settingsService, IResultsService resultsService)
        {
            _logger = logger;
            _settingsService = settingsService;
            _thresholds = settingsService.GetSystemHealthMonitorAppSettings().Thresholds;
            _systemMetricsService = systemMetricsService;
            _notificationPipeClient = notificationPipeClient;
            _resultsService = resultsService;
        }
        public async Task<int> GetPoolingInterval()
        {
            return _thresholds.PoolingIntervalSeconds * 1000;
        }
        public async Task GenerateSystemMetricsSampleAndSendNotifications(bool gettignInitialSampleFailed)
        {
            decimal cpuUsage = await _systemMetricsService.GetCPUUsagePercentage();
            decimal memoryUsage = await _systemMetricsService.GetMemoryUsagePercentage();
            decimal diskIO = await _systemMetricsService.GetDiskIOPercentage();
            Dictionary<string, decimal> UsedDiskSpace = await _systemMetricsService.GetUsedDiskSpacePercentage();
            decimal networkTotalUsage = 0m;
            decimal incomingPacketsLoss = 0m;
            decimal outgoingPacketsLoss = 0m;

            if (!gettignInitialSampleFailed)
            {
                networkTotalUsage = await _systemMetricsService.GetNetworkIOTotalUsagePercentage();
                incomingPacketsLoss = await _systemMetricsService.GetIncomingNetworkPacketsLossPercentage();
                outgoingPacketsLoss = await _systemMetricsService.GetOutgoingNetworkPacketsLossPercentage();

                if (networkTotalUsage > _thresholds.NetworkIOThresholdPercent)
                {
                    _logger.LogInformation("Threshold exceeded for network I/O. Sending notification.");
                    await _notificationPipeClient.SendAsync("Network I/0", networkTotalUsage, _thresholds.NetworkIOThresholdPercent);
                }
                if (incomingPacketsLoss > _thresholds.IncomingPacketsLossThreshold)
                {
                    _logger.LogInformation("Threshold exceeded for incoming packet loss. Sending notification.");
                    await _notificationPipeClient.SendAsync("Incoming Packets Loss", incomingPacketsLoss, _thresholds.IncomingPacketsLossThreshold);
                }
                if (outgoingPacketsLoss > _thresholds.OutgoingPacketsLossThreshold)
                {
                    _logger.LogInformation("Threshold exceeded for outgoing packet loss. Sending notification.");
                    await _notificationPipeClient.SendAsync("Outgoing Packets Loss", outgoingPacketsLoss, _thresholds.OutgoingPacketsLossThreshold);
                }
            }
          
            if (cpuUsage > _thresholds.CPUThresholdPercent)
            {
                _logger.LogInformation("Threshold exceeded for CPU usage. Sending notification.");
                await _notificationPipeClient.SendAsync("CPU Usage", cpuUsage, _thresholds.CPUThresholdPercent);
            }
            if (memoryUsage > _thresholds.MemoryThresholdPercent)
            {
                _logger.LogInformation("Threshold exceeded for memory usage. Sending notification.");
                await _notificationPipeClient.SendAsync("Memory Usage", memoryUsage, _thresholds.MemoryThresholdPercent);
            }
            if (diskIO > _thresholds.DiskIOThresholdPercent)
            {
                _logger.LogInformation("Threshold exceeded for disk I/O. Sending notification.");
                await _notificationPipeClient.SendAsync("Disk I/0", diskIO, _thresholds.DiskIOThresholdPercent);
            }
           

            IterateThroughDictioanryAndCallPipeClient(UsedDiskSpace, "Used Disk Space", _thresholds.UsedDiskSpaceThresholdPercent, _notificationPipeClient);


            List<SystemHealthResults> systemHealthResults = CreateSystemHealthResultsList(cpuUsage, memoryUsage, diskIO, UsedDiskSpace, networkTotalUsage, incomingPacketsLoss, outgoingPacketsLoss);
            _resultsService.SaveLatestResults(systemHealthResults);
        }


        private async void IterateThroughDictioanryAndCallPipeClient(Dictionary<string, decimal> metrics, string metricName, decimal treshold, INotificationPipeClient notificationPipeClient)
        {
            foreach (string key in metrics.Keys)
            {
                if (metrics[key] < treshold)
                {
                    _logger.LogInformation("Threshold exceeded for disk space. Sending notification.");
                    await notificationPipeClient.SendAsync($"Disk {metricName}", metrics[key], treshold, key);
                }
            }
        }

        public List<SystemHealthResults> CreateSystemHealthResultsList(decimal cpuUsage, decimal memoryUsage, decimal diskIO, Dictionary<string, decimal> usedDiskSpace, decimal networkTotalUsage, decimal incomingPacketsLoss, decimal outgoingPacketsLoss)
        {
            DateTimeOffset timestamp = DateTimeOffset.Now;
            List<SystemHealthResults> systemHealthResults = new List<SystemHealthResults>()
            {
                new SystemHealthResults("CPU Usage", cpuUsage.ToString(), timestamp), 
                new SystemHealthResults("Memory Usage", memoryUsage.ToString(), timestamp),
                new SystemHealthResults("Disk I/O", diskIO.ToString(), timestamp),
                new SystemHealthResults("Network I/O", networkTotalUsage.ToString(), timestamp),
                new SystemHealthResults("Incoming Packets Loss", incomingPacketsLoss.ToString(), timestamp),
                new SystemHealthResults("Outgoing Packets Loss", outgoingPacketsLoss.ToString(), timestamp)

            };

            foreach (string key in usedDiskSpace.Keys)
            {
                systemHealthResults.Add
                (
                   new SystemHealthResults($"Drive {key}", usedDiskSpace[key].ToString())
                );
            }

            return systemHealthResults;
        }
   
        public void CleanUpOldResults()
        {
            _resultsService.CleanUpOldResults();
        }
    }
}
