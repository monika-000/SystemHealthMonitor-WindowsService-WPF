using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using SystemHealthMonitor.WPF.ViewModels;

namespace SystemHealthMonitor.WPF.Interfaces
{
    public interface IHealthMonitorNavigationService
    {
        BaseViewModel NavigateTo<TView, TViewModel>() 
            where TView : UserControl
            where TViewModel: BaseViewModel;
        void BringWindowToFront();

    }
}
