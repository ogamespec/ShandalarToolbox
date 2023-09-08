using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
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
        public ShandalarAsset(string filePath, byte[] data, ImageType imageType)
        {
            this.filePath = filePath;
            this.data = data;
            this.imageType = imageType;
        }
        public ImageType imageType;
        public string filename, parentName, filePath;
        public byte[] data;
        public Bitmap image;
        public int childIndex;
        public bool hasEmbeddedPalette;


    }
}
