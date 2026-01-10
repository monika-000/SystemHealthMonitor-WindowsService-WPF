using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text.Json;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Interfaces;

namespace SystemHealthMonitor.WPF.Services
{
    internal class NotificationPipeListener : INotificationPipeListener
    {
        public event EventHandler<SystemHealthNotificationMessage>? NotificationReceived;
        private const string _pipeName = "SystemHealthPipe";

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
                    continue; // Client disconnected or pipe broke
                }
                catch (Exception ex)
                {
                    //Log
                }
            }
        }

    }
}
