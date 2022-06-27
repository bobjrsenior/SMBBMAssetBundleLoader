using System.Collections.Generic;

namespace SMBBMFileRedirector
{
    internal class ReplacementDef
    {
        public string name { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public int file_format_version { get; set; }

        public Dictionary<string, string> asset_bundles { get; set; }
        public Dictionary<string, string> asset_to_asset_bundles { get; set; }
        public Dictionary<string, CueSheetDef> cue_sheets { get; set; }
        public Dictionary<string, string> cue_to_cue_sheet { get; set; }
        public Dictionary<string, string> movies { get; set; }
    }
}
