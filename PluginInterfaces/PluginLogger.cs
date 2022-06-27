using System;
using System.Collections.Generic;
using System.Text;

namespace SMBBMFileRedirector.PluginInterfaces
{
    public interface PluginLogger
    {
        void LogDebug(string message);
        void LogInfo(string message);
    }
}
