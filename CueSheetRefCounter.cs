using Flash2;
using System.Collections.Generic;

namespace SMBBMFileRedirector
{
    /// <summary>
    /// Keeps track of how many Cue Sheets reference an injected one
    /// to make sure it loads and unloads only when it should be
    /// </summary>
    internal class CueSheetRefCounter
    {
        private static Dictionary<sound_id.cuesheet, CueSheetRefCounter> refDict = new();

        private sound_id.cuesheet cueSheet;
        int numRefs = 0;

        public CueSheetRefCounter(sound_id.cuesheet cueSheet)
        {
            this.cueSheet = cueSheet;
        }

        public static bool AddReference(sound_id.cuesheet cueSheet)
        {
            CueSheetRefCounter refCounter;
            if (refDict.TryGetValue(cueSheet, out refCounter))
            {
                refCounter.numRefs++;
                // If the ref count was 0 before, we need to load the asset bundle
                if (refCounter.numRefs == 1)
                    return true;
                return false;
            }
            else
            {
                refCounter = new(cueSheet);
                refCounter.numRefs++;
                refDict[cueSheet] = refCounter;
                return true;
            }
        }

        public static bool RemoveReference(sound_id.cuesheet cueSheet)
        {
            CueSheetRefCounter refCounter;
            if (refDict.TryGetValue(cueSheet, out refCounter))
            {
                refCounter.numRefs--;
                // If the ref count is now 0, we need to unload the asset bundle
                if (refCounter.numRefs == 0)
                    return true;
                return false;
            }
            else
            {
                PluginResourcesFileRedirector.Instance.PluginLogger.LogDebug($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name}: Trying to remove ref for CueSheet {cueSheet} but it isn't tracked!");
                return false;
            }
        }
    }
}
