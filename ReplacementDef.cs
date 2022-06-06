using System.Collections.Generic;

namespace SMBBMFileRedirector
{
    internal class ReplacementDef
    {
        public string name;
        public string description;
        public string author;
        public int file_format_version;

        public Dictionary<string, string> asset_bundles;
        public Dictionary<string, string> acb_audio_files;
        public Dictionary<string, string> awb_audio_files;
        public Dictionary<string, string> usm_audio_files;
    }
}
