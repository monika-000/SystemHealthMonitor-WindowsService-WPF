using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WindowsService.Interfaces;

namespace SystemHealthMonitor.WindowsService.Services
{
    internal class HealthCheckService : IHealthCheckService
    {
        private readonly MetricsThresholds _thresholds;
        private readonly ISystemMetricsService _systemMetricsService;
        private INotificationPipeClient _notificationPipeClient;
        private readonly ISettingsService _settingsService;

        public HealthCheckService(ISystemMetricsService systemMetricsService, INotificationPipeClient notificationPipeClient, ISettingsService settingsService)
        {
            _settingsService = settingsService;
             _thresholds = settingsService.GetSystemHealthMonitorAppSettings().Thresholds;
            _systemMetricsService = systemMetricsService;
            _notificationPipeClient = notificationPipeClient;
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
            Dictionary<string, decimal> freeDiskSpace = await _systemMetricsService.GetFreeDiskSpacePercentage();
            
            if (!gettignInitialSampleFailed)
            {
                decimal networkTotalUsage = await _systemMetricsService.GetNetworkIOTotalUsagePercentage();
                decimal incomingPacketsLoss = await _systemMetricsService.GetIncomingNetworkPacketsLossPercentage();
                decimal outgoingPacketsLoss = await _systemMetricsService.GetOutgoingNetworkPacketsLossPercentage();

                if (networkTotalUsage > _thresholds.NetworkIOThresholdPercent)
                {
                    //log
                    await _notificationPipeClient.SendAsync("Network I/0", networkTotalUsage, _thresholds.NetworkIOThresholdPercent);
                }
                if (incomingPacketsLoss > _thresholds.IncomingPacketsLossThreshold)
                {
                    //log
                    await _notificationPipeClient.SendAsync("Incoming Packets Loss", incomingPacketsLoss, _thresholds.IncomingPacketsLossThreshold);
                }
                if (outgoingPacketsLoss > _thresholds.OutgoingPacketsLossThreshold)
                {
                    //log
                    await _notificationPipeClient.SendAsync("Outgoing Packets Loss", outgoingPacketsLoss, _thresholds.OutgoingPacketsLossThreshold);
                }
            }
          
            if (cpuUsage > _thresholds.CPUThresholdPercent)
            {
                //log
                await _notificationPipeClient.SendAsync("CPU Usage", cpuUsage, _thresholds.CPUThresholdPercent);
            }
            if (memoryUsage > _thresholds.MemoryThresholdPercent)
            {
                //log
                await _notificationPipeClient.SendAsync("Memory Usage", memoryUsage, _thresholds.MemoryThresholdPercent);
            }
            if (diskIO > _thresholds.DiskIOThresholdPercent)
            {
                //log
                await _notificationPipeClient.SendAsync("Disk I/0", diskIO, _thresholds.DiskIOThresholdPercent);
            }

            IterateThroughDictioanryAndCallPipeClient(freeDiskSpace, "Free Disk Space", _thresholds.FreeDiskSpaceThresholdPercent, _notificationPipeClient);
        }

        private async void IterateThroughDictioanryAndCallPipeClient(Dictionary<string, decimal> metrics, string metricName, decimal treshold, INotificationPipeClient notificationPipeClient)
        {
            foreach (string key in metrics.Keys)
            {
                if (metrics[key] < treshold)
                {
                    //log
                    await notificationPipeClient.SendAsync(metricName, metrics[key], treshold, key);
                }
            }
        }
    }
}
