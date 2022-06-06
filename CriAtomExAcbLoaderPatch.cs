using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HarmonyLib;

namespace SMBBMAssetBundleLoader
{
    [HarmonyPatch(typeof(CriAtomExAcbLoader))]
    public class CriAtomExAcbLoaderPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CriAtomExAcbLoader.LoadAcbFileAsync),
    new[] { typeof(CriFsBinder), typeof(string), typeof(string), typeof(bool) })]
        static bool LoadAcbFileAsync_bind_string_string(CriFsBinder binder, ref string acbPath, ref string awbPath, bool loadAwbOnMemory)
        {
            Plugin.Log.LogInfo($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Loading acbPath {acbPath} and awbPath {awbPath}");
            string acbFileName = Path.GetFileName(acbPath);
            if (Plugin.acbAudioFiles.ContainsKey(acbFileName))
            {
                acbPath = Plugin.acbAudioFiles[acbFileName];
                Plugin.Log.LogInfo($"Patched Acb Audio File {acbFileName} with filepath: {acbPath}");
            }
            // Not every acb file comes with a corresponding awb file
            if(awbPath != null && awbPath.Length > 0)
            {
                string awbFileName = Path.GetFileName(awbPath);
                if (Plugin.awbAudioFiles.ContainsKey(awbFileName))
                {
                    awbPath = Plugin.awbAudioFiles[awbFileName];
                    Plugin.Log.LogInfo($"Patched Acb Audio File {awbFileName} with filepath: {awbPath}");
                }
            }
            return true;
        }
    }
}
