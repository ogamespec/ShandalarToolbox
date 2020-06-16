using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShandalarImageToolbox
{
    public static class GeneralUtilityFunctions
    {
        public static int[] ByteArrayToIntArray(byte[] buf, int startOffset)
        {
            int[] intArr = new int[(buf.Length - startOffset)/ 4];
            int offset = startOffset;
            for (int i = 0; i < intArr.Length; i++)
            {
                intArr[i] = buf[0 + offset] + (buf[1 + offset] << 8) + (buf[2 + offset] << 16) + (buf[3 + offset] << 24);
                offset += 4;
            }
            return intArr;
        }
    }
}
