using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ShandalarImageToolbox
{
    public class Wavelet
    {
        
        
        public static bool waveLetBuffersReady;
        public static int[] waveLetBuffer_1;
        public static int[] waveLetBuffer_2;
        public static int buf1Index, buf2Index;
        public static int savedCtab;
        public static int[] savedData;
        public static int savedDataOffset;

        public static void CardWaveletDecode_Sub7(                            // 0x492410
            int[] data,
            int tab1,
            int tab2,
            ref int[] tmp,
            int tmpIndex,
            int arg_C,
            int arg_10,
            int arg_14,
            int arg_18
        )
        {
            for (int i = 0; i < arg_10; i++)
            {
                int var_C = tab1;
                int var_10 = tab1 + arg_C;
                int var_8 = tmpIndex + arg_18 * 2;
                tab1++;

                while (tab1 < var_10)
                {
                    tmp[var_8] = savedData[tab1] + savedData[tab2];
                    tmp[var_8 + arg_18] = savedData[tab1] - savedData[tab2];
                    tab1++;
                    tab2++;
                    var_8 += arg_18 * 2;
                }

                tmp[tmpIndex] = savedData[var_C] + savedData[tab2];
                tmp[tmpIndex + arg_18] = savedData[var_C] - savedData[tab2];

                tmpIndex++;
                tab2++;
            }
        }

        public static void CardWaveletDecode_Sub8(                    // 0x4924E5
            ref int[] buf1,
            ref int[] buf2,
            int outTab,
            int arg_C,
            int arg_10,
            int arg_14,
            int arg_18)
        {
            int tempBuf1Index = buf1Index, tempBuf2Index = buf2Index;

            for (int i = 0; i < arg_10; i++)
            {
                int var_C = tempBuf1Index;
                int var_10 = tempBuf1Index + arg_C;
                int var_8 = outTab + arg_18 * 2;
                tempBuf1Index++;

                while (tempBuf1Index < var_10)
                {
                    savedData[var_8] = (buf1[tempBuf1Index] + buf2[tempBuf2Index]) / 2;
                    savedData[var_8 + arg_18] = (buf1[tempBuf1Index] - buf2[tempBuf2Index]) / 2;

                    tempBuf1Index++;
                    tempBuf2Index++;
                    var_8 += arg_18 * 2;
                }

                savedData[outTab] = (buf1[var_C] + buf2[tempBuf2Index]) / 2;
                savedData[outTab+arg_18] = (buf1[var_C] - buf2[tempBuf2Index]) / 2;

                outTab++;
                tempBuf2Index++;
            }
        }

        public static void WaveletDecode(ref int[] data, int ctab, int width, int tabSize)                   // 0x4922CB
        {
            int[] buf1, buf2;
            savedData = data;

            if (!waveLetBuffersReady)
            {
                buf1 = waveLetBuffer_1 = new int[0x10000];
                buf2 = waveLetBuffer_2 = new int[0x10000];
                buf1Index = 0;
                buf2Index = 0;
                waveLetBuffersReady = true;
            }
            else
            {
                buf1 = waveLetBuffer_1;
                buf2 = waveLetBuffer_2;
                buf1Index = 0;
                buf2Index = 0;
            }

            int size = tabSize;

            while (size < width)
            {
                CardWaveletDecode_Sub7(
                    data,
                    ctab,
                    size * size + ctab,
                    ref buf1,
                    buf1Index,
                    size,
                    size,
                    2 * size,       // not used
                    size);

                CardWaveletDecode_Sub7(
                    data,
                    size * size * 2 + ctab,
                    size * size * 3 + ctab,
                    ref buf2,
                    buf2Index,
                    size,
                    size,
                    2 * size,       // not used
                    size);

                CardWaveletDecode_Sub8(
                    ref buf1,
                    ref buf2,
                    ctab,
                    size,
                    2 * size,
                    2 * size,       // not used
                    2 * size);

                size *= 2;
            }
            data = savedData;
        }

        ///////////////////////////////////////////////////////////

        public static bool YCbCrTabReady;
        public static byte[] YCbCrTab = new byte[0x400 + 0x1c00];
        public static int YCbCrTabPtr = 0x400;
        public static int rgbPtr;
        
        public static Bitmap Decode_YCbCrToRGB(                 // 0x4925E6
            int[] data,                    // always 0
            int YTab,
            int width,
            int height,
            int CbTab,
            int CrTab,
            int derivedWidth,
            int derivedHeight
        )
        {
            Bitmap image = new Bitmap(width, height);
            /// Prepare table

            if (!YCbCrTabReady)
            {
                int i = -0x400;

                while (i < 0x1c00)
                {
                    if (i > 0)
                    {
                        int eax = i >> 2;
                        if (eax >= 0xff)
                        {
                            eax = 0xff;
                        }
                        YCbCrTab[i + YCbCrTabPtr] = (byte)eax;
                    }
                    else
                    {
                        YCbCrTab[i + YCbCrTabPtr] = 0;
                    }

                    i++;
                }

                YCbCrTabReady = true;
            }

            /// Allocate RGB buffer


            for (int y = 0; y < height; y++)
            {
                int CbPtr;
                int CrPtr;

                if (derivedHeight != 0)
                {
                    CbPtr = CbTab + (y / 2) * derivedWidth;
                    CrPtr = CrTab + (y / 2) * derivedWidth;
                }
                else
                {
                    CbPtr = CbTab + y * derivedWidth;
                    CrPtr = CrTab + y * derivedWidth;
                }

                for (int x = 0; x < width; x++)
                {
                    int yval = data[YTab];
                    int CbVal;
                    int CrVal;

                    ////

                    if (derivedHeight != 0)
                    {
                        if ((x & 1) != 0)          /// Lerp
                        {
                            int eax = width - x - 1;
                            eax = (eax == 0) ? -1 : 0;
                            eax = data[CbPtr+eax + 1];
                            CbVal = (data[CbPtr] + eax) / 2;

                            eax = width - x - 1;
                            eax = (eax == 0) ? -1 : 0;
                            eax = data[CrPtr+eax + 1];
                            CrVal = (data[CrPtr] + eax) / 2;
                        }
                        else
                        {
                            CbVal = data[CbPtr];
                            CrVal = data[CrPtr];
                        }
                    }
                    else
                    {
                        CbVal = data[CbPtr];
                        CrVal = data[CrPtr];
                    }

                    ////

                    int r = yval + CrVal + CrVal / 2 + CrVal / 8 - 0x333;
                    int b = yval + CbVal * 2 - 0x400;
                    int g = yval * 2 - yval / 4 - r / 2 - b / 4 - b / 16;

                    image.SetPixel(x, y, Color.FromArgb(YCbCrTab[YCbCrTabPtr + r], YCbCrTab[YCbCrTabPtr + g], YCbCrTab[YCbCrTabPtr + b]));

                    /// Advance pointers

                    if (derivedHeight != 0)
                    {
                        if ((x & 1) != 0)
                        {
                            CbPtr++;
                            CrPtr++;
                        }
                    }
                    else
                    {
                        CbPtr++;
                        CrPtr++;
                    }

                    YTab++;
                    rgbPtr += 3;
                }
            }
            //reset variables
            waveLetBuffersReady = false;
            YCbCrTabReady = false;
            return image;
        }
    
    }

}

