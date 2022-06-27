#if BMM
using SMBBMFileRedirector.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace SMBBMFileRedirector.BMM
{
    public class Main
    {
        private static DelayedPatchHandler delayedPatchHandler;

        public static List<Type> OnModLoad(Dictionary<string, object> settings)
        {
            new PluginResourcesFileRedirector(ModLoader.BEPINEX, "SMBBM File Redirector", new BMMPluginLogger(), Directory.GetCurrentDirectory(), new BMMLeaderboardDisabler());
            PluginStartupShared plugin = new();
            plugin.Load();

            List<Type> listInjectedTypes = new(1);
            return listInjectedTypes;
        }

        public static void OnModUpdate()
        {
            if (delayedPatchHandler == null)
            {
                // Create Detours
                AssetBundleCachePatch.CreateDetour();
                cuesheet_param_tPatch.CreateDetour();
                SoundPatch.CreateDetour();

                // Register our MonoBehaviour
                // For some strange reason, it won't be injected automatically when returned
                // by OnModLoad if the MgCourseDatumElement_tPatch.GetNextStepDelegate is defined
                // I couldn't figure out why because it makes no sense but this gets around the issue.
                ClassInjector.RegisterTypeInIl2Cpp(typeof(DelayedPatchHandler));

                // Create an Object to run our MonoBehaviour
                var obj = new GameObject { hideFlags = HideFlags.HideAndDontSave };
                UnityEngine.Object.DontDestroyOnLoad(obj);
                delayedPatchHandler = new DelayedPatchHandler(obj.AddComponent(Il2CppType.Of<DelayedPatchHandler>()).Pointer);
            }
        }
    }
}
#endif