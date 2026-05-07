using System;
using System.Collections.Generic;
using System.Text;

namespace SystemHealthMonitor.WPF.Interfaces
{
    internal interface IPipeListener
    {
        Task StartAsync();
    }
}
