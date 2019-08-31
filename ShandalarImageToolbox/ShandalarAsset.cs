using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace ShandalarImageToolbox
{

    public class ShandalarAsset
    {
        public ShandalarAsset(string filename, byte[] data)
        {
            this.filename = filename;
            this.data = data;
        }
        public string filename;
        public byte[] data;
        public Bitmap image;


    }
}
