using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ShandalarImageToolbox
{
    public class Vlc {
        public static int bitsLeft;
        public static byte[] bitstream;
        public static int bitstreamStart;
        public static int bitstreamPtr;
        public static int bitstreamLimit;
        public static uint bitsDword;
        public static uint[] bitsDwordMask = new uint[32];


        public static int Tab1Count;

       public struct Tab1Entry
        {
            public int field_0;
            public int field_1;
        } 

public static Tab1Entry[] Tab1 = new Tab1Entry[0x2000];

        public static int SavedIndexCount;
        public static int[] SavedIndexTab;

        public struct Tab2Entry
        {
           public int field_0;
           public int field_1;
           public int field_2;
        }

public static Tab2Entry[] Tab2 = new Tab2Entry[0x100];

        //////////////////////////////////////////////////////////////////////////////

        public static int GetBits(int bits)           // 0x506AD0
        {
            uint value;

            if (bits <= bitsLeft)
            {
                value = bitsDword & bitsDwordMask[32 - bits];
                bitsDword >>= bits;

                bitsLeft -= bits;
                return (int)value;
            }
            else
            {
                value = bitsDword;

                bits -= bitsLeft;
                int bytesLeft = (int)(bitstreamPtr - bitstreamStart + 4);

                if (bytesLeft < bitstreamLimit)
                {
                    
                    bitsDword = BitConverter.ToUInt32(bitstream,bitstreamPtr);
                    bitstreamPtr += 4;

                    value |= (bitsDword & bitsDwordMask[32 - bits]) << bitsLeft;

                    bitsDword >>= bits;
                    bitsLeft = 32 - bits;

                    return (int)value;
                }
                else
                {
                    return -1;
                }
            }
        }

        public static int GetNextBit()                    // 0x506BF0
        {
            int bitOut = 0;

            if (bitsLeft == 0)
            {
                if ((bitstreamStart - bitstreamPtr) < bitstreamLimit)
                {
                    bitsDword = BitConverter.ToUInt32(bitstream,bitstreamPtr);
                    bitstreamPtr += 4;
                    bitsLeft = 32;
                }
                else
                {
                    return -1;
                }
            }

            if ((bitsDword & 1) != 0)
                bitOut = 1;

            bitsDword >>= 1;
            bitsLeft--;

            return bitOut;
        }

       public static void DumpTab1(int entries)
        {
            for (int i = 0; i < entries; i++)
            {
                Console.WriteLine("{0}: [{1}, {2}]", i, Tab1[i].field_0, Tab1[i].field_1);
            }
        }

        public static void DumpTab2()
        {
            for (int i = 0; i < 256; i++)
            {
                Console.WriteLine("{0}: [0x{1}, 0x{2}, 0x{3}]", i,
                    Tab2[i].field_0.ToString("X8"),
                    Tab2[i].field_1.ToString("X8"),
                    Tab2[i].field_2.ToString("X8"));
            }
        }

        public static int Vlc_GenTab2(int count)          // 0x505EB2
        {
            int var_6C = 0;

            int[] powersOfTwo = { 1, 2, 4, 8, 0x10, 0x20, 0x40, 0x80, 0x100, 0x200, 0x400 };
            int[] var_EC = new int[32];

            uint i = 0;
            while (i < 32)
            {
                var_EC[i] = 1;
                i++;
            }

            int var_8 = count - 1;

            int[] var_38 = new int[100];
            var_38[0] = var_8;

            /// Loop

            while (var_EC[0] >= 0)
            {
                /////

                int var_FC = var_6C++;
                int var_4;

                if (var_EC[var_FC] != 0)
                {
                    var_4 = Tab1[var_8].field_0;
                }
                else
                {
                    var_4 = Tab1[var_8].field_1;
                }

                ////

                var_8 = var_4 - SavedIndexCount;
                var_38[var_6C] = var_8;

                if (var_8 >= 0)
                {
                    if (var_6C == 8)
                    {
                        int var_F8 = 0;
                        int var_C = 0;

                        while (var_F8 < var_6C)
                        {
                            var_C |= var_EC[var_F8] << var_F8;
                            var_F8++;
                        }

                        Tab2[var_C].field_0 = int.MaxValue;
                        Tab2[var_C].field_1 = var_6C;
                        Tab2[var_C].field_2 = var_8;

                        var_EC[var_6C] = 1;
                        var_6C--;
                        var_EC[var_6C]--;
                        var_8 = var_38[var_6C];
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    int var_F0 = 0;
                    int var_C = 0;

                    while (var_F0 < var_6C)
                    {
                        var_C |= var_EC[var_F0] << var_F0;
                        var_F0++;
                    }

                    //// 

                    i = 0;

                    while (i < powersOfTwo[8 - var_6C])
                    {
                        int var_F4 = (int)(i << var_6C);

                        Tab2[var_F4 | var_C].field_0 = SavedIndexTab[var_4];
                        Tab2[var_F4 | var_C].field_1 = var_6C;
                        Tab2[var_F4 | var_C].field_2 = -1;

                        i++;
                    }

                    ////

                    var_EC[var_6C] = 1;
                    var_6C--;
                    var_EC[var_6C]--;
                    var_8 = var_38[var_6C];
                }

                /// Loop

                while (var_EC[0] >= 0)
                {
                    if (var_EC[var_6C] < 0)
                    {
                        var_EC[var_6C] = 1;
                        var_6C--;
                        var_EC[var_6C]--;
                        var_8 = var_38[var_6C];
                    }
                    else
                        break;
                }

            }

            return 0;
        }

        public static int Vlc_GenTabs(byte[] data,int dataOffset, int[] indexTab, int indexCount)            // 0x505D9A
        {
            int bitsProcessed = 13;

            int i = 0;
            while (i < 32)
            {
                bitsDwordMask[i] = 0xFFFFFFFF >> i;
                i++;
            }

            /// Setup bitstream
            bitstream = data;
            bitstreamStart = bitstreamPtr = dataOffset;
            bitstreamLimit = 100000;
            bitsDword = 0;
            bitsLeft = 0;

            Tab1Count = GetBits(13);

            /// Loop

            i = 0;
            while (i < Tab1Count)
            {
                Tab1[i].field_0 = GetBits(13);
                Tab1[i].field_1 = GetBits(13);

                bitsProcessed += 26;
                i++;
            }

            //DumpTab1(Tab1Count);

            /// Step 1

            SavedIndexCount = indexCount;
            SavedIndexTab = indexTab;

            Vlc_GenTab2(Tab1Count);

            //DumpTab2();

            return (bitsProcessed + 7) / 8;
        }

       public static void Vlc_DecompressChunk(byte[] data,int dataOffset,ref byte[] outArray,int startOffset,  int dataSize)                // 0x506453
        {
            int index = startOffset;
            bitstream = data;
            bitstreamPtr = dataOffset;
            bitstreamStart = dataOffset;
            bitstreamLimit = dataSize;

            if ((dataOffset & 3) != 0)
            {
                /// Unaligned pointer

                int unalignedBytes = 4 - ((dataOffset & 3) != 0 ? 1 : 0);

                bitsLeft = unalignedBytes * 8;

                uint mask = 0xFFFFFFFF >> (32 - bitsLeft);
                bitsDword = (BitConverter.ToUInt32(data, dataOffset)) & mask;

                bitstreamPtr += unalignedBytes;
            }
            else
            {
                bitsLeft = 0;
                bitsDword = 0;
            }

            int var_C = GetBits(8);

            while (var_C != -1)
            {
                if (Tab2[var_C].field_0 == (int)0x7FFFFFFF)
                {
                    int var_14 = (int)Tab2[var_C].field_2;
                    int var_10;

                    do
                    {
                        int var_18 = GetNextBit();

                        if (var_18 == -1)
                            goto loc_5066BB;

                        if (var_18 != 0)
                        {
                            var_10 = Tab1[var_14].field_0;
                        }
                        else
                        {
                            var_10 = Tab1[var_14].field_1;
                        }

                        var_14 = var_10 - SavedIndexCount;

                    } while (var_14 >= 0);

                    if (var_10 != 0)
                    {
                        /// Set as SavedIndex

                        outArray[index] = (byte)SavedIndexTab[var_10];
                        outArray[index + 1] = (byte)(SavedIndexTab[var_10] >> 8);
                        outArray[index + 2] = (byte)(SavedIndexTab[var_10] >> 16);
                        outArray[index + 3] = (byte)(SavedIndexTab[var_10] >> 24);
                        index+=4;
                    }
                    else
                    {
                        /// Clear

                        int zeroes = GetBits(10);

                        if (zeroes >= 0)
                        {
                            for(int i = index; i < index + zeroes * 4; i++)
                            {
                                outArray[i] = 0;
                            }
                            index += zeroes*4;
                        }
                    }

                loc_5066BB:
                    var_C = GetBits(8);
                }
                else
                {
                    int var_24 = Tab2[var_C].field_1;

                    if (Tab2[var_C].field_0 == int.MinValue)
                    {
                        int var_28 = GetBits(var_24 + 2);
                        if (var_28 < 0)
                            break;

                        var_28 = (var_28 << (8 - var_24)) | (var_C >> (byte)var_24);

                        /// Clear
                        for (int i = index; i < index + var_28 * 4; i++)
                        {
                            outArray[i] = 0;
                        }
                        index += var_28*4;

                        var_C = GetBits(8);
                    }
                    else
                    {
                        /// Set as F0
                       
                        outArray[index] = (byte)Tab2[var_C].field_0;
                        outArray[index + 1] = (byte)(Tab2[var_C].field_0 >> 8);
                        outArray[index + 2] = (byte)(Tab2[var_C].field_0 >> 16);
                        outArray[index + 3] = (byte)(Tab2[var_C].field_0 >> 24);
                        index +=4;

                        var_C >>= (byte)var_24;

                        int var_20 = GetBits(var_24);
                        if (var_20 == -1)
                            break;

                        var_C |= var_20 << (8 - var_24);
                    }
                }
            }
        }

       

        public static byte[] VlcDecompress(byte[] srcBytes)          // 0x4928AF
        {
            int decompressedSize = (int)BitConverter.ToUInt32(srcBytes, 0x90) + 20000;
            byte[] result = new byte[decompressedSize];
            /// Block 1

            int ecx = (int)BitConverter.ToUInt32(srcBytes, 0x28) - 1;
            ecx = (ecx == 0) ? 1 : 2;

            int width = (int)BitConverter.ToUInt32(srcBytes, 0x1c);
            int var_30 = width / ecx;
            int var_2C = var_30;

            /// Block 2

            ecx = BitConverter.ToUInt32(srcBytes, 0) == 0 ? 1 : 2;
            int var_18 = var_2C / ecx;
            int var_10 = var_18;

            byte[] dataPtr = srcBytes.Skip(0x9c).ToArray();           // skip header
            int smallTableSize = (int)BitConverter.ToUInt32(srcBytes, 0x24);

            int eax = var_18 * var_10;
            ecx = var_30 * var_2C;
            int var_C = ecx + 2 * eax;

            /// Step 1
            int dataOffset = 0;
            int bigIndexCount = (int)BitConverter.ToUInt32(dataPtr, dataOffset);
            dataOffset += 4;

            int[] bigIndexTabPtr = GeneralUtilityFunctions.ByteArrayToIntArray(dataPtr, dataOffset);
            bigIndexTabPtr[0] = int.MinValue;

            dataOffset += bigIndexCount * 4;        // Skip large index table

            dataOffset += Vlc_GenTabs(dataPtr,dataOffset, bigIndexTabPtr, bigIndexCount);

            /// Loop

            int pieceNum = 0;        /// piece num
            
            while (pieceNum < BitConverter.ToUInt32(srcBytes, 0x28))
            {
                int size;

                /// Calculate pointers

                int tableSize = smallTableSize * smallTableSize * 4;
                int var_24_offset = (var_C + 0x40) * pieceNum * 4;
                int var_38_offset = var_24_offset + var_30 * var_2C * 4 + 0x80;
                int var_34_offset = var_38_offset + var_18 * var_10 * 4 + 0x80;

                /// Small index table + data 1

                Array.Copy(dataPtr, dataOffset, result, var_24_offset, tableSize);
                var_24_offset += tableSize;
                dataOffset += tableSize;

                size = (int)BitConverter.ToUInt32(srcBytes, pieceNum * 4 + 0x5c);
                Vlc_DecompressChunk(dataPtr, dataOffset,ref result, var_24_offset, size);
                dataOffset += size;

                /// Small index table + data 2

                Array.Copy(dataPtr, dataOffset, result, var_38_offset,tableSize);
                var_38_offset += tableSize;
                dataOffset += tableSize;

                size = (int)BitConverter.ToUInt32(srcBytes, pieceNum * 4 + 0x6c);
                Vlc_DecompressChunk(dataPtr, dataOffset,ref result, var_38_offset, size);
                dataOffset += size;

                /// Small index table + data 3

                Array.Copy(dataPtr, dataOffset, result, var_34_offset, tableSize);
                var_34_offset += tableSize;
                dataOffset += tableSize;

                size = (int)BitConverter.ToUInt32(srcBytes, pieceNum * 4 + 0x7c);
                
                Vlc_DecompressChunk(dataPtr,dataOffset, ref result, var_34_offset, size);
                dataOffset += size;

                pieceNum++;
            }

            return result;
        }
    }
}