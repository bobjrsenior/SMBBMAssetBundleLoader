using System;
using System.IO;

namespace SMBBMFileRedirector.PluginInterfaces
{
    internal abstract class PluginResources<T>
    {
        /// <summary>
        /// The directory of the game root
        /// </summary>
        public string gameRootPath;

        /// <summary>
        /// The directory that plugin user data is stored in
        /// </summary>
        public string userDataDir;

        /// <summary>
        /// The name of this plugins data directory
        /// </summary>
        public abstract string dataDirName { get; }

        /// <summary>
        /// The directory that data for this Plugin is expected
        /// </summary>
        public string dataDir;

        /// <summary>
        /// Waas this mod loaded by BMM or BepInEx?
        /// </summary>
        public ModLoader MOD_LOADER;

        /// <summary>
        /// How should we log things?
        /// </summary>
        public PluginLogger PluginLogger;

        /// <summary>
        /// How should the leaderboard be disabled?
        /// </summary>
        public LeaderboardDisabler LeaderboardDisabler;

        public string PLUGIN_NAME;

        public PluginResources(ModLoader modLoader, string pluginName, PluginLogger pluginLogger, String gameRootPath, LeaderboardDisabler leaderboardDisabler = null)
        {
            PLUGIN_NAME = pluginName;
            MOD_LOADER = modLoader;
            this.PluginLogger = pluginLogger;
            this.gameRootPath = gameRootPath;
            userDataDir = $"{gameRootPath}{Path.DirectorySeparatorChar}UserData";
            dataDir = $"{userDataDir}{Path.DirectorySeparatorChar}{dataDirName}";
            LeaderboardDisabler = leaderboardDisabler;
        }
    }
}
