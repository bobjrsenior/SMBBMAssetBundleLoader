#if BIE
using SMBBMFileRedirector.PluginInterfaces;

namespace SMBBMFileRedirector.BepInEx
{
    public class BepInExPluginLogger : PluginLogger
    {
        public void LogDebug(string message)
        {
            Plugin.Log.LogDebug(message);
        }

        public void LogInfo(string message)
        {
            Plugin.Log.LogInfo(message);
        }
    }
}
#endif