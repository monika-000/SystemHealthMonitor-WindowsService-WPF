using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Services;

namespace SystemHealthMonitor.WPF.ViewModels
{
    internal class NotificationViewModel
    {
        private readonly NotificationPipeListener _listener;
        private readonly WindowsToastNotificationService _windowsToastNotificationService;

        public NotificationViewModel(NotificationPipeListener listener, WindowsToastNotificationService windowsToastNotificationService)
        {
            _listener = listener;
            _windowsToastNotificationService = windowsToastNotificationService;
            _listener.NotificationReceived += OnNotificationReceived;
        }

        private void OnNotificationReceived(object sender, SystemHealthNotificationMessage message)
        {
            _windowsToastNotificationService.ShowWindowsToastNotification(message);
        }
      
       
    }
}
