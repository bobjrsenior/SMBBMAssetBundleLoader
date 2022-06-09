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
        /// Value: Path to the Asset Bundle to patch it with
        /// </summary>
        internal static Dictionary<string, string> assetBundles;

        /// <summary>
        /// A Key/Value map of CueSheets to patch
        /// Key: Cue Sheet name
        /// Value: Path to the Cue Sheet to patch it with
        /// </summary>
        internal static Dictionary<string, CueSheetDef> cueSheets;

        /// <summary>
        /// A Key/Value map of Cue to CueSheets
        /// Key: Cue name
        /// Value: Cue Sheet to direct it to
        /// </summary>
        internal static Dictionary<string, string> cueToCueSheets;

        /// <summary>
        /// A depdency mapping from original Cue Sheets to injected ones
        /// Key: Original Cue Sheet
        /// Value: Injected Cue Sheet that holds a Cue from the original Cue Sheet
        /// </summary>
        internal static Dictionary<Flash2.sound_id.cuesheet, Flash2.sound_id.cuesheet> cueSheetDependency;

        /// <summary>
        /// A Key/Value map of Movies to patch
        /// Key: Movie name
        /// Value: Path to the Movie to patch it with
        /// </summary>
        internal static Dictionary<string, string> movies;


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

            // Find and load all the configuration JSON files
            assetBundles = new();
            cueSheets = new();
            cueToCueSheets = new();
            cueSheetDependency = new();
            movies = new();
            foreach (var file in Directory.EnumerateFiles(dataDir, "*.json", SearchOption.TopDirectoryOnly))
            {
                LoadJSONFile(file);
            }
            Log.LogDebug("Done loading json files");

            // Log the final configuration key/value set for debugging use
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
                foreach (KeyValuePair<string, CueSheetDef> cueSheet in cueSheets)
                {
                    dict += $"\"{cueSheet.Key}\", {{\"{cueSheet.Value.acb}\", \"{cueSheet.Value.awb}\"}}\n";
                }
                Log.LogDebug($"Final Cue Sheet List JSON is {{{dict}}}");
                dict = "";
                foreach (KeyValuePair<string, string> cueToCueSheet in cueToCueSheets)
                {
                    dict += $"\"{cueToCueSheet.Key}\", \"{cueToCueSheet.Value}\"\n";
                }
                Log.LogDebug($"Final Cue->Cue Sheet Mapping List JSON is {{{dict}}}");
                dict = "";
                foreach (KeyValuePair<string, string> movie in movies)
                {
                    dict += $"\"{movie.Key}\", \"{movie.Value}\"\n";
                }
                Log.LogDebug($"Final Movie List JSON is {{{dict}}}");
            }


            // If we are patching something, make sure to disable the leaderboards
            // Will probably need something more complicated in the future (i.e. should UI patches disable it?)
            if (assetBundles.Count > 0)
            {
                SMBBMLeaderboardDisabler.Plugin.DisableLeaderboards(PluginInfo.PLUGIN_NAME);
            }

            // Harmony Patching
            var harmony = new Harmony("com.bobjrsenior.SMBBMFileRedirector");
            harmony.PatchAll();

            AddComponent<DelayedPatchHandler>();

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
                MergeCueSheets(replacementDef.cue_sheets);
                MergeCueToCueSheets(replacementDef.cue_to_cue_sheet);
                MergeMovies(replacementDef.movies);
                Log.LogDebug($"Loaded: {replacementDef}");
            }
        }

        /// <summary>
        /// Merges a dictionary of AssetBundle replacements with the current AssetBundle Key/Value Patch mapping
        /// </summary>
        /// <param name="newAssetBundles">assetBundles to merge</param>
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
        /// Merges a dictionary of audio CueSheet file replacements with the current Awb Audio Key/Value Patch mapping
        /// </summary>
        /// <param name="newCueSheets">Cue Sheets to merge</param>
        internal void MergeCueSheets(Dictionary<string, CueSheetDef> newCueSheets)
        {
            if (newCueSheets != null)
            {
                foreach (KeyValuePair<string, CueSheetDef> cueSheet in newCueSheets)
                {
                    if (cueSheet.Value.acb != null && cueSheet.Value.acb.Length > 0)
                        cueSheet.Value.acb = $"{dataDir}{Path.DirectorySeparatorChar}{cueSheet.Value.acb}";
                    if (cueSheet.Value.awb != null && cueSheet.Value.awb.Length > 0)
                        cueSheet.Value.awb = $"{dataDir}{Path.DirectorySeparatorChar}{cueSheet.Value.awb}";

                    cueSheets[cueSheet.Key] = cueSheet.Value;
                }
            }
        }

        /// <summary>
        /// Merges a dictionary of Cue->Cue Sheet mappings with the current Cue->Cue Sheet Key/Value mappings
        /// </summary>
        /// <param name="newCueToCueSheet">Cue->CueSheet mappings to merge</param>
        internal void MergeCueToCueSheets(Dictionary<string, string> newCueToCueSheet)
        {
            if (newCueToCueSheet != null)
            {
                foreach (KeyValuePair<string, string> cueToCueShe in newCueToCueSheet)
                {
                    cueToCueSheets[cueToCueShe.Key] = cueToCueShe.Value;
                }
            }
        }

        /// <summary>
        /// Merges a dictionary of Movie replacements with the current Movie Key/Value Patch mapping
        /// </summary>
        /// <param name="newAssetBundles">Movies to merge</param>
        internal void MergeMovies(Dictionary<string, string> newMovies)
        {
            if (newMovies != null)
            {
                foreach (KeyValuePair<string, string> movie in newMovies)
                {
                    // Prepend "Movie/" since for easier comparing with the game's internal path
                    movies[$"Movie/{movie.Key}"] = $"{dataDir}{Path.DirectorySeparatorChar}{movie.Value}";
                }
            }
        }
    }
}
