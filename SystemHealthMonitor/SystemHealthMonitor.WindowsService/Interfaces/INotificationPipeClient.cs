
namespace SystemHealthMonitor.WindowsService.Interfaces
{
    internal interface INotificationPipeClient
    {
        Task SendAsync(string metricName, decimal value, decimal threshold, string key = "");

    }
}
