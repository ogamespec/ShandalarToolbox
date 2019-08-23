using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShandalarImageDecoder
{
    public enum AssetType
    {
        Image
    }
    public class ShandalarAsset
    {
        public ShandalarAsset(AssetType assetType, string name)
        {
            this.assetType = assetType;
            this.name = name;
        }
        public AssetType assetType;
        public string name;

    }
}
