using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ShandalarImageToolbox
{
    public class CATImageDecoder
    {
        public static Bitmap DecodeCatImage(ShandalarAsset asset)
        {
            //Console.WriteLine("Decompressing " + asset.filename);
            byte[] uncompressedData = Vlc.VlcDecompress(asset.data);

            int width = (int)BitConverter.ToUInt32(asset.data, 0x1c);
            int height = (int)BitConverter.ToUInt32(asset.data, 0x20);
            int smallTableSize = (int)BitConverter.ToUInt32(asset.data, 0x24);

            int var_14;
            int var_24;

            if ((int)BitConverter.ToUInt32(asset.data, 0) != 0)
            {
                /// var_14
                int eax = width;
                int ecx = (int)BitConverter.ToUInt32(asset.data, 0x28) - 1;
                if (ecx == 0)
                    var_14 = eax / 2;
                else
                    var_14 = eax;

                /// var_24
                eax = height;
                ecx = (int)BitConverter.ToUInt32(asset.data, 0x28) - 1;
                if (ecx == 0)
                    var_24 = eax / 2;
                else
                    var_24 = eax;
            }
            else
            {
                var_14 = width;
                var_24 = height;
            }

            int ptr1 = 0;
            int ptr2 = ptr1 + width * width * 4 + 0x80;
            int ptr3 = ptr2 + var_14 * var_14 * 4 + 0x80;

            int[] tempArray = GeneralUtilityFunctions.ByteArrayToIntArray(uncompressedData, 0);

            Wavelet.WaveletDecode(ref tempArray, ptr1, width, smallTableSize);
            Wavelet.WaveletDecode(ref tempArray, ptr2 / 4, var_14, smallTableSize);
            Wavelet.WaveletDecode(ref tempArray, ptr3 / 4, var_14, smallTableSize);

            /// YCbCr -> RGB
            Bitmap outputImage = Wavelet.Decode_YCbCrToRGB(tempArray,
            ptr1,
            width,
            height,
            ptr2 / 4,
            ptr3 / 4,
            var_14,
            var_24);

            // Return image
            return outputImage;
        }
    }
}
