using System.ComponentModel;


namespace SystemHealthMonitor.Shared.Models
{
    public class MetricsThresholds
    {
        [DisplayName("Sample Interval")]
        [Description("Time interval (in seconds) that determines how frequently metrics are collected.")]
        public int PoolingIntervalSeconds { get; set; }
        [DisplayName("CPU")]
        [Description("CPU usage threshold (in percentage) after which the system will raise an alarm.")]
        public int CPUThresholdPercent { get; set; }
        [DisplayName("Memory")]
        [Description("Memory usage threshold (in percentage) after which the system will raise an alarm.")]
        public int MemoryThresholdPercent { get; set; }
        [DisplayName("Used Disk Space")]
        [Description("Used disk space threshold (in percentage) after which the system will raise an alarm.")]
        public int UsedDiskSpaceThresholdPercent { get; set; }
        [DisplayName("Disk I/O")]
        [Description("Disk I/O threshold (in percentage) after which the system will raise an alarm.")]
        public int DiskIOThresholdPercent { get; set; }
        [DisplayName("Network I/O")]
        [Description("Network I/O threshold (in percentage) after which the system will raise an alarm.")]
        public int NetworkIOThresholdPercent { get; set; }
        [DisplayName("Incoming Packets Loss")]
        [Description("Incoming Packets Loss threshold (in percentage) after which the system will raise an alarm.")]
        public decimal IncomingPacketsLossThreshold { get; set; }
        [DisplayName("Outgoing Packets Loss")]
        [Description("Outgoing Packets Loss threshold (in percentage) after which the system will raise an alarm.")]
        public decimal OutgoingPacketsLossThreshold { get; set; }
      
    }
}
