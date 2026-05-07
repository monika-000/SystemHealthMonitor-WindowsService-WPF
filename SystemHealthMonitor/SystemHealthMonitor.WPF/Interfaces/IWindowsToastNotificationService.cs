using Microsoft.Toolkit.Uwp.Notifications;
using System.Windows.Navigation;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Services;

namespace SystemHealthMonitor.WPF.Interfaces
{
    public interface IWindowsToastNotificationService
    {
        void ShowWindowsToastNotification(SystemHealthNotificationMessage message);
    }
}
