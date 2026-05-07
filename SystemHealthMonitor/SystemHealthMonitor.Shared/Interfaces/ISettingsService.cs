using System;
using System.Collections.Generic;
using System.Text;
using SystemHealthMonitor.Shared.Models;

namespace SystemHealthMonitor.Shared.Interfaces
{
    public interface ISettingsService
    {
        SystemHealthMonitorAppSettings GetSystemHealthMonitorAppSettings();
        void SaveSettigns(SystemHealthMonitorAppSettings settigns);
        void EnsureDirecoryAndFileExists();
    }
}
