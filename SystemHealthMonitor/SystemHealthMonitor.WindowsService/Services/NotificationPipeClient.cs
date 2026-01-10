using System.IO.Pipes;
using System.Text.Json;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WindowsService.Interfaces;


namespace SystemHealthMonitor.WindowsService.Services
{
    internal class NotificationPipeClient : INotificationPipeClient
    {
        private const string _pipeName = "SystemHealthPipe";
        public async Task SendAsync(string metricName, decimal value, decimal threshold, string key = "")
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
                {
                    pipeClient.Connect();
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
                //log: Client disconnected or pipe broke
            }
            catch (Exception ex)
            {
                //log
                throw;
            }
        }
    }
}
