using Flash2;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;


namespace SMBBMFileRedirector
{
    internal class DelayedSoundHandler : MonoBehaviour
    {
        private readonly float startupDelay = 1.0f;
        private float curDelay = 0.0f;
        private bool initializedCueSheets = false;
        private bool initializedCues = false;

        /// <summary>
        /// A mapping of injected Cue Sheet Enums to their Enum int value
        /// </summary>
        private Dictionary<string, int> newCueSheetMapping;

        void Update()
        {
            if (!initializedCueSheets || !initializedCues)
            {
                curDelay += Time.deltaTime;
                if (curDelay > startupDelay)
                {
                    // Mkae sure the CueSheets are setup first in case a new one is injected
                    if (!initializedCueSheets)
                        InitializeCueSheet();
                    if (initializedCueSheets && !initializedCues)
                        InitializeCues();
                }
            }
        }

        private void InitializeCueSheet()
        {
            initializedCueSheets = true;

            // Try to get the game's cue sheet dictionary (it may not exist at frame 0)
            Il2CppSystem.Collections.Generic.Dictionary<sound_id.cuesheet, Sound.cuesheet_param_t> cueSheetDict = Sound.Instance.m_cueSheetParamDict;
            if (cueSheetDict != null)
            {
                // Initialize our new cue sheet mappings (used if we inject a new cue sheet)
                newCueSheetMapping = new();
                int newEnumKeyValue = 0x65;

                // Go through every cue sheet mapping we have configured
                foreach (KeyValuePair<string, CueSheetDef> cueSheet in Plugin.cueSheets)
                {
                    // Get the cue sheet's enum value
                    sound_id.cuesheet enumValue = Sound.GetCueSheetEnumValue(cueSheet.Key);
                    if (cueSheetDict.ContainsKey(enumValue))
                    {
                        // The enum exists and is in the dictionary so grab the info and patch it
                        Sound.cuesheet_param_t cueSheetParam = cueSheetDict[enumValue];
                        cueSheetParam.m_acbFileName = cueSheet.Value.acb;
                        Plugin.Log.LogDebug($"Patched Acb for Cue Sheet {cueSheet.Key} with filepath: {cueSheet.Value.acb}");
                        if (cueSheet.Value.awb != null)
                        {
                            cueSheetParam.m_awbFileName = cueSheet.Value.awb;
                            Plugin.Log.LogDebug($"Patched Awb for Cue Sheet {cueSheet.Key} with filepath: {cueSheet.Value.awb}. It has a Cue Sheet name of cueSheet name of {cueSheetParam.m_cueSheetName}");

                        }
                    }
                    else if (enumValue == sound_id.cuesheet.invalid)
                    {
                        // The CueSheet Enum doesn't exist yet so we have to inject it ourselves
                        Il2CppSystem.Type fieldAttrType = UnhollowerRuntimeLib.Il2CppType.From(typeof(sound_id.cuesheet));
                        EnumInjector.InjectEnumValues(typeof(sound_id.cuesheet), new()
                        {
                            [cueSheet.Key] = newEnumKeyValue
                        });

                        // Now that it's been injected, create a cuesheet object
                        // add it to the games dictionary of cuesheets
                        // and keept track of the num we added
                        Sound.cuesheet_param_t newCueSheet = new(cueSheet.Key, cueSheet.Value.acb, cueSheet.Value.awb);
                        cueSheetDict[(sound_id.cuesheet)newEnumKeyValue] = newCueSheet;
                        newCueSheetMapping[cueSheet.Key] = newEnumKeyValue;
                        Plugin.Log.LogDebug($"Injected new sound_id.cuesheet enum {cueSheet.Key} ({newEnumKeyValue}), gave it Acb path {newCueSheet.acbFileName} and Awb path {newCueSheet.awbFileName}");
                        newEnumKeyValue++;
                    }
                    else
                    {
                        // The cue sheet exists but the game didn't map it
                        Plugin.Log.LogDebug($"The cue sheet {cueSheet.Value.awb} is a valid Enum but isn't mapping by the game");
                    }

                }
            }
            else
            {
                initializedCueSheets = false;
                curDelay = startupDelay;
            }
        }

        private void InitializeCues()
        {
            initializedCues = true;

            // Try to get the games cue dictionary (it may not exist at frame 0)
            Il2CppSystem.Collections.Generic.Dictionary<sound_id.cue, Sound.cue_param_t> cueDict = Sound.Instance.m_cueParamDict;
            if (cueDict != null)
            {
                // Go through every cue sheet mapping we have configured
                foreach (KeyValuePair<string, string> cueToCueSheet in Plugin.cueToCueSheets)
                {
                    // Get the cue's enum value
                    sound_id.cue enumValue = Sound.GetCueEnumValue(cueToCueSheet.Key);
                    if (cueDict.ContainsKey(enumValue))
                    {
                        // Try to get the cue information from the games cue dictionary
                        Sound.cue_param_t cueParam = cueDict[enumValue];

                        // Get the Cue Sheet Enum we want to redirect this cue to and update the game's cue information with it
                        sound_id.cuesheet cueSheetEnumValue = Sound.GetCueSheetEnumValue(cueToCueSheet.Value);
                        sound_id.cuesheet oldCueSheet = cueParam.cueSheet;
                        cueParam.cueSheet = cueSheetEnumValue;

                        // Keep track of redirected cues so that we can make sure the redirected cue sheets are loaded
                        Plugin.cueSheetDependency[oldCueSheet] = cueSheetEnumValue;
                        Plugin.Log.LogDebug($"Redirected Cue {cueToCueSheet.Key} to Cue Sheet {cueToCueSheet.Value}");
                    }
                }
            }
            else
            {
                initializedCues = false;
            }
        }
    }
}
