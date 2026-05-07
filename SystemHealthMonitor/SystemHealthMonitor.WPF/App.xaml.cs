using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;
using Serilog.Core;
using System.IO;
using System.Windows;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.Shared.Services;
using SystemHealthMonitor.WPF.Interfaces;
using SystemHealthMonitor.WPF.Services;
using SystemHealthMonitor.WPF.ViewModels;
using SystemHealthMonitor.WPF.Views;
using Forms = System.Windows.Forms;

namespace SystemHealthMonitor.WPF
{
    public partial class App : Application
    {
        private Forms.NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;
        private WindowsToastNotificationService _windowsToastNotificationService;
        private IToastNotificationPipeListener _toastNotificationPipeListener;
        private IHealthMonitorNavigationService _windowsToastNavigationService;
        private IPipeListener _notificationPipeListener;
        private IHost _builder;
        private static Mutex _mutex;
        private readonly CancellationTokenSource _cts = new();
        public App()
        {
            string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MPCorp", "SystemHealthMonitorApp", "logs");
            LoggingService loggingService = new LoggingService();
            loggingService.Configure(logsPath);

            _notifyIcon = new Forms.NotifyIcon();

            _builder = Host.CreateDefaultBuilder()
                        .UseSerilog()
                        .ConfigureServices(services =>
                        {
                            //1. Services
                            services.AddSingleton<IPipeListener, NotificationPipeListener>();
                            services.AddSingleton<IToastNotificationPipeListener, ToastNotificationPipeListener>();
                            services.AddSingleton<IWindowsToastNotificationService, WindowsToastNotificationService>();
                            services.AddSingleton<ISettingsService, SettingsService>();
                            services.AddSingleton<IResultsService, ResultsService>();
                            
                            services.AddSingleton<MainWindow>();

                            //2. HealthMonitorNavigationService (depeands on MainWindow)
                            services.AddSingleton<IHealthMonitorNavigationService>(sp =>
                            {
                                _mainWindow = sp.GetRequiredService<MainWindow>();
                                var host = _mainWindow.MainContentControl;
                                return new HealthMonitorNavigationService(sp, _mainWindow, host);

                            });

                            //3. ViewModels   
                            services.AddTransient<NotificationViewModel>();
                            services.AddSingleton<MainWindowViewModel>();
                            services.AddTransient<ResultsViewModel>();
                            services.AddTransient<DashboardViewModel>();
                            services.AddTransient<SettingsViewModel>();

                            //4. Views
                            services.AddTransient<ResultsView>();
                            services.AddTransient<DashboardView>();
                            services.AddTransient<SettingsView>();

                        })
                        .Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var token = _cts.Token;

            bool createdNew;
            _mutex = new Mutex(true, "SystemHealthMonitorApp", out createdNew);
     
            if (!createdNew)
            {
                if (e.Args.Length > 0)
                {
                   await ToastNotificationPipeClient.SendMessageAsync(e.Args[0]);
                }

                Shutdown();
                return;
            }

            _notifyIcon.Icon = new System.Drawing.Icon("Icons/SystemHealthMonitor.ico");
            _notifyIcon.Text = "SystemHealthMonitor";
            _notifyIcon.MouseClick += new Forms.MouseEventHandler(NotifyIcon_Click);
            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Quit", null, OnQuitClicked);
            _notifyIcon.Visible = true;
        
            // Start pipe listener    
            _windowsToastNavigationService = _builder.Services.GetRequiredService<IHealthMonitorNavigationService>();
            _toastNotificationPipeListener = _builder.Services.GetRequiredService<IToastNotificationPipeListener>();
            _toastNotificationPipeListener.MessageReceived += _toastNotificationPipeListener.OnExternalCommand;
            _notificationPipeListener = _builder.Services.GetRequiredService<IPipeListener>();

            // Listener run an infite loops, start them in the background,so main thread is not blocked
            StartListenerWithRestart(_toastNotificationPipeListener, "Toast Notification Pipe Listener", token);
            StartListenerWithRestart(_notificationPipeListener, "Notification Pipe Listener", token);

            _windowsToastNotificationService = new WindowsToastNotificationService();

            ToastNotificationManagerCompat.OnActivated += async args =>
            {
                await ToastNotificationPipeClient.SendMessageAsync(args.Argument);
            };

            _mainWindow.DataContext = _builder.Services.GetRequiredService<MainWindowViewModel>();
                
            _mainWindow.Show();
            _mainWindow.Activate(); 
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

        private void StartListenerWithRestart(IPipeListener listener, string name, CancellationToken token)
        {
           
                Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            await listener.StartAsync();
                        }
                        catch (Exception ex)
                        {
                            await Task.Delay(1000); //Restart service
                        }
                    }

                });
        }
    }
}

