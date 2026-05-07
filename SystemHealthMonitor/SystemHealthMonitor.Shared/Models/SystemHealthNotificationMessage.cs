namespace SystemHealthMonitor.Shared.Models
{
    public class SystemHealthNotificationMessage
    {
        public string MetricName { get; set; }
        public decimal Value { get; set; }
        public string Message { get; set; }

        public SystemHealthNotificationMessage(string metricName, decimal value, string message)
        {
            MetricName = metricName;
            Value = value;
            Message = message;
        }
    }
}
