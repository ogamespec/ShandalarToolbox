using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace ShandalarImageToolbox
{
    public enum ImageType
    {
        Pic,
        Spr,
        Cat
    }

    public class ShandalarAsset
    {
        public ShandalarAsset(string filename, byte[] data, ImageType imageType)
        {
            this.filename = filename;
            this.data = data;
            this.imageType = imageType;
        }
        public ImageType imageType;
        public string filename, parentName;
        public byte[] data;
        public Bitmap image;
        public int childIndex;
        public bool hasEmbeddedPalette;


    }
}
