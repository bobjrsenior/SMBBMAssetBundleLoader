using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace SMBBMFileRedirector
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(SMBBMLeaderboardDisabler.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BasePlugin
    {
        /// <summary>
        /// For logging convienence
        /// </summary>
        internal static new ManualLogSource Log;

        /// <summary>
        /// The directory that plugin user data is stored in
        /// </summary>
        public static readonly string userDataDir = $"{Paths.GameRootPath}{Path.DirectorySeparatorChar}UserData";

        /// <summary>
        /// The name of this plugins data directory
        /// </summary>
        public static readonly string dataDirName = "FileReplacements";

        /// <summary>
        /// The directory that data for this Plugin is expected
        /// </summary>
        public static readonly string dataDir = $"{userDataDir}{Path.DirectorySeparatorChar}{dataDirName}";

        /// <summary>
        /// A Key/Value map of AssetBundles to patch
        /// Key: Asset Bundle Name
        /// Value: Patch to Asset Bundle to patch it with
        /// </summary>
        internal static Dictionary<string, string> assetBundles;

        /// <summary>
        /// A Key/Value map of Acb audio files to patch
        /// Key: Acb audio file name
        /// Value: Patch to Acb audio file to patch it with
        /// </summary>
        internal static Dictionary<string, string> acbAudioFiles;

        /// <summary>
        /// A Key/Value map of Awb audio files to patch
        /// Key: Awb audio file name
        /// Value: Patch to Awb audio file to patch it with
        /// </summary>
        internal static Dictionary<string, string> awbAudioFiles;


        public override void Load()
        {
            Plugin.Log = base.Log;

            // Make sure the UserData and plugin data directories exist
            // The exists check isn't needed but is included for logging purposes
            if (!Directory.Exists(userDataDir))
            {
                Directory.CreateDirectory(userDataDir);
                Log.LogInfo("Created UserData folder since it didn't already exist");
            }
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
                Log.LogInfo($"Created {userDataDir} folder since it didn't already exist");
            }

            // Find and load all the AssetBundle configuration JSON files
            assetBundles = new Dictionary<string, string>();
            acbAudioFiles = new Dictionary<string, string>();
            awbAudioFiles = new Dictionary<string, string>();
            foreach (var file in Directory.EnumerateFiles(dataDir, "*.json", SearchOption.TopDirectoryOnly))
            {
                LoadJSONFile(file);
            }
            Log.LogDebug("Done loading json files");

            // Log the final AssetBundle key/value set for debugging use
            // Also make sure we don't waste time processing here if we don't log it
            if (Logger.ListenedLogLevels >= LogLevel.Debug)
            {
                string dict = "";
                foreach (KeyValuePair<string, string> assetBundle in assetBundles)
                {
                    dict += $"\"{assetBundle.Key}\", \"{assetBundle.Value}\"\n";
                }
                Log.LogDebug($"Final Asset Bundle List JSON is {{{dict}}}");
                dict = "";
                foreach (KeyValuePair<string, string> acbAudioFile in acbAudioFiles)
                {
                    dict += $"\"{acbAudioFile.Key}\", \"{acbAudioFile.Value}\"\n";
                }
                Log.LogDebug($"Final Acb Audio File List JSON is {{{dict}}}");
                dict = "";
                foreach (KeyValuePair<string, string> awbAudioFile in awbAudioFiles)
                {
                    dict += $"\"{awbAudioFile.Key}\", \"{awbAudioFile.Value}\"\n";
                }
                Log.LogDebug($"Final Awb Audio File List JSON is {{{dict}}}");
            }


            // If we are patching something, make sure to disable the leaderboards
            // Will probably need something more complicated in the future (i.e. should UI patches disable it?)
            if (assetBundles.Count > 0)
            {
                SMBBMLeaderboardDisabler.Plugin.DisableLeaderboards(PluginInfo.PLUGIN_NAME);
            }

            // Harmony Patching
            var harmony = new Harmony("com.bobjrsenior.AssetBundlePatch");
            harmony.PatchAll();

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        /// <summary>
        /// Loads the AssetBundle settings from a given JSON file
        /// </summary>
        /// <param name="filepath">filepath of the JSON file to load</param>
        private void LoadJSONFile(string filepath)
        {
            Log.LogDebug($"Loading file {filepath}");

            using (StreamReader file = File.OpenText(filepath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                // Serializethe JSON file into a C# one
                JObject obj = JToken.ReadFrom(reader) as JObject;
                ReplacementDef replacementDef = obj.ToObject<ReplacementDef>();
                MergeAssetBundles(replacementDef.asset_bundles);
                MergeAcbAudioFiles(replacementDef.acb_audio_files);
                MergeAwbAudioFiles(replacementDef.awb_audio_files);
                Log.LogDebug($"Loaded: {replacementDef}");
            }
        }

        /// <summary>
        /// Merges a dictionary of AssetBundle replacements current AssetBundle Key/Value Patch mapping
        /// </summary>
        /// <param name="assetBundles">assetBundles to merge</param>
        internal void MergeAssetBundles(Dictionary<string, string> newAssetBundles)
        {
            if (newAssetBundles != null)
            {
                foreach (KeyValuePair<string, string> assetBundle in newAssetBundles)
                {
                    assetBundles[assetBundle.Key] = $"{dataDir}{Path.DirectorySeparatorChar}{assetBundle.Value}";
                }
            }
        }

        /// <summary>
        /// Merges a dictionary of Acb audio file replacements current Acb Audio Key/Value Patch mapping
        /// </summary>
        /// <param name="acbAudioFiles">Acb audio files to merge</param>
        internal void MergeAcbAudioFiles(Dictionary<string, string> newAcbAudioFiles)
        {
            if (newAcbAudioFiles != null)
            {
                foreach (KeyValuePair<string, string> acbAudioFile in newAcbAudioFiles)
                {
                    acbAudioFiles[acbAudioFile.Key] = $"{dataDir}{Path.DirectorySeparatorChar}{acbAudioFile.Value}";
                }
            }
        }

        /// <summary>
        /// Merges a dictionary of Awb audio file replacements current Awb Audio Key/Value Patch mapping
        /// </summary>
        /// <param name="awbAudioFiles">Awb audio files to merge</param>
        internal void MergeAwbAudioFiles(Dictionary<string, string> newAwbAudioFiles)
        {
            if (newAwbAudioFiles != null)
            {
                foreach (KeyValuePair<string, string> awbAudioFile in newAwbAudioFiles)
                {
                    awbAudioFiles[awbAudioFile.Key] = $"{dataDir}{Path.DirectorySeparatorChar}{awbAudioFile.Value}";
                }
            }
        }
    }
}
