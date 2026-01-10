using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using SystemHealthMonitor.WindowsService.Interfaces;

namespace SystemHealthMonitor.WindowsService.Services
{
    internal class SystemMetricsService : ISystemMetricsService
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
            decimal cpuUsage = 0.0m;
            try
            {
                _cpuCounter.NextValue();

                Thread.Sleep(1000);
                float value = _cpuCounter.NextValue();
                cpuUsage = (decimal)value;
                return cpuUsage;
            }
            catch (Exception ex)
            {
                //log
                return cpuUsage;
            }

        }
        public async Task<decimal> GetDiskIOPercentage()
        {
            decimal diskIO = 0.00m;
            try
            {
                _diskCounter.NextValue();

                Thread.Sleep(1000);
                float value = _diskCounter.NextValue();
                diskIO = (decimal)value;
                return diskIO;
            }
            catch (Exception ex)
            {
                //log
                return diskIO;
            }
        }

        public async Task<Dictionary<string, decimal>> GetFreeDiskSpacePercentage()
        {
            Dictionary<string, decimal> diskSpaceMetrics = new Dictionary<string, decimal>();
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    if (drive.IsReady)
                    {
                        long availableFreeSpace = drive.AvailableFreeSpace;
                        long totalSize = drive.TotalSize;
                        decimal percentageOfAvailableFreeSpace = Math.Ceiling(((decimal)availableFreeSpace / totalSize * 100));
                        diskSpaceMetrics.Add($"Drive {drive.Name}", percentageOfAvailableFreeSpace);
                    }
                }
                return diskSpaceMetrics;
            }
            catch (Exception ex)
            {
                //log
                return diskSpaceMetrics;
            }
        }

        public async Task<decimal> GetMemoryUsagePercentage()
        {
            decimal memoryUsage = 0.0m;
            try
            {
                _memoryCounter.NextValue();

                Thread.Sleep(1000);
                float value = _memoryCounter.NextValue();
                memoryUsage = (decimal)value;
                return memoryUsage;
            }
            catch (Exception ex)
            {
                //log
                return memoryUsage;
            }
        }

        public NetworkInterface PopulateIntialNetworkIOUsageFields()
        {
            try
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
            catch (Exception ex)
            {
                //log
                return null;
            }

        }

        public async Task<decimal> GetNetworkIOTotalUsagePercentage()
        {
            decimal usagePercent = 0.0m;
            try
            {
                NetworkInterface activeInterface = GetPrimaryNetworkInterface();
                
                if (activeInterface != null)
                {
                    IPv4InterfaceStatistics stats = activeInterface.GetIPv4Statistics();
                    long bytesSent = stats.BytesSent;
                    long bytesReceived = stats.BytesReceived;
                    long totalCapacityBytesPerSecond = activeInterface.Speed / 8;
                    long bytesTotal = bytesSent + bytesReceived;
                    long previousBytesTotal = _prevBytesReceived + _prevBytesSent;
                    long actualBytesTotal = Math.Abs(bytesTotal - previousBytesTotal);

                    _prevBytesReceived = bytesReceived;
                    _prevBytesSent = bytesSent;

                    usagePercent = ((decimal)actualBytesTotal / totalCapacityBytesPerSecond) * 100;

                }
                return usagePercent;
            }
            catch (Exception ex)
            {
                //log
                return 0.00m;
            }

        }

        public async Task<decimal> GetIncomingNetworkPacketsLossPercentage()
        {
            decimal incomingPacketsLossPercent = 0.0m;
            try
            {
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

                return incomingPacketsLossPercent;
            }
            catch (Exception ex)
            {
                //log
                return incomingPacketsLossPercent;
            }

        }
        public async Task<decimal> GetOutgoingNetworkPacketsLossPercentage()
        {
            decimal outgoingPacketsLossPercent = 0.0m;
            try
            {
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

                return outgoingPacketsLossPercent;
            }
            catch (Exception ex)
            {
                //log
                return outgoingPacketsLossPercent;
            }
        }

        private static NetworkInterface? GetPrimaryNetworkInterface()
        {
            NetworkInterface? activeInterface = null;
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

                //Get the primary network interface
                activeInterface = interfaces.Where(i => i.OperationalStatus == OperationalStatus.Up &&
                (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet || i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                i.GetIPProperties().GatewayAddresses.Count > 0
                 ).OrderBy(i => i.GetIPProperties().GetIPv4Properties()?.Index ?? int.MaxValue).FirstOrDefault();

                return activeInterface;
            }
            catch (Exception ex)
            {
                //log
                return activeInterface;
            }
         }
    }
}
