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

            int newWidth;
            int newHeight;

            if ((int)BitConverter.ToUInt32(asset.data, 0) != 0)
            {
                bool halfSize = BitConverter.ToUInt32(asset.data, 0x28) == 1 ? true : false;
                if (halfSize){
                    newWidth = width / 2;
					newHeight = height / 2;
				}
                else{
                    newWidth = width;
					newHeight = height;
				}
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }

            int ptr1 = 0;
            int ptr2 = ptr1 + width * width * 4 + 0x80;
            int ptr3 = ptr2 + newWidth * newWidth * 4 + 0x80;

            int[] tempArray = GeneralUtilityFunctions.ByteArrayToIntArray(uncompressedData, 0);

            Wavelet.WaveletDecode(ref tempArray, ptr1, width, smallTableSize);
            Wavelet.WaveletDecode(ref tempArray, ptr2 / 4, newWidth, smallTableSize);
            Wavelet.WaveletDecode(ref tempArray, ptr3 / 4, newWidth, smallTableSize);

            /// YCbCr -> RGB
            Bitmap outputImage = Wavelet.Decode_YCbCrToRGB(tempArray,
            ptr1,
            width,
            height,
            ptr2 / 4,
            ptr3 / 4,
            newWidth,
            newHeight);

            // Return image
            return outputImage;
        }
    }
}
