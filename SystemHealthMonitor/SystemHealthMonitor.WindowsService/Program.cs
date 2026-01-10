
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.Shared.Services;
using SystemHealthMonitor.WindowsService.Interfaces;
using SystemHealthMonitor.WindowsService.Services;

namespace SystemHealthMonitor.WindowsService
{
    public class Program
    {
        public static void Main(string[] args)
        { 
            var builder = Host.CreateApplicationBuilder(args);

            // 1. Host configuration
            builder.Services.AddWindowsService();

            // 2. Application services (DI registrations)
            builder.Services.AddTransient<INotificationPipeClient, NotificationPipeClient>();
            builder.Services.AddSingleton<ISystemMetricsService, SystemMetricsService>();
            builder.Services.AddSingleton<IHealthCheckService, HealthCheckService>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();

            // 4. Hosted services (workers)
            builder.Services.AddHostedService<SystemHealthMonitorWorker>();

            var app = builder.Build();
            app.Run();
        }
    }
}