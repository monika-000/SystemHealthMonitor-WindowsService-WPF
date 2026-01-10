using System.Text.Json;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;

namespace SystemHealthMonitor.Shared.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsPath;
        public SettingsService()
        {
                _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SystemHealthMonitorApp", "settings.json");
        }

        public SystemHealthMonitorAppSettings GetSystemHealthMonitorAppSettings()
        {
            if (!File.Exists(_settingsPath))
            {
                return new SystemHealthMonitorAppSettings();
            }

            var json = File.ReadAllText(_settingsPath);
            SystemHealthMonitorAppSettings settigns = JsonSerializer.Deserialize<SystemHealthMonitorAppSettings>(json);

            return settigns;
        }
        
        public bool SaveSettigns(SystemHealthMonitorAppSettings settigns)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
                var json = JsonSerializer.Serialize(settigns, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsPath, json);
                return true;
            }
            catch (Exception ex)
            {
                //log
                return false;
            }
        }

    }
}
