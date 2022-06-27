#if BMM
using SMBBMFileRedirector.PluginInterfaces;

namespace SMBBMFileRedirector.BMM
{
    public class BMMLeaderboardDisabler : LeaderboardDisabler
    {
        public void DisableLeaderboards()
        {
            // Disabled by BMM pre-startup
        }

        public void DisableLeaderboards(string disabler)
        {
            // Disabled by BMM pre-startup
        }
    }
}
#endif