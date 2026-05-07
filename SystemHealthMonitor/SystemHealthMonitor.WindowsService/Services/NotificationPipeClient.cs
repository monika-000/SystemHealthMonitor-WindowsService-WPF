using System.IO.Pipes;
using System.Text.Json;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WindowsService.Interfaces;


namespace SystemHealthMonitor.WindowsService.Services
{
    internal class NotificationPipeClient : INotificationPipeClient
    {
        private const string _pipeName = "SystemHealthPipe";
        private readonly ILogger<NotificationPipeClient> _logger;


        public NotificationPipeClient(ILogger<NotificationPipeClient> logger) 
        {
            _logger = logger;
        }
        
        public async Task SendAsync(string metricName, decimal value, decimal threshold, string key = "")
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
                {
                    await pipeClient.ConnectAsync();
                    decimal difference = value - threshold;
                    string message = key == "" ? $"{metricName} threshold exceeded by {difference}%" : $"{key}. {metricName} threshold exceeded by {difference}%";

                    using (StreamWriter sw = new StreamWriter(pipeClient))
                    {
                        SystemHealthNotificationMessage systemHealthNotificationMessage = new SystemHealthNotificationMessage(metricName, value, message);
                        string json = JsonSerializer.Serialize(systemHealthNotificationMessage);
                        await sw.WriteLineAsync(json);
                        await sw.FlushAsync();
                    }
                }
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException or TimeoutException or EndOfStreamException)
            {
                _logger.LogWarning("Somehting went wrong while sending message in notification pipe client.The error is not critical. Error: {0}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while sending message in notification pipe client. Error: {0}", ex); //Log error, the next pooling cycle will restart the pipe.
            }

        }
    }
}
