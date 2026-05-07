using System.Collections.ObjectModel;
using System.Windows.Input;
using SystemHealthMonitor.WPF.Commands;
using SystemHealthMonitor.WPF.Interfaces;
using SystemHealthMonitor.WPF.Models;
using SystemHealthMonitor.WPF.Views;

namespace SystemHealthMonitor.WPF.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<ScreenDefinition> ScreenDefinition { get; }
       
        private BaseViewModel _selectedTabViewModel;
        private IHealthMonitorNavigationService _healthNavigationService;
        public ICommand SwitchScreenCommand { get; }

        public MainWindowViewModel(IHealthMonitorNavigationService healthNavigationService)
         {
             _healthNavigationService = healthNavigationService;

             ScreenDefinition = new ObservableCollection<ScreenDefinition>()
             {
                 new ScreenDefinition(typeof(SettingsView), typeof(SettingsViewModel), "Settings", true),
                 new ScreenDefinition(typeof(DashboardView), typeof(DashboardViewModel), "Dashboard", false),
                 new ScreenDefinition(typeof(ResultsView), typeof(ResultsViewModel), "Results", false)
                
             };

             _healthNavigationService.NavigateTo<DashboardView, DashboardViewModel>();
             SwitchScreenCommand = new RelayCommand(SwitchScreen);
         }
        private void SwitchScreen(object screen)
        {
            Type viewType = ((ScreenDefinition)screen).ViewType;
            switch(viewType)
            {
               case Type t when t == typeof(DashboardView):
                        _healthNavigationService.NavigateTo<DashboardView, DashboardViewModel>();
                    break;
                case Type t when t == typeof(ResultsView):
                        _healthNavigationService.NavigateTo<ResultsView, ResultsViewModel>();
                    break;
                case Type t when t == typeof(SettingsView):
                        _healthNavigationService.NavigateTo<SettingsView, SettingsViewModel>();
                    break;
            }
        }
    }

}
