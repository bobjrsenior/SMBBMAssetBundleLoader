using HarmonyLib;

namespace SMBBMFileRedirector
{
    [HarmonyPatch(typeof(CriAtomExAcbLoader))]
    public class CriAtomExAcbLoaderPatch
    {
        /*[HarmonyPrefix]
        [HarmonyPatch(nameof(CriAtomExAcbLoader.LoadAcbFileAsync),
    new[] { typeof(CriFsBinder), typeof(string), typeof(string), typeof(bool) })]
        static bool LoadAcbFileAsync_bind_string_string(CriFsBinder binder, ref string acbPath, ref string awbPath, bool loadAwbOnMemory)
        {
            string acbFileName = Path.GetFileName(acbPath);
            if (Plugin.acbAudioFiles.ContainsKey(acbFileName))
            {
                acbPath = Plugin.acbAudioFiles[acbFileName];
                Plugin.Log.LogDebug($"Patched Acb Audio File {acbFileName} with filepath: {acbPath}");
            }
            // Not every acb file comes with a corresponding awb file
            if (awbPath != null && awbPath.Length > 0)
            {
                string awbFileName = Path.GetFileName(awbPath);
                if (Plugin.awbAudioFiles.ContainsKey(awbFileName))
                {
                    awbPath = Plugin.awbAudioFiles[awbFileName];
                    Plugin.Log.LogDebug($"Patched Acb Audio File {awbFileName} with filepath: {awbPath}");
                }
            }
            return true;
        }*/
    }
}
