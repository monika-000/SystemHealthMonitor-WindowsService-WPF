
namespace SystemHealthMonitor.WindowsService.Interfaces
{
    internal interface IHealthCheckService
    {
        Task<int> GetPoolingInterval();
        Task GenerateSystemMetricsSampleAndSendNotifications(bool gettignInitialSampleFailed);
    }
}
