using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using SystemHealthMonitor.WPF.Interfaces;
using SystemHealthMonitor.WPF.ViewModels;
using SystemHealthMonitor.WPF.Views;


namespace SystemHealthMonitor.WPF.Services
{
    public class HealthMonitorNavigationService : IHealthMonitorNavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MainWindow _mainWindow;
        private readonly ContentControl _contentHost;
        public HealthMonitorNavigationService(IServiceProvider serviceProvider, MainWindow mainWindow, ContentControl contentHost)
        {
            _serviceProvider = serviceProvider;
            _mainWindow = mainWindow;
            _contentHost = contentHost;
        }

        public BaseViewModel NavigateTo<TView, TViewModel>()
            where TView : UserControl
            where TViewModel : BaseViewModel
        {
            var vm = _serviceProvider.GetRequiredService<TViewModel>();
            
            //Run on main STA thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                var view = _serviceProvider.GetRequiredService<TView>();
              
                view.DataContext = vm;
                _contentHost.Content = view;
              
            });
            return vm;
        }
        public void NavigateTo(Type viewType)
        {
            var view = (UserControl)_serviceProvider.GetRequiredService(viewType);
            _contentHost.Content = view;
        }
        public void BringWindowToFront()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_mainWindow.WindowState == System.Windows.WindowState.Minimized)
                {
                    _mainWindow.WindowState = System.Windows.WindowState.Normal;
                }
                _mainWindow.Show();
                _mainWindow.Focus();
            });
        }

    }
}
