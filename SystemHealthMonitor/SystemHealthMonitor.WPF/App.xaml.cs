using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Windows;
using SystemHealthMonitor.WPF.Interfaces;
using SystemHealthMonitor.WPF.Services;
using SystemHealthMonitor.WPF.ViewModels;
using Forms = System.Windows.Forms;

namespace SystemHealthMonitor.WPF
{
    public partial class App : Application
    {
        private readonly Forms.NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;
        private readonly WindowsToastNotificationService _windowsToastNotificationService;
        private IHost _builder;
        public App()
        {
            _notifyIcon = new Forms.NotifyIcon();
            _windowsToastNotificationService = new WindowsToastNotificationService();

            _builder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    //1. Services
                    services.AddSingleton<INotificationPipeListener, NotificationPipeListener>();
                    services.AddSingleton<IWindowsToastNotificationService, WindowsToastNotificationService>();
                    services.AddSingleton<INavigationService, NavigationService>();

                    //2. ViewModels   
                    services.AddTransient<NotificationViewModel>();

                    //3. Views
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _notifyIcon.Icon = new System.Drawing.Icon("Icons/SystemHealthMonitor.ico");
            _notifyIcon.Text = "SystemHealthMonitor";
            _notifyIcon.MouseClick += new Forms.MouseEventHandler(NotifyIcon_Click);
            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Quit", null, OnQuitClicked);
            _notifyIcon.Visible = true;
           
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                _windowsToastNotificationService.HandleToastNotificationClicks(toastArgs, _windowsToastNotificationService);
            };
            _mainWindow = _builder.Services.GetRequiredService<MainWindow>();
            _mainWindow.Show();

            base.OnStartup(e);
        }

        private void NotifyIcon_Click(object sender, Forms.MouseEventArgs e)
        {
            if(e.Button == Forms.MouseButtons.Left)
            {
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.Show();
                _mainWindow.Activate();
            } 
        }

        private void OnQuitClicked(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();
            base.OnExit(e);
        }
    }

}
