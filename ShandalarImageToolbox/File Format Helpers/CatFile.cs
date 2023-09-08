using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ShandalarImageToolbox
{
    public class CatFile
    {
        public string name;
        public UInt32 hash;
        public byte[] data;
    }

    internal class CatDirEntry
    {
        public UInt32 hash;
        public UInt32 fileOffset;
        public UInt32 size;
    }

    class Cat
    {
        public List<CatFile> files = new List<CatFile>();

        public Cat(string filename)
        {
            /// Load directory

            byte[] catImage = File.ReadAllBytes(filename);

            int filesCount = GetEntriesCount(catImage);

            /// Load files

            for(int i=0; i< filesCount; i++)
            {
                CatFile file = LoadCatFile(catImage, i);
                files.Add(file);
            }
        }

        /// Read dword (Little-endian)

        private UInt32 GetDword (byte[] catImage, int offset)
        {
            UInt32 value =
                (UInt32)(catImage[offset + 3] << 24) |
                (UInt32)(catImage[offset + 2] << 16) |
                (UInt32)(catImage[offset + 1] << 8) |
                (UInt32)(catImage[offset + 0]);
            return value;
        }

        /// Get direntry count

        private int GetEntriesCount (byte [] catImage)
        {
            return (int)GetDword(catImage, 0);
        }

        /// Get Direntry

        private CatDirEntry GetDirEntry (byte[] catImage, int entryNum)
        {
            CatDirEntry entry = new CatDirEntry();

            int entryOffset = entryNum * 12 + 4;

            entry.hash = GetDword(catImage, entryOffset);
            entry.fileOffset = GetDword(catImage, entryOffset + 4);
            entry.size = GetDword(catImage, entryOffset + 8);

            return entry;
        }

        /// Load file

        private CatFile LoadCatFile (byte[] catImage, int entryNum)
        {
            CatFile file = new CatFile();

            CatDirEntry entry = GetDirEntry(catImage, entryNum);

            file.hash = entry.hash;
            file.data = new byte[entry.size];
            
            for (int i=0; i<entry.size; i++)
            {
                file.data[i] = catImage[entry.fileOffset + i];
            }
            int offset = 8;
            while (file.data[offset] != 0 && file.data[offset] != 2)
            {
                file.name += (char)file.data[offset];
                offset++;
            }
            if (file.name[0] == 'c' || file.name[0] == 'C') file.name = file.name.Substring(1);


            return file;
        }

    }
}
