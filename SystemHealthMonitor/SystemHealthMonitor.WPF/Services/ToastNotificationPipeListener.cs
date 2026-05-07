using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Pipes;
using System.Printing;
using System.Security.AccessControl;
using System.Security.Principal;
using SystemHealthMonitor.WPF.Interfaces;
using SystemHealthMonitor.WPF.ViewModels;
using SystemHealthMonitor.WPF.Views;

namespace SystemHealthMonitor.WPF.Services
{
    internal class ToastNotificationPipeListener : IToastNotificationPipeListener 
    {
        public event Action<string> MessageReceived;
        private readonly string _pipeName = "ToastNotificationPipe";
        private bool _running;
        private readonly IHealthMonitorNavigationService _navigationService;
        private readonly ILogger<ToastNotificationPipeListener> _logger;
        public ToastNotificationPipeListener(IHealthMonitorNavigationService navigationService, ILogger<ToastNotificationPipeListener> logger)
        {
            _navigationService = navigationService;  
            _logger = logger;
        }
        public async Task StartAsync()
        {
            while (true)
            {
                try
                {
                    /* Allow any logged-in user to write to this named pipe. WPF app and Toast Notification
                     * will run under different identities.
                     * It will prevent system accounts access, and remote and annonymous access */
                    PipeSecurity pipeSecurity = new PipeSecurity();
                    pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
                    
                    using (NamedPipeServerStream pipeServer = NamedPipeServerStreamAcl.Create(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity))
                    {
                        await pipeServer.WaitForConnectionAsync();
                        using (var reader = new StreamReader(pipeServer))
                        {
                            string message = await reader.ReadToEndAsync();
                            MessageReceived?.Invoke(message.Trim());
                        }
                    }
                }
                catch (Exception ex) when (ex is IOException or ObjectDisposedException or OperationCanceledException)
                {
                    // Client disconnected or pipe broke. Only log and keep listening.
                    _logger.LogWarning("Somehting went wrong in toast notification pipe listener.The error is not critical, so the pipe remains active. Error: {0}", ex);
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Critical error while reading message in toast notification pipe listener. Error: {0}", ex);
                    throw; //Throw and let the caller handel it.
                }
            }
        }
            
        public void OnExternalCommand(string message)
        {
            if (message == "action=viewButton")
            {
                _navigationService.NavigateTo<ResultsView, ResultsViewModel>();
                _navigationService.BringWindowToFront();
            }   
        }
    }
}
