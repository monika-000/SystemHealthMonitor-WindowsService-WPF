using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SystemHealthMonitor.Shared.Models
{
    public class SystemHealthResults
    {
        public string Metric { get; set; }
        public string Value { get; set; }
        
        public DateTimeOffset DateTime { get; set; }

        public SystemHealthResults()
        {

        }
        public SystemHealthResults(string metric, string value)
        {
            Metric = metric;
            Value = value;

        }
        public SystemHealthResults(string metric, string value, DateTimeOffset dateTime)
        {
            Metric = metric;
            Value = value;
            DateTime = dateTime;
        }
    }
}
