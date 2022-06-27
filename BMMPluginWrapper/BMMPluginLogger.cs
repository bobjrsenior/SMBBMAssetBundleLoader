#if BMM
using SMBBMFileRedirector.PluginInterfaces;
using System;

namespace SMBBMFileRedirector.BMM
{
    public class BMMPluginLogger : PluginLogger
    {
        public void LogDebug(string message)
        {
            // As far as I know BMM doesn't really have an active Log Level
            // To avoid completely spamming the output, this is disabled for BMM
            //Console.WriteLine($"DEBUG: {message}");
        }

        public void LogInfo(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }
    }
}
#endif