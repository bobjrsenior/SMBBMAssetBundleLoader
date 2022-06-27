using Framework;
using HarmonyLib;
using System;
using System.IO;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;


namespace SMBBMFileRedirector
{
    public class AssetBundleCachePatch
    {
        // BMM doesn't support IL2CPP Harmony Patches
        // So we a Detour instead
        private delegate IntPtr get_streaming_asset_fullpath_crifs_delegate(IntPtr in_fileName);
        private static get_streaming_asset_fullpath_crifs_delegate get_streaming_asset_fullpath_crifs_delegate_instance;
        private static get_streaming_asset_fullpath_crifs_delegate get_streaming_asset_fullpath_crifs_delegate_original;

        private static readonly string StreamingAssetsSubPath = $"smbbm_Data{Path.DirectorySeparatorChar}StreamingAssets{Path.DirectorySeparatorChar}StandaloneWindows64{Path.DirectorySeparatorChar}AssetBundles";
        private static string assetBundleDir;

        public static unsafe void CreateDetour()
        {
            get_streaming_asset_fullpath_crifs_delegate_instance = get_streaming_asset_fullpath_crifs_detour;

            assetBundleDir = $"{PluginResourcesFileRedirector.Instance.gameRootPath}{Path.DirectorySeparatorChar}{StreamingAssetsSubPath}";

            var original = typeof(AssetBundleCache).GetMethod(nameof(AssetBundleCache.get_streaming_asset_fullpath_crifs), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));

            get_streaming_asset_fullpath_crifs_delegate_original = ClassInjector.Detour.Detour(methodInfo.MethodPointer,
                               get_streaming_asset_fullpath_crifs_delegate_instance);
        }

        static IntPtr get_streaming_asset_fullpath_crifs_detour(IntPtr in_fileName)
        {
            string filename = new Il2CppSystem.String(in_fileName);

            // Return our redirect if we have one
            if (PluginResourcesFileRedirector.Instance.assetBundles.ContainsKey(filename))
            {
                return ((Il2CppSystem.String)PluginResourcesFileRedirector.Instance.assetBundles[filename]).Pointer;
            }
            // Otherwise let the original method handle it
            return get_streaming_asset_fullpath_crifs_delegate_original(in_fileName);
        }
    }
}
