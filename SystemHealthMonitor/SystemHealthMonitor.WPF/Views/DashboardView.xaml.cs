using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.ViewModels;


namespace SystemHealthMonitor.WPF.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            var main = Application.Current.MainWindow as MainWindow;
            main?.CleanupMemoryRequest += OnCleanupMemoryRequest;
            main?.LoadCollectionsToMemoryRequest += OnLoadCollectionsRequest;
        }

        private void OnCleanupMemoryRequest()
        {
            var vm = DataContext as DashboardViewModel;
            vm?.CleanupMemeoryOnWindowClosed<SystemHealthResults>(vm.LatestResults, vm.Results, vm.WorstResults);

        }
        private void OnLoadCollectionsRequest()
        {
            var vm = DataContext as DashboardViewModel;
            vm?.LoadData();
        }
    }
}
