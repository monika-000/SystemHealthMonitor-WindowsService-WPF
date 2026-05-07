using System;
using System.Collections.Generic;
using System.Text;

namespace SystemHealthMonitor.WindowsService.Interfaces
{
    public interface IHealthCheckService
    {
        Task<int> GetPoolingInterval();
        Task GenerateSystemMetricsSampleAndSendNotifications(bool gettignInitialSampleFailed);
        void CleanUpOldResults();
    }
}
