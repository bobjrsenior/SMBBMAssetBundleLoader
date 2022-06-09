using Flash2;
using HarmonyLib;

namespace SMBBMFileRedirector
{
    [HarmonyPatch(typeof(Sound.cuesheet_param_t))]
    public class cuesheet_param_tPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Sound.cuesheet_param_t.Unload))]
        static bool Unload(Sound.cuesheet_param_t __instance)
        {
            sound_id.cuesheet in_cueSheet = Sound.GetCueSheetEnumValue(__instance.m_cueSheetName);

            // See if we have a dependency saved for this cuesheet
            sound_id.cuesheet dependsOn;
            if (Plugin.cueSheetDependency.TryGetValue(in_cueSheet, out dependsOn))
            {
                // Get the dependencies cuesheet info
                if (in_cueSheet != dependsOn && Sound.Instance.m_cueSheetParamDict.ContainsKey(dependsOn))
                {
                    // Make sure the dependency isn't already unloaded
                    Sound.cuesheet_param_t cueSheetInfo = Sound.Instance.m_cueSheetParamDict[dependsOn];
                    if (!cueSheetInfo.isLoaded)
                    {
                        // Unload the dependency (TODO This could cause issues is a cue is redirected to another base game cue sheet)
                        Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Unloading {in_cueSheet}. Also unloading {dependsOn} as a dependency");
                        Sound.Instance.LoadCueSheetASync(dependsOn);
                    }
                }
            }
            return true;
        }
    }
}
