using Serilog;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Services;
using SystemHealthMonitor.WindowsService.Interfaces;
using SystemHealthMonitor.WindowsService.Services;

namespace SystemHealthMonitor.WindowsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MPCorp", "SystemHealthMonitorApp", "logs");
            LoggingService loggingService = new LoggingService();
            loggingService.Configure(logsPath);

            var builder = Host.CreateApplicationBuilder(args);

            // 1. Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // 2. Host configuration
            builder.Services.AddWindowsService();

            // 3. Application services (DI registrations)
            builder.Services.AddTransient<INotificationPipeClient, NotificationPipeClient>();
            builder.Services.AddSingleton<ISystemMetricsService, SystemMetricsService>();
            builder.Services.AddSingleton<IHealthCheckService, HealthCheckService>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IResultsService, ResultsService>();

            // 4. Hosted services (workers)
            builder.Services.AddHostedService<SystemHealthMonitorWorker>();

            var app = builder.Build();
            app.Run();
        }
    }
}