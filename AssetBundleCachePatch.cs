using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using UnityEngine;
using Framework;
using UnhollowerBaseLib;

namespace SMBBMAssetBundleLoader
{
    [HarmonyPatch(typeof(AssetBundleCache))]
    public class AssetBundleCachePatch
    {
        /*
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.getAssetBundle),
    new[] { typeof(string) })]
        static bool getAssetBundle_String(string in_path)
        {
            if(!in_path.StartsWith("Icon/StandaloneWindows64") && !in_path.StartsWith("UI/") && !in_path.StartsWith("Data/UI/") && !in_path.StartsWith("UI/"))
                Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_path}");
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.PreloadAssetBundleReq),
    new[] { typeof(string), typeof(bool) })]
        static bool PreloadAssetBundleReq_String_Bool(string in_AssetBundleName, bool in_IsBackground)
        {
            Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_AssetBundleName}");
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(AssetBundleCache.get_asset_bundle_name_list),
    new[] { typeof(string) })]
        static void get_asset_bundle_name_list_String(ref Il2CppSystem.Collections.Generic.List<String> __result, string in_path)
        {
            if (!in_path.StartsWith("Icon/StandaloneWindows64") && !in_path.StartsWith("UI/") && !in_path.StartsWith("Data/UI/") && !in_path.StartsWith("UI/"))
            {
                string result = "{";
                if (__result != null && __result.Count > 0)
                {
                    foreach (var val in __result)
                    {
                        result += $"'{val}', ";
                    }
                }
                result += "}";
                Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_path} and returned {result}");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.PreloadReq),
    new[] { typeof(string), typeof(bool) })]
        static bool PreloadReq_String_Bool(string in_path, bool in_isBackground)
        {
            Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_path}");
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.get_streaming_asset_fullpath),
    new[] { typeof(string) })]
        static bool get_streaming_asset_fullpath_String(string in_fileName)
        {
            Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_fileName}");
            return true;
        }
        */
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.get_streaming_asset_fullpath_crifs),
    new[] { typeof(string) })]
        static bool get_streaming_asset_fullpath_crifs_String(ref string __result, string in_fileName)
        {
            //Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_fileName} and returned {__result}");
            if(Plugin.assetBundles.ContainsKey(in_fileName))
            {
                __result = Plugin.assetBundles[in_fileName];
                Plugin.Log.LogInfo($"Patched {in_fileName} with value: {__result}");
                return false;
            }
            return true;
        }
        /*
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.get_streaming_asset_url),
    new[] { typeof(string) })]
        static bool get_streaming_asset_url_String(string in_fileName)
        {
            Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_fileName}");
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.UnloadAssetBundle),
    new[] { typeof(string) })]
        static bool UnloadAssetBundle_String(string in_AssetBundleName)
        {
            Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_AssetBundleName}");
            return true;
        }

        /*[HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.GetAssetWithSubAssets),
    new[] { typeof(string) })]
        static bool GetAssetWithSubAssets_String<T>(ref Il2CppArrayBase<T> __result, string in_path)
        {
            Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} Called with asset bundle path {in_path}");
            return true;
        }*/
    }
}
