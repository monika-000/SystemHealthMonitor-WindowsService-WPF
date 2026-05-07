using System.Diagnostics;
using System.Net.NetworkInformation;
using SystemHealthMonitor.WindowsService.Interfaces;

namespace SystemHealthMonitor.WindowsService.Services
{
    public class SystemMetricsService : ISystemMetricsService
    {
        private readonly PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private readonly PerformanceCounter _diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
        private readonly PerformanceCounter _memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        private static long _prevBytesSent;  
        private static long _prevBytesReceived;
        private static long _prevIncomingPacketsErrors;
        private static long _prevOutgoingPacketsErrors;
        private static long _prevIncomingPacketsDiscards;
        private static long _prevOutgoingPacketsDiscards;
        private static long _prevTotalSentPackets;
        private static long _prevTotalReceivedPackets;

        public async Task<decimal> GetCPUUsagePercentage()
        {
            _cpuCounter.NextValue();
            Thread.Sleep(1000);
            float value = _cpuCounter.NextValue();
            decimal cpuUsage = (decimal)value;
            return Math.Round(cpuUsage, 2);
        }
        public async Task<decimal> GetDiskIOPercentage()
        {       
            _diskCounter.NextValue();
            Thread.Sleep(1000);
            float value = _diskCounter.NextValue();
            decimal diskIO = (decimal)value;
            return Math.Round(diskIO, 2);
        }

        public async Task<Dictionary<string, decimal>> GetUsedDiskSpacePercentage()
        {
            Dictionary<string, decimal> diskSpaceMetrics = new Dictionary<string, decimal>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady)
                {
                    long totalSize = drive.TotalSize;
                    long usedSpace = totalSize - drive.AvailableFreeSpace;
                    decimal percentageOfUsedFreeSpace = Math.Ceiling(((decimal)usedSpace / totalSize * 100));
                    diskSpaceMetrics.Add($"Drive {drive.Name}", percentageOfUsedFreeSpace);
                }
            }
            return diskSpaceMetrics;
        }

        public async Task<decimal> GetMemoryUsagePercentage()
        {
            _memoryCounter.NextValue();
            Thread.Sleep(1000);
            float value = _memoryCounter.NextValue();
            decimal memoryUsage = (decimal)value;
            return Math.Round(memoryUsage, 2);
        }

        public NetworkInterface PopulateIntialNetworkIOUsageFields()
        {
            NetworkInterface activeInterface = GetPrimaryNetworkInterface();

            if (activeInterface != null)
            {
                IPv4InterfaceStatistics stats = activeInterface.GetIPv4Statistics();
                _prevBytesSent = stats.BytesSent;
                _prevBytesReceived = stats.BytesReceived;
                _prevOutgoingPacketsErrors = stats.OutgoingPacketsWithErrors;
                _prevIncomingPacketsDiscards = stats.OutgoingPacketsDiscarded;
                _prevIncomingPacketsErrors = stats.IncomingPacketsWithErrors;
                _prevIncomingPacketsDiscards = stats.IncomingPacketsDiscarded;
                _prevTotalReceivedPackets = stats.UnicastPacketsReceived + stats.NonUnicastPacketsReceived;
                _prevTotalSentPackets = stats.UnicastPacketsSent + stats.NonUnicastPacketsSent;
            }

            return activeInterface;
        }

        public async Task<decimal> GetNetworkIOTotalUsagePercentage()
        {
            decimal usagePercent = 0.0m;
          
            NetworkInterface activeInterface = GetPrimaryNetworkInterface();
                
            if (activeInterface != null)
            {
                IPv4InterfaceStatistics stats = activeInterface.GetIPv4Statistics();
                long bytesSent = stats.BytesSent;
                long bytesReceived = stats.BytesReceived;
                long totalCapacityBytesPerSecond = activeInterface.Speed / 8; //convert to bytes
                long bytesTotal = bytesSent + bytesReceived;
                long previousBytesTotal = _prevBytesReceived + _prevBytesSent;
                long actualBytesTotal = Math.Abs(bytesTotal - previousBytesTotal);

                _prevBytesReceived = bytesReceived;
                _prevBytesSent = bytesSent;

                usagePercent = Math.Round((decimal)actualBytesTotal / totalCapacityBytesPerSecond * 100, 2);

            }
            return usagePercent;
          
        }

      
        public async Task<decimal> GetIncomingNetworkPacketsLossPercentage()
        {
            decimal incomingPacketsLossPercent = 0.0m;
           
            NetworkInterface activeInterface = GetPrimaryNetworkInterface();

            if (activeInterface != null)
            {
                long incomingPacketsWithErrors = 0;
                long incomingPacketsDiscarded = 0;
                long totalPacketsReceived = 0;

                IPv4InterfaceStatistics stats = activeInterface.GetIPv4Statistics();
                incomingPacketsWithErrors = stats.IncomingPacketsWithErrors;
                incomingPacketsDiscarded = stats.IncomingPacketsDiscarded;
                totalPacketsReceived = stats.UnicastPacketsReceived + stats.NonUnicastPacketsReceived;


                long deltaIncomingWithErrors = incomingPacketsWithErrors - _prevIncomingPacketsErrors;
                long deltaIncomingDiscarded = incomingPacketsDiscarded - _prevIncomingPacketsDiscards;
                long deltaTotalReceived = totalPacketsReceived - _prevTotalReceivedPackets;
                long deltaTotalIncomingErrorsAndDiscards = deltaIncomingWithErrors + deltaIncomingDiscarded;
                incomingPacketsLossPercent = ((decimal)deltaTotalIncomingErrorsAndDiscards / totalPacketsReceived) * 100;

                _prevIncomingPacketsErrors = incomingPacketsWithErrors;
                _prevIncomingPacketsDiscards = incomingPacketsDiscarded;
                _prevTotalReceivedPackets = totalPacketsReceived;
            }

            return Math.Round(incomingPacketsLossPercent,2);
        }

        public async Task<decimal> GetOutgoingNetworkPacketsLossPercentage()
        {
            decimal outgoingPacketsLossPercent = 0.0m;
           
            NetworkInterface activeInterface = GetPrimaryNetworkInterface();

            long outgoingPacketsWithErrors = 0;
            long outgoingPacketsDiscarded = 0;
            long totalPacketsSent = 0;

            if (activeInterface != null)
            {
                IPv4InterfaceStatistics stats = activeInterface.GetIPv4Statistics();
                outgoingPacketsWithErrors += stats.OutgoingPacketsWithErrors;
                outgoingPacketsDiscarded += stats.OutgoingPacketsDiscarded;
                totalPacketsSent += stats.UnicastPacketsSent + stats.NonUnicastPacketsSent;


                long deltaOutgoingWithErrors = outgoingPacketsWithErrors - _prevOutgoingPacketsErrors;
                long deltaOutgoingPacketsDiscarded = outgoingPacketsDiscarded - _prevOutgoingPacketsDiscards;
                long deltaTotalSent = totalPacketsSent - _prevTotalSentPackets;
                long deltaTotalOutgoingErrorsAndDiscards = deltaOutgoingWithErrors + deltaOutgoingPacketsDiscarded;

                outgoingPacketsLossPercent = ((decimal)deltaTotalOutgoingErrorsAndDiscards / totalPacketsSent) * 100;

                _prevOutgoingPacketsErrors = outgoingPacketsWithErrors;
                _prevOutgoingPacketsDiscards = outgoingPacketsDiscarded;
                _prevTotalSentPackets = totalPacketsSent;
            }

            return Math.Round(outgoingPacketsLossPercent, 2);
        }

        private static NetworkInterface GetPrimaryNetworkInterface()
        {
            NetworkInterface activeInterface = null;
          
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            //Get the primary network interface
            activeInterface = interfaces.Where(i => i.OperationalStatus == OperationalStatus.Up &&
            (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet || i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
            i.GetIPProperties().GatewayAddresses.Count > 0
                ).OrderBy(i => i.GetIPProperties().GetIPv4Properties()?.Index ?? int.MaxValue).FirstOrDefault();

            return activeInterface;
         }
    }
}
