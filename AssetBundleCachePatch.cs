using Framework;
using HarmonyLib;

namespace SMBBMFileRedirector
{
    [HarmonyPatch(typeof(AssetBundleCache))]
    public class AssetBundleCachePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AssetBundleCache.get_streaming_asset_fullpath_crifs),
    new[] { typeof(string) })]
        static bool get_streaming_asset_fullpath_crifs_String(ref string __result, string in_fileName)
        {
            if (Plugin.assetBundles.ContainsKey(in_fileName))
            {
                __result = Plugin.assetBundles[in_fileName];
                Plugin.Log.LogDebug($"Patched AssetBundle {in_fileName} with filepath: {__result}");
                return false;
            }
            return true;
        }
    }
}
