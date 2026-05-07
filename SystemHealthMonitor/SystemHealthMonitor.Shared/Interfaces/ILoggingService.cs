using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SystemHealthMonitor.Shared.Interfaces
{
    internal interface ILoggingService
    {
         void Configure(string basePath);
    }
}
