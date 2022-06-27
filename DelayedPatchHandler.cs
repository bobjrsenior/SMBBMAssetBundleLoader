using Flash2;
using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace SMBBMFileRedirector
{
    internal class DelayedPatchHandler : MonoBehaviour
    {
        private PluginResourcesFileRedirector resourses;

        private readonly float startupDelay = 0.1f;
        private float curDelay = 0.0f;
        private bool initializedAssetBundles = false;
        private bool initializedCueSheets = false;
        private bool initializedCues = false;
        private bool initializedMovies = false;

        public DelayedPatchHandler(IntPtr value) : base(value) { }

        public DelayedPatchHandler() { }

        void Start()
        {
            // Solely because the name is too long...
            resourses = PluginResourcesFileRedirector.Instance;
        }

        void Update()
        {
            // This delay is here in case all of the data isn't
            // initialized by frame 0
            if (!FullyInitialized())
            {
                curDelay += Time.deltaTime;
                if (curDelay > startupDelay)
                {
                    // Mkae sure the CueSheets are setup first in case a new one is injected
                    if (!initializedAssetBundles)
                        InitializeAssetBundles();
                    if (!initializedCueSheets)
                        InitializeCueSheet();
                    if (initializedCueSheets && !initializedCues)
                        InitializeCues();
                    if (!initializedMovies)
                        InitializeMovies();
                }
            }
        }

        private bool FullyInitialized()
        {
            return initializedAssetBundles && initializedCueSheets && initializedCues && initializedMovies;
        }

        /// <summary>
        /// Very Experimental
        /// </summary>
        private void InitializeAssetBundles()
        {
            initializedAssetBundles = true;

            Il2CppSystem.Collections.Generic.Dictionary<string, string> pathToAssetBundleDict = AssetBundleCache.Instance.m_pathToAssetBundleNameDict;
            if (pathToAssetBundleDict != null && AssetBundleCache.Instance.m_isReady)
            {
                foreach (KeyValuePair<string, string> assetToAssetBundle in resourses.assetToAssetBundles)
                {
                    if (pathToAssetBundleDict.ContainsKey(assetToAssetBundle.Key))
                    {
                        // Determine if we are redirecting to a new bundle or an existing one
                        string oldAssetBundle = pathToAssetBundleDict[assetToAssetBundle.Key];
                        pathToAssetBundleDict[assetToAssetBundle.Key] = assetToAssetBundle.Value;
                        resourses.PluginLogger.LogDebug($"Redirected Asset {assetToAssetBundle.Key} to Asset Bundle {assetToAssetBundle.Value}");

                        // Now add the new dependency
                        if (!AssetBundleCache.Instance.m_assetBundleDependencyDict.ContainsKey(oldAssetBundle))
                        {
                            AssetBundleCache.Instance.m_assetBundleDependencyDict.Add(oldAssetBundle, new());
                            resourses.PluginLogger.LogDebug($"Added dep list to {oldAssetBundle} since it didn't have one");
                        }
                        if (!AssetBundleCache.Instance.m_assetBundleDependencyDict[oldAssetBundle].Contains(assetToAssetBundle.Value))
                        {
                            AssetBundleCache.Instance.m_assetBundleDependencyDict[oldAssetBundle].Add(assetToAssetBundle.Value);
                            resourses.PluginLogger.LogDebug($"Added {assetToAssetBundle.Value} as a dependency to {oldAssetBundle}");

                        }
                        // See if the new asset bundle exists yet
                        if (!AssetBundleCache.Instance.m_assetBundleDependencyDict.ContainsKey(assetToAssetBundle.Value))
                        {
                            AssetBundleCache.Instance.m_assetBundleDependencyDict.Add(assetToAssetBundle.Value, new());
                            AssetBundleCache.Instance.m_assetBundleDependencyDict[assetToAssetBundle.Value].Add(oldAssetBundle);
                            resourses.PluginLogger.LogDebug($"Added {assetToAssetBundle.Value} to the dep dict with a dep on {oldAssetBundle}");
                        }
                    }
                    else
                    {
                        // Can't redsirect since the original asset doesn't exist
                        resourses.PluginLogger.LogDebug($"Can't find Asset {assetToAssetBundle.Key} in Asset list");
                    }
                }
            }
            else
            {
                initializedAssetBundles = false;
            }

        }

        private void InitializeCueSheet()
        {
            initializedCueSheets = true;

            // Try to get the game's cue sheet dictionary (it may not exist at frame 0)
            Il2CppSystem.Collections.Generic.Dictionary<sound_id.cuesheet, Sound.cuesheet_param_t> cueSheetDict = Sound.Instance.m_cueSheetParamDict;
            if (cueSheetDict != null)
            {
                // Initialize our new cue sheet mappings (used if we inject a new cue sheet)
                resourses.newCueSheetMapping = new();

                // Figure out what the next valid Enum int value is in case we inject one
                int newEnumKeyValue = Enum.GetValues(typeof(sound_id.cuesheet)).Length;

                // Go through every cue sheet mapping we have configured
                foreach (KeyValuePair<string, CueSheetDef> cueSheet in resourses.cueSheets)
                {
                    // Get the cue sheet's enum value
                    sound_id.cuesheet enumValue = Sound.GetCueSheetEnumValue(cueSheet.Key);
                    if (cueSheetDict.ContainsKey(enumValue))
                    {
                        // The enum exists and is in the dictionary so grab the info and patch it
                        Sound.cuesheet_param_t cueSheetParam = cueSheetDict[enumValue];
                        cueSheetParam.m_acbFileName = cueSheet.Value.acb;
                        resourses.PluginLogger.LogDebug($"Patched Acb for Cue Sheet {cueSheet.Key} with filepath: {cueSheet.Value.acb}");
                        if (cueSheet.Value.awb != null)
                        {
                            cueSheetParam.m_awbFileName = cueSheet.Value.awb;
                            resourses.PluginLogger.LogDebug($"Patched Awb for Cue Sheet {cueSheet.Key} with filepath: {cueSheet.Value.awb}. It has a Cue Sheet name of cueSheet name of {cueSheetParam.m_cueSheetName}");
                        }
                    }
                    else if (enumValue == sound_id.cuesheet.invalid)
                    {
                        // Now that it's been injected, create a cuesheet object
                        // add it to the games dictionary of cuesheets
                        // and keept track of the num we added
                        Sound.cuesheet_param_t newCueSheet = new(cueSheet.Key, cueSheet.Value.acb, cueSheet.Value.awb);
                        cueSheetDict[(sound_id.cuesheet)newEnumKeyValue] = newCueSheet;
                        resourses.newCueSheetMapping[cueSheet.Key] = (sound_id.cuesheet)newEnumKeyValue;
                        resourses.PluginLogger.LogDebug($"Injected new sound_id.cuesheet enum {cueSheet.Key} ({newEnumKeyValue}), gave it Acb path {newCueSheet.acbFileName} and Awb path {newCueSheet.awbFileName}");
                        newEnumKeyValue++;
                    }
                    else
                    {
                        // The cue sheet exists but the game didn't map it
                        resourses.PluginLogger.LogDebug($"The cue sheet {cueSheet.Value.awb} is a valid Enum but isn't mapping by the game");
                    }
                }
            }
            else
            {
                initializedCueSheets = false;
                curDelay = startupDelay;
            }
        }

        private void InitializeCues()
        {
            initializedCues = true;

            // Try to get the games cue dictionary (it may not exist at frame 0)
            Il2CppSystem.Collections.Generic.Dictionary<sound_id.cue, Sound.cue_param_t> cueDict = Sound.Instance.m_cueParamDict;
            if (cueDict != null)
            {
                // Go through every cue sheet mapping we have configured
                foreach (KeyValuePair<string, string> cueToCueSheet in resourses.cueToCueSheets)
                {
                    // Get the cue's enum value
                    sound_id.cue enumValue = Sound.GetCueEnumValue(cueToCueSheet.Key);
                    if (cueDict.ContainsKey(enumValue))
                    {
                        // Try to get the cue information from the games cue dictionary
                        Sound.cue_param_t cueParam = cueDict[enumValue];

                        // Get the Cue Sheet Enum we want to redirect this cue to and update the game's cue information with it
                        sound_id.cuesheet cueSheetEnumValue = Sound.GetCueSheetEnumValue(cueToCueSheet.Value);
                        if (cueSheetEnumValue == sound_id.cuesheet.invalid)
                        {
                            if (!resourses.newCueSheetMapping.TryGetValue(cueToCueSheet.Value, out cueSheetEnumValue))
                                resourses.PluginLogger.LogInfo($"Cue {enumValue} redirected to invalid Cue Sheet {cueToCueSheet.Value}!");
                        }

                        sound_id.cuesheet oldCueSheet = cueParam.cueSheet;
                        cueParam.cueSheet = cueSheetEnumValue;

                        // Keep track of redirected cues so that we can make sure the redirected cue sheets are loaded
                        resourses.cueSheetDependency[oldCueSheet] = cueSheetEnumValue;
                        resourses.PluginLogger.LogDebug($"Redirected Cue {cueToCueSheet.Key} to Cue Sheet {cueToCueSheet.Value}");
                    }
                }
            }
            else
            {
                initializedCues = false;
            }

        }

        private void InitializeMovies()
        {
            initializedMovies = true;

            // Try to get the games movie param
            MovieParam movieParam = MovieManager.Instance.m_MovieParam;
            if (movieParam != null && movieParam.m_Data != null)
            {
                // Get the list of movie datums from it
                Il2CppSystem.Collections.Generic.List<MovieParam.Datum> movieDatums = movieParam.m_Data;
                foreach (MovieParam.Datum movieDatum in movieDatums)
                {
                    // Go through every path in every datum...
                    // There should only be one patch for each so who knows why it's a list...
                    for (int i = 0; i < movieDatum.m_PathList.Count; i++)
                    {
                        // Check if it's in our patch list and patch if it is
                        string path = movieDatum.m_PathList[i];
                        if (resourses.movies.ContainsKey(path))
                        {
                            movieDatum.m_PathList[i] = resourses.movies[path];
                            resourses.PluginLogger.LogDebug($"Patched Movie {path} with filepath: {movieDatum.m_PathList[i]}");
                        }

                    }
                }
            }
            else
            {
                initializedMovies = false;
            }
        }
    }
}
