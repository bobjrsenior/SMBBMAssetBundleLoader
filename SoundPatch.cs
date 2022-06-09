using Flash2;
using HarmonyLib;

namespace SMBBMFileRedirector
{
    [HarmonyPatch(typeof(Sound))]
    public class SoundPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Sound.LoadCueSheetASync),
    new[] { typeof(sound_id.cuesheet) })]
        static bool LoadCueSheetASync(Sound __instance, sound_id.cuesheet in_cueSheet)
        {
            // See if we have a dependency saved for this cuesheet
            sound_id.cuesheet dependsOn;
            if (Plugin.cueSheetDependency.TryGetValue(in_cueSheet, out dependsOn))
            {
                // Get the dependencies cuesheet info
                if (in_cueSheet != dependsOn && __instance.m_cueSheetParamDict.ContainsKey(dependsOn))
                {
                    // Make sure the dependency isn't already loading or loaded
                    Sound.cuesheet_param_t cueSheetInfo = __instance.m_cueSheetParamDict[dependsOn];
                    if (!cueSheetInfo.isLoading && !cueSheetInfo.isLoaded)
                    {
                        // Load the dependency
                        Plugin.Log.LogDebug($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Loading {in_cueSheet}. Also loading {dependsOn} as a dependency");
                        __instance.LoadCueSheetASync(dependsOn);
                    }
                }
            }
            return true;
        }

        /* 
         * Unfortunately, Sound.UnloadCueSheet method is called in a way unsupported
         * by BepInEx so I have to use cuesheet_param_t for unload handling instead
         * This is kept here for now in case support gets added
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Sound.UnloadCueSheet),
new[] { typeof(sound_id.cuesheet) })]
        static bool UnloadCueSheet(Sound __instance, sound_id.cuesheet in_cueSheet)
        {
            Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Unloading Cue Sheet {in_cueSheet}");
            // See if we have a dependency saved for this cuesheet
            sound_id.cuesheet dependsOn;
            if (Plugin.cueSheetDependency.TryGetValue(in_cueSheet, out dependsOn))
            {
                // Get the dependencies cuesheet info
                if (in_cueSheet != dependsOn && __instance.m_cueSheetParamDict.ContainsKey(dependsOn))
                {
                    // Make sure the dependency isn't already unloaded
                    Sound.cuesheet_param_t cueSheetInfo = __instance.m_cueSheetParamDict[dependsOn];
                    if (!cueSheetInfo.isLoaded)
                    {
                        // Unload the dependency (TODO This could cause issues is a cue is redirected to another base game cue sheet)
                        Plugin.Log.LogDebug($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Unloading {in_cueSheet}. Also unloading {dependsOn} as a dependency");
                        __instance.LoadCueSheetASync(dependsOn);
                    }
                }
            }
            return true;
        }*/
    }
}
