using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;

namespace SystemHealthMonitor.Shared.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ILogger<SettingsService> _logger;
        private readonly string _settingsPath;
        private readonly string _directoryPath;
        public SettingsService(ILogger<SettingsService> logger)
        {
            _logger = logger;
            _directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MPCorp", "SystemHealthMonitorApp");
            _settingsPath = Path.Combine(_directoryPath, "settings.json");
        }

        public SystemHealthMonitorAppSettings GetSystemHealthMonitorAppSettings()
        {
            EnsureDirecoryAndFileExists();

            var json = File.ReadAllText(_settingsPath);
            SystemHealthMonitorAppSettings settigns = JsonSerializer.Deserialize<SystemHealthMonitorAppSettings>(json);

            _logger.LogInformation("App settings succefully retrieved");

            return settigns;
        }
        
        public void SaveSettigns(SystemHealthMonitorAppSettings settigns)
        {
            EnsureDirecoryAndFileExists();
            var json = JsonSerializer.Serialize(settigns, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);

            _logger.LogInformation("Settings saved succefully");
        }

        public void EnsureDirecoryAndFileExists()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            if (!File.Exists(_settingsPath))
            {
                string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
                File.Copy(defaultPath, _settingsPath);
            }
        }

    }
}
