using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Input;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Commands;

namespace SystemHealthMonitor.WPF.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private bool _settingsLoadedSuccessfully;
        private readonly ILogger<SettingsViewModel> _logger;
        public ICommand SaveSettingsCommand { get; }
        private readonly ISettingsService _settingsService;
        private SystemHealthMonitorAppSettings _settings;
        public SystemHealthMonitorAppSettings Settings 
        { 
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }
        

        public SettingsViewModel(ISettingsService settingsService, ILogger<SettingsViewModel> logger)
        {
            _settingsService = settingsService;
            _logger = logger;
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            LoadSettings();
        }

       
        private void SaveSettings(object obj)
        {
            try
            {
                SystemHealthMonitorAppSettings newSettings = new SystemHealthMonitorAppSettings
                {
                    Thresholds = new MetricsThresholds()
                    {
                        PoolingIntervalSeconds = Settings.Thresholds.PoolingIntervalSeconds,
                        CPUThresholdPercent = Settings.Thresholds.CPUThresholdPercent,
                        MemoryThresholdPercent = Settings.Thresholds.MemoryThresholdPercent,
                        UsedDiskSpaceThresholdPercent = Settings.Thresholds.UsedDiskSpaceThresholdPercent,
                        DiskIOThresholdPercent = Settings.Thresholds.DiskIOThresholdPercent,
                        NetworkIOThresholdPercent = Settings.Thresholds.NetworkIOThresholdPercent,
                        IncomingPacketsLossThreshold = Settings.Thresholds.IncomingPacketsLossThreshold,
                        OutgoingPacketsLossThreshold = Settings.Thresholds.OutgoingPacketsLossThreshold
                    }
                };

                _settingsService.SaveSettigns(newSettings);
                MessageBox.Show("Settings saved successfully.", "Settings saved", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save settings. Exception: {0}", ex);
                MessageBox.Show("Couldn't save settings. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                Settings = _settingsService.GetSystemHealthMonitorAppSettings();
                _settingsLoadedSuccessfully = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load settings: Exception: {0}", ex);
                _settingsLoadedSuccessfully = false;
            }
        }

        public void OnUserControlLoaded()
        {
            if(!_settingsLoadedSuccessfully)
            {
                MessageBox.Show("Couldn't load settings. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
