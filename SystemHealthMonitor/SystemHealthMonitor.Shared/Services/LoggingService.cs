using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using SystemHealthMonitor.Shared.Interfaces;

namespace SystemHealthMonitor.Shared.Services
{
    public class LoggingService : ILoggingService
    {

        public  void Configure(string basePath)
        {
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File
                (
                    Path.Combine(basePath, "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14
                )
                .CreateLogger();
        }
    }
     
}
