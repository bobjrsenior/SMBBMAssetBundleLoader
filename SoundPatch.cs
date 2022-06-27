using Flash2;
using HarmonyLib;
using System;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace SMBBMFileRedirector
{
    public class SoundPatch
    {
        // BMM doesn't support IL2CPP Harmony Patches
        // So we a Detour instead
        private delegate void LoadCueSheetASyncDelegate(IntPtr _thisPtr, sound_id.cuesheet in_cueSheet);
        private static LoadCueSheetASyncDelegate LoadCueSheetASyncInstance;
        private static LoadCueSheetASyncDelegate LoadCueSheetASyncdDelegateOriginal;
        public static unsafe void CreateDetour()
        {
            LoadCueSheetASyncInstance = LoadCueSheetASync;

            var original = typeof(Sound).GetMethod(nameof(Sound.LoadCueSheetASync), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));

            LoadCueSheetASyncdDelegateOriginal = ClassInjector.Detour.Detour(methodInfo.MethodPointer,
                               LoadCueSheetASyncInstance);
        }

        static void LoadCueSheetASync(IntPtr _thisPtr, sound_id.cuesheet in_cueSheet)
        {
            Sound __instance = new(_thisPtr);

            // See if we have a dependency saved for this cuesheet
            sound_id.cuesheet dependsOn;
            if (PluginResourcesFileRedirector.Instance.cueSheetDependency.TryGetValue(in_cueSheet, out dependsOn))
            {
                // Get the dependencies cuesheet info
                if (in_cueSheet != dependsOn && __instance.m_cueSheetParamDict.ContainsKey(dependsOn))
                {
                    // Make sure the dependency isn't already loading or loaded
                    Sound.cuesheet_param_t cueSheetInfo = __instance.m_cueSheetParamDict[dependsOn];
                    if (!cueSheetInfo.isLoading && !cueSheetInfo.isLoaded && CueSheetRefCounter.AddReference(dependsOn))
                    {
                        // Load the dependency
                        PluginResourcesFileRedirector.Instance.PluginLogger.LogDebug($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Loading {in_cueSheet}. Also loading {dependsOn} as a dependency");
                        __instance.LoadCueSheetASync(dependsOn);
                    }
                }
            }
            LoadCueSheetASyncdDelegateOriginal(_thisPtr, in_cueSheet);
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
