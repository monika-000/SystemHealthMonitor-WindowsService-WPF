using System;
using System.Collections.Generic;
using System.Text;

namespace SystemHealthMonitor.WindowsService.Interfaces
{
    public interface INotificationPipeClient
    {
         Task SendAsync(string metricName, decimal value, decimal threshold, string key = "");

    }
}
