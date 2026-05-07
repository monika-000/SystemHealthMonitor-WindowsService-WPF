using System.Windows;
using System.Windows.Controls;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.ViewModels;

namespace SystemHealthMonitor.WPF.Views
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
            var main = Application.Current.MainWindow as MainWindow;
            main?.CleanupMemoryRequest += OnCleanupMemoryRequest;
            main?.LoadCollectionsToMemoryRequest += OnLoadCollectionsRequest;
        } 

        private void OnCleanupMemoryRequest()
        {
            var rvm = DataContext as ResultsViewModel;
            rvm?.CleanupMemeoryOnWindowClosed<SystemHealthResults>(rvm.PageResults, rvm.AllResults, rvm.FilteredResults);
        }
        private void OnLoadCollectionsRequest()
        {
            var vm = DataContext as ResultsViewModel;
            vm?.LoadData();
        }
      
    }
}
