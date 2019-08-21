

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;



namespace PicDecode{
    public class SprDecoder{
        
        public static Bitmap[] GetSprites (byte[] data, Color[] palette){
            List<Bitmap> bitmaps = new List<Bitmap>();
            int offset = 0;
            int imageIndex = 0;
            while(offset < data.Length){
                int startOffset = offset;
                UInt32 imageDataSize = BitConverter.ToUInt32(data, offset);
                if (imageDataSize == 0xFFFFFFFF) break;
                offset += 4;
                int width = BitConverter.ToUInt16(data,offset);
                offset += 2;
                int height = BitConverter.ToUInt16(data,offset);
                offset += 2;
                int unknown1 = BitConverter.ToUInt16(data,offset);
                offset += 2;
                int unknown2 = BitConverter.ToUInt16(data, offset);
                offset += 2;
                int numberOfEmptyLinesAbove = BitConverter.ToUInt16(data, offset);
                offset += 2;
                int cutoffOffsetY = BitConverter.ToUInt16(data, offset); //how many lines after the transparent space in the image to start setting pixels as transparent
                offset += 2;
                Bitmap bitmap = new Bitmap(width,height);
                bitmap.MakeTransparent();
                Console.WriteLine("Image " + imageIndex + " information: offset: 0x" + startOffset.ToString("X") +  ", Image data size: 0x" + imageDataSize.ToString("X") +
                    ", width: " + width + ", height: " + height + ", unknown1: 0x" +
                    unknown1.ToString("X") + ", unknown2: 0x" + unknown2.ToString("X") + ",number of top empty lines: " + numberOfEmptyLinesAbove + ", cutoff y offset: " + cutoffOffsetY) ;
                bool lineHasTransparentPixels; //set to true when the line has intentionally transparent pixels
                int numberOfPixelsInData = 0;
                for (int y = 0; y < height; y++)
                {
                    if (y < numberOfEmptyLinesAbove || y >= cutoffOffsetY + numberOfEmptyLinesAbove)
                    {
                        
                        for (int x = 0; x < width; x++)
                        {
                           bitmap.SetPixel(x, y, Color.Transparent);
                        }
                        continue;
                        
                    }
                    //Console.WriteLine("Current offset: 0x" + offset.ToString("X") + ", y: " + y);
                    while (data[offset] == 0xFF) offset++;
                    if (offset >= startOffset + (int)imageDataSize)
                    {
                        for (int i = y; i < height; i++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                bitmap.SetPixel(x, i, Color.Transparent);
                            }
                        }
                        break;
                    }
                    int transparentPixelsAmount = data[offset];
                    offset++;
                    int unknown3 = data[offset];
                    offset++;
                    lineHasTransparentPixels = false;
                   
                   
                    if (unknown3 != 0xFE && unknown3 != 0xFF)
                    {
                        lineHasTransparentPixels = true;
                        numberOfPixelsInData = unknown3;
                    }
                    else
                    {
                        numberOfPixelsInData = data[offset];
                        offset++;
                    }
                    for (int x = 0; x < width; x++)
                    {
                        if (data[offset] == 0xFF && !lineHasTransparentPixels) offset++;
                        
                        if (x < transparentPixelsAmount || x >= numberOfPixelsInData + transparentPixelsAmount)
                        {
                           bitmap.SetPixel(x, y, Color.Transparent);
                        }
                        else if (lineHasTransparentPixels && data[offset] == 0)
                        {
                           bitmap.SetPixel(x, y, Color.Transparent);
                            offset++;
                        }
                        else
                        {
                            bitmap.SetPixel(x, y, palette[data[offset]]);

                            offset++;
                        }


                    }
                
                }

                offset = startOffset + (int)imageDataSize;
                bitmaps.Add(bitmap);
                imageIndex++;
            }
            return bitmaps.ToArray();

        }
    }
}