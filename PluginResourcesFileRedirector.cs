using SMBBMFileRedirector.PluginInterfaces;
using System.Collections.Generic;

namespace SMBBMFileRedirector
{
    internal class PluginResourcesFileRedirector : PluginInterfaces.PluginResources<PluginResourcesFileRedirector>
    {

        public static PluginResourcesFileRedirector Instance;

        public override string dataDirName => "FileReplacements";

        /// <summary>
        /// A Key/Value map of AssetBundles to patch
        /// Key: Asset Bundle Name
        /// Value: Path to the Asset Bundle to patch it with
        /// </summary>
        public Dictionary<string, string> assetBundles;

        /// <summary>
        /// A Key/Value map of Assets to AssetBundles
        /// Key: Asset name
        /// Value: Asset Bundle to redirect it to
        /// </summary>
        public Dictionary<string, string> assetToAssetBundles;

        /// <summary>
        /// A Key/Value map of CueSheets to patch
        /// Key: Cue Sheet name
        /// Value: Path to the Cue Sheet to patch it with
        /// </summary>
        public Dictionary<string, CueSheetDef> cueSheets;

        /// <summary>
        /// A Key/Value map of Cue to CueSheets
        /// Key: Cue name
        /// Value: Cue Sheet to direct it to
        /// </summary>
        public Dictionary<string, string> cueToCueSheets;

        /// <summary>
        /// A depdency mapping from original Cue Sheets to injected ones
        /// Key: Original Cue Sheet
        /// Value: Injected Cue Sheet that holds a Cue from the original Cue Sheet
        /// </summary>
        public Dictionary<Flash2.sound_id.cuesheet, Flash2.sound_id.cuesheet> cueSheetDependency;

        /// <summary>
        /// A map of 'injected' Cue Sheets names to their Cue Sheet Enum
        /// </summary>
        public Dictionary<string, Flash2.sound_id.cuesheet> newCueSheetMapping;

        /// <summary>
        /// A Key/Value map of Movies to patch
        /// Key: Movie name
        /// Value: Path to the Movie to patch it with
        /// </summary>
        public Dictionary<string, string> movies;

        public PluginResourcesFileRedirector(ModLoader modLoader, string pluginName, PluginLogger pluginLogger, string gameRootPath, LeaderboardDisabler leaderboardDisabler = null) : base(modLoader, pluginName, pluginLogger, gameRootPath, leaderboardDisabler)
        {
            Instance = this;
        }
    }
}
