using System;
using System.Collections.Generic;
using System.Text;

namespace SMBBMFileRedirector.PluginInterfaces
{
    public interface LeaderboardDisabler
    {
        void DisableLeaderboards();
        void DisableLeaderboards(string disabler);
    }
}
