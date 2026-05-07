using System;
using System.Collections.Generic;
using System.Text;

namespace SystemHealthMonitor.WPF.Interfaces
{
    internal interface IToastNotificationPipeListener : IPipeListener
    {
        void OnExternalCommand(string message);
        event Action<string> MessageReceived;
    }
}
