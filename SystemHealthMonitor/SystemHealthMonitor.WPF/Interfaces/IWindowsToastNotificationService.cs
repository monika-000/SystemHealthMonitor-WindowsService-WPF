using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Services;

namespace SystemHealthMonitor.WPF.Interfaces
{
    internal interface IWindowsToastNotificationService
    {
        void ShowWindowsToastNotification(SystemHealthNotificationMessage message);
        void HandleToastNotificationClicks(ToastNotificationActivatedEventArgsCompat toastArgs, WindowsToastNotificationService windowsToastNotificationService);
        void ClearAllNotifications();
    }
}
