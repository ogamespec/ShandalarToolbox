using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShandalarImageToolbox
{
    public class Wavelet
    {
        
        
        public static bool waveLetBuffersReady;
        public static byte[] waveLetBuffer_1;
        public static byte[] waveLetBuffer_2;
        public static int buf1Index, buf2Index;
        public static int savedCtab;
        public static byte[] savedData;
        public static int savedDataOffset;

        public static void CardWaveletDecode_Sub7(                            // 0x492410
            byte[] data,
            int tab1,
            int tab2,
            ref byte[] tmp,
            ref int tmpIndex,
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
                    for (int j = 0; j < 4; j++) tmp[var_8+j] = (byte)(savedData[tab1+j] + savedData[tab2+j]);
                    for (int j = 0; j < 4; j++) tmp[var_8+arg_18+j] = (byte)(savedData[tab1+j] - savedData[tab2+j]);

                    tab1+= 4;
                    tab2+= 4;
                    var_8 += arg_18 * 2 * 4;
                }

                for (int j = 0; j < 4; j++) tmp[tmpIndex+j] = (byte)(savedData[var_C+j] + savedData[tab2+j]);
                for(int j = 0; j < 4; j++) tmp[tmpIndex + arg_18 + j] = (byte)(savedData[var_C + j] - savedData[tab2 + j]);

                tmpIndex +=4;
                tab2+=4;
            }
        }

        public static void CardWaveletDecode_Sub8(                    // 0x4924E5
            ref byte[] buf1,
            ref byte[] buf2,
            int outTab,
            int arg_C,
            int arg_10,
            int arg_14,
            int arg_18)
        {
            for (int i = 0; i < arg_10; i++)
            {
                int var_C = buf1Index;
                int var_10 = buf1Index + arg_C;
                int var_8 = outTab + arg_18 * 2;
                buf1Index++;

                while (buf1Index < var_10)
                {
                    for (int j = 0; j < 4; j++) savedData[var_8+j] = (byte)((buf1[buf1Index+j] + buf2[buf2Index+j]) / 2);
                    for (int j = 0; j < 4; j++) savedData[var_8 + arg_18+j] = (byte)((buf1[buf1Index+j] + buf2[buf2Index+j]) / 2);

                    buf1Index++;
                    buf2Index++;
                    var_8 += arg_18 * 2;
                }

                for (int j = 0; j < 4; j++) savedData[outTab+j] = (byte)((buf1[var_C+j] + buf2[buf2Index+j]) / 2);
                for (int j = 0; j < 4; j++) savedData[outTab+arg_18+j] = (byte)((buf1[var_C+j] - buf2[buf2Index+j]) / 2);

                outTab+=4;
                buf2Index+=4;
            }
            savedCtab = outTab;
        }

        public static void WaveletDecode(ref byte[] data, int ctab, int width, int tabSize)                   // 0x4922CB
        {
            byte[] buf1, buf2;
            savedData = data;
            if (!waveLetBuffersReady)
            {
                buf1 = waveLetBuffer_1 = new byte[0x40000]; //new int[0x10000];
                buf2 = waveLetBuffer_2 = new byte[0x40000]; //new int[0x10000];
                buf1Index = 0;
                buf2Index = 0;
                waveLetBuffersReady = true;
            }
            else
            {
                buf1 = waveLetBuffer_1;
                buf2 = waveLetBuffer_2;
            }

            int size = tabSize;

            while (size < width)
            {
                CardWaveletDecode_Sub7(
                    data,
                    ctab,
                    size * size + ctab,
                    ref buf1,
                    ref buf1Index,
                    size,
                    size,
                    2 * size,       // not used
                    size);
                ctab = savedCtab;
                CardWaveletDecode_Sub7(
                    data,
                    size * size * 2 + ctab,
                    size * size * 3 + ctab,
                    ref buf2,
                    ref buf2Index,
                    size,
                    size,
                    2 * size,       // not used
                    size);
                ctab = savedCtab;

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
        public static int YCbCrTabPtr = 0x40;
        /*
        public static byte[] Decode_YCbCrToRGB(                 // 0x4925E6
            byte[] data,                    // always 0
            int* YTab,
            int width,
            int height,
            int* CbTab,
            int* CrTab,
            int derivedWidth,
            int derivedHeight
        )
        {
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
                        YCbCrTabPtr[i] = (byte)eax;
                    }
                    else
                    {
                        YCbCrTabPtr[i] = 0;
                    }

                    i++;
                }

                YCbCrTabReady = true;
            }

            /// Allocate RGB buffer

            byte* buf;

            if (rgbPtr)
            {
                buf = rgbPtr;
            }
            else
            {
                buf = rgbPtr = new byte[width * width * 3 + 0x10];       /// Should be width * height (?)
            }

            for (int y = 0; y < height; y++)
            {
                int* CbPtr;
                int* CrPtr;

                if (derivedHeight)
                {
                    CbPtr = &CbTab[(y / 2) * derivedWidth];
                    CrPtr = &CrTab[(y / 2) * derivedWidth];
                }
                else
                {
                    CbPtr = &CbTab[y * derivedWidth];
                    CrPtr = &CrTab[y * derivedWidth];
                }

                for (int x = 0; x < width; x++)
                {
                    int yval = *YTab;
                    int CbVal;
                    int CrVal;

                    ////

                    if (derivedHeight)
                    {
                        if (x & 1)          /// Lerp
                        {
                            int eax = width - x - 1;
                            eax = (eax == 0) ? -1 : 0;
                            eax = CbPtr[eax + 1];
                            CbVal = (CbPtr[0] + eax) / 2;

                            eax = width - x - 1;
                            eax = (eax == 0) ? -1 : 0;
                            eax = CrPtr[eax + 1];
                            CrVal = (CrPtr[0] + eax) / 2;
                        }
                        else
                        {
                            CbVal = CbPtr[0];
                            CrVal = CrPtr[0];
                        }
                    }
                    else
                    {
                        CbVal = CbPtr[0];
                        CrVal = CrPtr[0];
                    }

                    ////

                    int r = yval + CrVal + CrVal / 2 + CrVal / 8 - 0x333;
                    int b = yval + CbVal * 2 - 0x400;
                    int g = yval * 2 - yval / 4 - r / 2 - b / 4 - b / 16;

                    rgbPtr[0] = YCbCrTabPtr[b];         // B
                    rgbPtr[1] = YCbCrTabPtr[g];         // G
                    rgbPtr[2] = YCbCrTabPtr[r];         // R

                    /// Advance pointers

                    if (derivedHeight)
                    {
                        if (x & 1)
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

            return buf;
        }
        */
        
    
    }

}

