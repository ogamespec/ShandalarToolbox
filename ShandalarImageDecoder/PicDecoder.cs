/// Microprose MTG .PIC Decoder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShandalarImageDecoder
{
    class PicDecoder
    {
        public int width;
        public int height;
        public int compressedSize;

        public bool bcdPacked;

        private byte[] savedData;
        private int dataOffset = 0;

        public PicDecoder(byte [] data)
        {
            savedData = data;
            dataOffset = 0;

            string magic = Encoding.UTF8.GetString(data, 0, 2);
            dataOffset += 2;
            if (magic[0] != 'X' && magic[0] != 'M') 
                throw new Exception("Invalid PIC format!");
            bool hasPalette = false;
            if (magic[0] == 'M')
            {
                hasPalette = true;
                int paletteDataLength = BitConverter.ToUInt16(data,dataOffset);
                dataOffset += paletteDataLength + 2;
                dataOffset += 2;
            }
            /// Engine also support other formats, but we don't support it since all .pics are "X" format
            bcdPacked = magic == "X1";

            compressedSize = GetWord();
            if (magic == "M1") compressedSize = data.Length % 65536 - 4;
            width = GetWord();
            height = GetWord();

            Console.WriteLine("width: {0}, height: {1}, compressed size: 0x{2}, bcdPacked: {3}, hasPalette: {4}",
                width.ToString(), height.ToString(), compressedSize.ToString("X"), bcdPacked.ToString(), hasPalette.ToString());

            PrepareContext();
        }

        private UInt16 GetWord()
        {
            byte lo = savedData[dataOffset++];

            /// Unaligned access should return zero

            byte hi = 0;
            if (dataOffset < savedData.Length)
                hi = savedData[dataOffset++];

            return (UInt16)((hi << 8) | lo);
        }

        /// <summary>
        /// Internal context
        /// </summary>

        private int RepeatCount;
        private byte RepeatByte;
        private byte[] SomeBuffer;
        private int SomeBufferOffset;
        private UInt16 MagicValueWord;
        private byte MagicValueByte;
        private int bitPointer;
        private int onesCounter;        /// Significant bit count
        private UInt32 bitMask;
        private byte[] LUT;
        private int dword_5360E4;       /// 0x100
        private int savedIndex = 0;
        private byte savedByte = 0;

        public bool passedFirstByte = false;
        private void PrepareContext()
        {
            RepeatCount = 0;
            RepeatByte = 0;

            SomeBuffer = new byte[0x10000];
            SomeBufferOffset = -1;

            MagicValueWord = GetWord();

            MagicValueByte = (byte)(MagicValueWord & 0xff);
            if (MagicValueByte > 11)
            {
                MagicValueByte = 11;
            }

            MagicValueWord &= 0xff00;
            MagicValueWord |= MagicValueByte;


            bitPointer = 8;

            LUT = new byte[(1 << MagicValueByte) * 3];
            
            PrepareBuffer();
        }

        private void PrepareBuffer()
        {
            onesCounter = 9;
            bitMask = 0x1FF;

            dword_5360E4 = 0x100;

            /// Fill FF FF 00 pattern

            for(int i=0; i < (1 << MagicValueByte); i++)
            {
                SetLutIndex(i, 0xffff);
            }

            /// Fix first 256 entries by FF FF xx pattern (where xx = 00 ... FF)

            for (int i = 0; i < 0x100; i++)
            {
                SetLutValue(i, (byte)i);
            }
        }

        /// <summary>
        /// Sequiental decoder
        /// </summary>
        /// <param name="amount"></param>
        
        public void DecodeImage(byte[,] data)
        {
            byte[] line;
            for (int y = 0; y < data.GetLength(1); y++)
            {
                line = new byte[data.GetLength(0)];
                try
                {
                    DecodeNextBytes(line);
                }
                catch(IndexOutOfRangeException e)
                {
                    Console.WriteLine("The image data is shorter than the given dimensions in the PIC file.");
                    return;   
                }
                for (int x = 0; x < data.GetLength(0); x++) data[x, y] = line[x];
            }
            
        }

        public void DecodeNextBytes(byte [] outPut, bool debug=false)
        {
            int opcount;

            if (bcdPacked)
            {
                opcount = (outPut.Length + 1) / 2;
            }
            else
            {
                opcount = outPut.Length;
            }

            for (int i = 0; i < opcount; i++)
            {
                byte value;

                /// Fetch next byte

                if (RepeatCount != 0)
                {
                    value = RepeatByte;
                    RepeatCount--;
                }
                else
                {
                    value = NextRun();

                    if (value == 0x90)
                    {
                        value = NextRun();

                        if (value != 0)
                        {
                            RepeatCount = value - 1;

                            value = RepeatByte;
                            RepeatCount--;
                        }
                        else
                        {
                            RepeatByte = value = 0x90;
                        }
                    }
                    else
                    {
                        RepeatByte = value;
                    }
                }

                /// Output byte
                    if (bcdPacked)
                    {
                        /// Unpack BCD as uint16

                        byte hiPart = (byte)(value >> 4);
                        byte lowPart = (byte)(value & 0xf);

                        outPut[2 * i + 1] = hiPart;
                        outPut[2 * i] = lowPart;

                        if (debug)
                        {
                            Console.Write(lowPart.ToString("X2") + " ");
                            Console.Write(hiPart.ToString("X2") + " ");
                        }
                    }
                    else
                    {
                        outPut[i] = value;

                        if (debug)
                            Console.Write(value.ToString("X2") + " ");
                    }
            }

            if (debug)
                Console.Write("\n");
        }

        private byte NextRun ()
        {
            if ( SomeBufferOffset == -1)
            {
                int b = MagicValueWord >> (16 - bitPointer);
                int c = bitPointer;

                /// Loop 1

                while (c < onesCounter)
                {
                    MagicValueWord = GetWord();
                    b |= (MagicValueWord << c);
                    c += 16;
                }

                /// After Loop 1

                bitPointer = c - onesCounter;

                int oldIndex = (int)(b & bitMask);
                int newIndex = oldIndex;

                if (oldIndex >= dword_5360E4)
                {
                    newIndex = dword_5360E4;
                    oldIndex = savedIndex;
                    PushValue(savedByte);
                }

                /// Loop 2

                while (true)
                {
                    int index = GetLutIndex(oldIndex) + 1;

                    if (index != 0x10000)
                    {
                        PushValue(GetLutValue(oldIndex));
                        oldIndex = index - 1;
                    }
                    else break;
                }

                /// After Loop 2

                savedByte = GetLutValue(oldIndex);
                PushValue(savedByte);
                SetLutValue(dword_5360E4, savedByte);
                SetLutIndex(dword_5360E4, savedIndex);

                dword_5360E4++;
                if (dword_5360E4 > bitMask)
                {
                    onesCounter++;
                    bitMask = (bitMask << 1) | 1;
                }

                if (onesCounter > MagicValueByte)
                {
                    PrepareBuffer();
                    newIndex = 0;
                }

                savedIndex = newIndex;
            }

            return PopValue();
        }

        private int GetLutIndex(int id)
        {
            int offset = id * 3;
            UInt16 word = (UInt16)((LUT[offset + 1] << 8) |
                LUT[offset]);
            return (int)word;
        }

        private byte GetLutValue(int id)
        {
            return LUT[id * 3 + 2];
        }

        private void SetLutIndex(int id, int newId)
        {
            int offset = id * 3;
            UInt16 word = (UInt16)newId;
            LUT[offset + 1] = (byte)(word >> 8);
            LUT[offset] = (byte)(word & 0xff);
        }

        private void SetLutValue (int id, byte value)
        {
            LUT[id * 3 + 2] = value;
        }

        private void PushValue (byte value)
        {
            SomeBufferOffset++;
            SomeBuffer[SomeBufferOffset] = value;
        }

        private byte PopValue ()
        {
            if (SomeBufferOffset < 0)
                throw new Exception("SomeBufferOffset < 0");
            return SomeBuffer[SomeBufferOffset--];
        }

    }
}
