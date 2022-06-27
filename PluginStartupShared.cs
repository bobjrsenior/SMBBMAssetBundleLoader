using System.Collections.Generic;
using System.IO;

namespace SMBBMFileRedirector
{
    public class PluginStartupShared
    {
        private PluginResourcesFileRedirector resourses;
        public void Load()
        {
            // Solely because the name is too long...
            resourses = PluginResourcesFileRedirector.Instance;

            // Make sure the UserData and plugin data directories exist
            // The exists check isn't needed but is included for logging purposes
            if (!Directory.Exists(resourses.userDataDir))
            {
                Directory.CreateDirectory(resourses.userDataDir);
                resourses.PluginLogger.LogInfo("Created UserData folder since it didn't already exist");
            }
            if (!Directory.Exists(resourses.dataDir))
            {
                Directory.CreateDirectory(resourses.dataDir);
                resourses.PluginLogger.LogInfo($"Created {resourses.userDataDir} folder since it didn't already exist");
            }

            // Find and load all the configuration JSON files
            resourses.assetBundles = new();
            resourses.assetToAssetBundles = new();
            resourses.cueSheets = new();
            resourses.cueToCueSheets = new();
            resourses.cueSheetDependency = new();
            resourses.movies = new();
            foreach (var file in Directory.EnumerateFiles(resourses.dataDir, "*.json", SearchOption.TopDirectoryOnly))
            {
                LoadJSONFile(file);
            }
            resourses.PluginLogger.LogDebug("Done loading json files");

            // Prevent excesses logging for BMM
            // TODO Only log this when logging debug info
            if (PluginResourcesFileRedirector.Instance.MOD_LOADER == PluginInterfaces.ModLoader.BEPINEX)
            {
                string dict = "";
                foreach (KeyValuePair<string, string> assetBundle in resourses.assetBundles)
                {
                    dict += $"\"{assetBundle.Key}\", \"{assetBundle.Value}\"\n";
                }
                resourses.PluginLogger.LogDebug($"Final Asset Bundle List JSON is {{{dict}}}");
                dict = "";
                foreach (KeyValuePair<string, string> assetToAssetBundle in resourses.assetToAssetBundles)
                {
                    dict += $"\"{assetToAssetBundle.Key}\", \"{assetToAssetBundle.Value}\"\n";
                }
                resourses.PluginLogger.LogDebug($"Final Asset->Asset Bundle Mapping List JSON is {{{dict}}}");
                dict = "";
                foreach (KeyValuePair<string, CueSheetDef> cueSheet in resourses.cueSheets)
                {
                    dict += $"\"{cueSheet.Key}\", {{\"{cueSheet.Value.acb}\", \"{cueSheet.Value.awb}\"}}\n";
                }
                resourses.PluginLogger.LogDebug($"Final Cue Sheet List JSON is {{{dict}}}");
                dict = "";
                foreach (KeyValuePair<string, string> cueToCueSheet in resourses.cueToCueSheets)
                {
                    dict += $"\"{cueToCueSheet.Key}\", \"{cueToCueSheet.Value}\"\n";
                }
                resourses.PluginLogger.LogDebug($"Final Cue->Cue Sheet Mapping List JSON is {{{dict}}}");
                dict = "";
                foreach (KeyValuePair<string, string> movie in resourses.movies)
                {
                    dict += $"\"{movie.Key}\", \"{movie.Value}\"\n";
                }
                resourses.PluginLogger.LogDebug($"Final Movie List JSON is {{{dict}}}");
            }


            // If we are patching something, make sure to disable the leaderboards
            // Will probably need something more complicated in the future (i.e. should UI patches disable it?)
            if (resourses.assetBundles.Count > 0)
            {
                resourses.LeaderboardDisabler.DisableLeaderboards(PluginResourcesFileRedirector.Instance.PLUGIN_NAME);
            }
        }

        /// <summary>
        /// Loads the AssetBundle settings from a given JSON file
        /// </summary>
        /// <param name="filepath">filepath of the JSON file to load</param>
        private void LoadJSONFile(string filepath)
        {
            resourses.PluginLogger.LogDebug($"Loading file {filepath}");

            // Serializethe JSON file into a C# one
            var replacementDef = System.Text.Json.JsonSerializer.Deserialize<ReplacementDef>(File.ReadAllText(filepath));
            MergeAssetBundles(replacementDef.asset_bundles);
            MergeAssetToAssetBundles(replacementDef.asset_to_asset_bundles);
            MergeCueSheets(replacementDef.cue_sheets);
            MergeCueToCueSheets(replacementDef.cue_to_cue_sheet);
            MergeMovies(replacementDef.movies);
            resourses.PluginLogger.LogDebug($"Loaded: {replacementDef}");
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
                    resourses.assetBundles[assetBundle.Key] = $"{resourses.dataDir}{Path.DirectorySeparatorChar}{assetBundle.Value}";
                }
            }
        }

        /// <summary>
        /// Merges a dictionary of Asset->Asset Bundle mappings with the current Asset->Asset Bundle Key/Value mappings
        /// </summary>
        /// <param name="newAssetToAssetBundles">Asset->Asset Bundle mappings to merge</param>
        internal void MergeAssetToAssetBundles(Dictionary<string, string> newAssetToAssetBundles)
        {
            if (newAssetToAssetBundles != null)
            {
                foreach (KeyValuePair<string, string> assetToAssetBundle in newAssetToAssetBundles)
                {
                    resourses.assetToAssetBundles[assetToAssetBundle.Key] = $"{resourses.dataDir}{Path.DirectorySeparatorChar}{assetToAssetBundle.Value}";
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
                        cueSheet.Value.acb = $"{resourses.dataDir}{Path.DirectorySeparatorChar}{cueSheet.Value.acb}";
                    if (cueSheet.Value.awb != null && cueSheet.Value.awb.Length > 0)
                        cueSheet.Value.awb = $"{resourses.dataDir}{Path.DirectorySeparatorChar}{cueSheet.Value.awb}";

                    resourses.cueSheets[cueSheet.Key] = cueSheet.Value;
                }
            }
        }

        /// <summary>
        /// Merges a dictionary of Cue->Cue Sheet mappings with the current Cue->Cue Sheet Key/Value mappings
        /// </summary>
        /// <param name="newCueToCueSheet">Cue->CueSheet mappings to merge</param>
        internal void MergeCueToCueSheets(Dictionary<string, string> newCueToCueSheets)
        {
            if (newCueToCueSheets != null)
            {
                foreach (KeyValuePair<string, string> cueToCueSheet in newCueToCueSheets)
                {
                    resourses.cueToCueSheets[cueToCueSheet.Key] = cueToCueSheet.Value;
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
                    resourses.movies[$"Movie/{movie.Key}"] = $"{resourses.dataDir}{Path.DirectorySeparatorChar}{movie.Value}";
                }
            }
        }
    }
}
