using System;
using System.Collections.Generic;
using System.Text;

namespace SMBBMAssetBundleLoader
{
    internal class AssetBundleSettings
    {
        public string name;
        public string description;
        public string author;
        public int file_format_version;

        public Dictionary<string, string> asset_bundles;
    }
}
