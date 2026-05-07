using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Interfaces;
using SystemHealthMonitor.WPF.Views;

namespace SystemHealthMonitor.WPF.Services
{
    public class WindowsToastNotificationService : IWindowsToastNotificationService
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
                    .SetBackgroundActivation()             
               )
               .AddButton(new ToastButton()
                    .SetContent("Dismiss")
                    .AddArgument("action", "dismissButton")
                )
               .Show();

        }  
    }
}
