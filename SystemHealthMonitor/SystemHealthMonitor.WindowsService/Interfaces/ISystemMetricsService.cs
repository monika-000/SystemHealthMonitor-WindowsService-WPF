using System.Net.NetworkInformation;

namespace SystemHealthMonitor.WindowsService.Interfaces
{
    internal interface ISystemMetricsService
    {
        Task<decimal> GetCPUUsagePercentage();
        Task<decimal> GetDiskIOPercentage();
        Task<Dictionary<string, decimal>> GetFreeDiskSpacePercentage();
        Task<decimal> GetMemoryUsagePercentage();
        Task<decimal> GetNetworkIOTotalUsagePercentage();
        Task<decimal> GetIncomingNetworkPacketsLossPercentage();
        Task<decimal> GetOutgoingNetworkPacketsLossPercentage();
        NetworkInterface PopulateIntialNetworkIOUsageFields();
    }
}
