using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Interfaces;

namespace SystemHealthMonitor.WPF.Services
{
    internal class NotificationPipeListener : IPipeListener
    {
        public event EventHandler<SystemHealthNotificationMessage>? NotificationReceived;
        private const string _pipeName = "SystemHealthPipe";

        private readonly ILogger<NotificationPipeListener> _logger;


        public NotificationPipeListener(ILogger<NotificationPipeListener> logger)
        {
            _logger = logger;
        }

        //Starts pipe and invokes the event
        public async Task StartAsync()
        {
            while (true)
            {
                try
                {

                    PipeSecurity pipeSecurity = new PipeSecurity();
                    SecurityIdentifier currentUser = WindowsIdentity.GetCurrent().User;
                    if (currentUser != null)
                    {
                        pipeSecurity.AddAccessRule(new PipeAccessRule(currentUser, PipeAccessRights.Write, System.Security.AccessControl.AccessControlType.Allow));
                    }

                    using (NamedPipeServerStream pipeServer = NamedPipeServerStreamAcl.Create(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity))
                    {
                        await pipeServer.WaitForConnectionAsync();

                        using (StreamReader reader = new StreamReader(pipeServer))
                        {
                            string json = await reader.ReadLineAsync();

                            SystemHealthNotificationMessage message = JsonSerializer.Deserialize<SystemHealthNotificationMessage>(json);

                            if (message != null)
                            {
                                NotificationReceived?.Invoke(this, message);
                            }
                        }
                    }
                }
                catch (Exception ex) when (ex is IOException or ObjectDisposedException or OperationCanceledException)
                {
                    // Client disconnected or pipe broke. Only log and keep listening.
                    _logger.LogWarning("Somehting went wrong in notification pipe listener.The error is not critical, so the pipe remains active. Error: {0}", ex);
                    continue; 
                }
                catch (Exception ex)
                {
                    _logger.LogError("Critical error while reading message in notification pipe listener. Error: {0}", ex);
                    throw; //Throw and let the caller handel it.
                    
                }
            }
        }

    }
}
