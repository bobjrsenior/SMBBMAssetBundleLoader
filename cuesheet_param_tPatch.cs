using Flash2;
using HarmonyLib;
using System;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using System.IO;

namespace SMBBMFileRedirector
{
    public class cuesheet_param_tPatch
    {
        // BMM doesn't support IL2CPP Harmony Patches
        // So we a Detour instead
        private delegate bool UnloadDelegate(IntPtr _thisPtr);
        private static UnloadDelegate UnloadDelegateInstance;
        private static UnloadDelegate UnloadDelegateOriginal;

        public static unsafe void CreateDetour()
        {
            UnloadDelegateInstance = Unload;

            var original = typeof(Sound.cuesheet_param_t).GetMethod(nameof(Sound.cuesheet_param_t.Unload), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));

            UnloadDelegateOriginal = ClassInjector.Detour.Detour(methodInfo.MethodPointer,
                               UnloadDelegateInstance);
        }

        static bool Unload(IntPtr _thisPtr)
        {
            Sound.cuesheet_param_t __instance = new(_thisPtr);

            sound_id.cuesheet in_cueSheet = Sound.GetCueSheetEnumValue(__instance.m_cueSheetName);
            if (in_cueSheet == sound_id.cuesheet.invalid && !PluginResourcesFileRedirector.Instance.newCueSheetMapping.TryGetValue(__instance.m_cueSheetName, out in_cueSheet))
            {
                PluginResourcesFileRedirector.Instance.PluginLogger.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Cue Sheet {__instance.m_cueSheetName} is invalid!");
            }

            // See if we have a dependency saved for this cuesheet
            sound_id.cuesheet dependsOn;
            if (PluginResourcesFileRedirector.Instance.cueSheetDependency.TryGetValue(in_cueSheet, out dependsOn))
            {
                // Get the dependencies cuesheet info
                if (in_cueSheet != dependsOn && Sound.Instance.m_cueSheetParamDict.ContainsKey(dependsOn))
                {
                    // Make sure the dependency isn't already unloaded
                    Sound.cuesheet_param_t cueSheetInfo = Sound.Instance.m_cueSheetParamDict[dependsOn];
                    if (cueSheetInfo.isLoaded && CueSheetRefCounter.RemoveReference(dependsOn))
                    {
                        PluginResourcesFileRedirector.Instance.PluginLogger.LogDebug($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Unloading {in_cueSheet}. Also unloading {dependsOn} as a dependency");
                        Sound.Instance.UnloadCueSheet(dependsOn);
                    }
                }
            }
            return UnloadDelegateOriginal(_thisPtr);
        }
    }
}
