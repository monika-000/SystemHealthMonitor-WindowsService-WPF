using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace SystemHealthMonitor.WindowsService.Interfaces
{
    public interface ISystemMetricsService
    {
        Task<decimal> GetCPUUsagePercentage();
        Task<decimal> GetDiskIOPercentage();
        Task<Dictionary<string, decimal>> GetUsedDiskSpacePercentage();
        Task<decimal> GetMemoryUsagePercentage();
        Task<decimal> GetNetworkIOTotalUsagePercentage();
        Task<decimal> GetIncomingNetworkPacketsLossPercentage();
        Task<decimal> GetOutgoingNetworkPacketsLossPercentage();
        NetworkInterface PopulateIntialNetworkIOUsageFields();
    }
}
