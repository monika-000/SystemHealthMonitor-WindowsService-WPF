using SystemHealthMonitor.WPF.Interfaces;

namespace SystemHealthMonitor.WPF.Services
{
    internal class NavigationService : INavigationService
    {
        private readonly MainWindow _mainWindow;

        public NavigationService(MainWindow mainWindow)
        {
              _mainWindow = mainWindow;  
        }
        public void ShowResultsWindow()
        {

        }
    }
}
