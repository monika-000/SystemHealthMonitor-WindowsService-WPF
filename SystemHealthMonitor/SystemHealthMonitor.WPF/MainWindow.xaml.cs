using System.ComponentModel;
using System.Windows;

namespace SystemHealthMonitor.WPF
{

    public partial class MainWindow : Window
    {
        public event Action? CleanupMemoryRequest;
        public event Action? LoadCollectionsToMemoryRequest;

        public MainWindow()
        {
            InitializeComponent();
            this.IsVisibleChanged += MainWindow_IsVisibleChanged;
        }

        private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(this.IsVisible)
            {
                LoadCollectionsToMemoryRequest?.Invoke();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();

            CleanupMemoryRequest?.Invoke();

        }

    }
}