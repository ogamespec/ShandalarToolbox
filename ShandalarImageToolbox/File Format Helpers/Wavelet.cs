using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShandalarImageToolbox
{
    public class Wavelet
    {
        /*
        public static bool waveLetBuffersReady;
        public static int[] waveLetBuffer_1;
        public static int[] waveLetBuffer_2;

        public static void CardWaveletDecode_Sub7(                            // 0x492410
            int* tab1,
            int* tab2,
            int* tmp,
            int arg_C,
            int arg_10,
            int arg_14,
            int arg_18
        )
        {
            for (int i = 0; i < arg_10; i++)
            {
                int* var_C = tab1;
                int* var_10 = &tab1[arg_C];
                int* var_8 = &tmp[arg_18 * 2];
                tab1++;

                while (tab1 < var_10)
                {
                    *var_8 = *tab1 + *tab2;
                    var_8[arg_18] = *tab1 - *tab2;

                    tab1++;
                    tab2++;
                    var_8 += arg_18 * 2;
                }

                *tmp = *var_C + *tab2;
                tmp[arg_18] = *var_C - *tab2;

                tmp++;
                tab2++;
            }
        }

        public static void CardWaveletDecode_Sub8(                    // 0x4924E5
            int* buf1,
            int* buf2,
            int* outTab,
            int arg_C,
            int arg_10,
            int arg_14,
            int arg_18)
        {
            for (int i = 0; i < arg_10; i++)
            {
                int* var_C = buf1;
                int* var_10 = &buf1[arg_C];
                int* var_8 = &outTab[arg_18 * 2];
                buf1++;

                while (buf1 < var_10)
                {
                    *var_8 = (*buf1 + *buf2) / 2;
                    var_8[arg_18] = (*buf1 - *buf2) / 2;

                    buf1++;
                    buf2++;
                    var_8 += arg_18 * 2;
                }

                *outTab = (*var_C + *buf2) / 2;
                outTab[arg_18] = (*var_C - *buf2) / 2;

                outTab++;
                buf2++;
            }
        }

        public static void WaveletDecode(int* ctab, int width, int tabSize)                   // 0x4922CB
        {
            int* buf1, *buf2;

            if (!waveLetBuffersReady)
            {
                buf1 = waveLetBuffer_1 = new int[0x10000];
                buf2 = waveLetBuffer_2 = new int[0x10000];
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
                    ctab,
                    &ctab[size * size],
                    buf1,
                    size,
                    size,
                    2 * size,       // not used
                    size);

                CardWaveletDecode_Sub7(
                    &ctab[size * size * 2],
                    &ctab[size * size * 3],
                    buf2,
                    size,
                    size,
                    2 * size,       // not used
                    size);

                CardWaveletDecode_Sub8(
                    buf1,
                    buf2,
                    ctab,
                    size,
                    2 * size,
                    2 * size,       // not used
                    2 * size);

                size *= 2;
            }
        }

        ///////////////////////////////////////////////////////////

        public static bool YCbCrTabReady;
        public static byte[] YCbCrTab = new byte[0x400 + 0x1c00];
        public static byte* YCbCrTabPtr = &YCbCrTab[0x400];

        public static byte[] Decode_YCbCrToRGB(                 // 0x4925E6
            byte* rgbPtr,                    // always 0
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

