
using System.Windows;
using System.Windows.Controls;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.ViewModels;


namespace SystemHealthMonitor.WPF.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) 
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var vm = (SettingsViewModel)DataContext;
                vm.OnUserControlLoaded();
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }
        private void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
           
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
      
    }
}
