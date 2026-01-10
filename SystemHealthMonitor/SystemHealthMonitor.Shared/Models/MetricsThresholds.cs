
namespace SystemHealthMonitor.Shared.Models
{
    public class MetricsThresholds
    {
        public int PoolingIntervalSeconds { get; set; } = 5;
        public int CPUThresholdPercent { get; set; } = 80;
        public int MemoryThresholdPercent { get; set; } = 70;
        public int FreeDiskSpaceThresholdPercent { get; set; } = 20;
        public int DiskIOThresholdPercent { get; set; } = 60;
        public int NetworkIOThresholdPercent { get; set; } = 50;
        public decimal IncomingPacketsLossThreshold { get; set; } = 0.1m;
        public decimal OutgoingPacketsLossThreshold { get; set; } = 0.05m;
      
    }
}
