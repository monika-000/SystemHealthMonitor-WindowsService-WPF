using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Interfaces;
using SystemHealthMonitor.WPF.View;

namespace SystemHealthMonitor.WPF.Services
{
    internal class WindowsToastNotificationService : IWindowsToastNotificationService
    {
        public void ShowWindowsToastNotification(SystemHealthNotificationMessage message) 
        {
            string imagePath = System.IO.Path.GetFullPath("Icons/SystemHealthMonitor.ico");
            new ToastContentBuilder()
               .AddArgument("action", "viewNotification")
               .AddText($"System Health Monitor: {message.MetricName}")
               .AddText(message.Message)
               .AddText($"Value: {message.Value}")
               .AddButton(new ToastButton()
                    .SetContent("View")
                    .AddArgument("action", "viewButton")
               )
               .AddButton(new ToastButton()
                    .SetContent("Dismiss")
                    .AddArgument("action", "dismissButton")
                )
               .Show();
        }

        public void HandleToastNotificationClicks(ToastNotificationActivatedEventArgsCompat toastArgs, WindowsToastNotificationService windowsToastNotificationService)
        {
            if(toastArgs.Argument == "action=viewButton")
            {
                //ShowResultsWindow();
            }
            
            windowsToastNotificationService.ClearAllNotifications();
        }

        public void ClearAllNotifications()
        {

        }
    }
}
