/// Microprose MTG .TR palette decoder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Text.RegularExpressions;

namespace ShandalarImageToolbox
{
    class PalDecoder
    {
        public Color[] Palette = new Color[256];

        private void ResetPalette()
        {
            for(int i=0; i<256; i++)
            {
                Palette[i] = Color.Transparent;
            }
        }

        public PalDecoder (string [] text)
        {
            int index = 0;

            ResetPalette();

            foreach (string line in text)
            {
                int format = line.Count(f => f == '-');

                Regex regex;

                if (format == 1)         // Non-indexed format
                {
                    regex = new Regex(@"(?<r>[\d]+[\s]*)(?<g>[\s]*[\d]+[\s]*)(?<b>[\s]*[\d]+[\s]*[-])");
                }
                else if (format == 2)        // Indexed format
                {
                    regex = new Regex(@"(?<index>[\d]+[\s]*[-]{1})(?<r>[\s]*[\d]+[\s]*)(?<g>[\s]*[\d]+[\s]*)(?<b>[\s]*[\d]+[\s]*[-])");
                }
                else
                {
                    throw new Exception("Invalid palette format!");
                }

                var matches = regex.Matches(line);

                byte rValue, gValue, bValue;

                if (matches.Count == 1)
                {
                    string rStr, gStr, bStr;

                    if (format == 1)
                    {
                        rStr = matches[0].Groups[1].Value.Replace("-", "").Trim();
                        gStr = matches[0].Groups[2].Value.Replace("-", "").Trim();
                        bStr = matches[0].Groups[3].Value.Replace("-", "").Trim();

                        index++;
                    }
                    else if (format == 2)
                    {
                        string indexStr = matches[0].Groups[1].Value.Replace ("-", "").Trim();
                        index = Convert.ToInt16(indexStr);

                        rStr = matches[0].Groups[2].Value.Replace("-", "").Trim();
                        gStr = matches[0].Groups[3].Value.Replace("-", "").Trim();
                        bStr = matches[0].Groups[4].Value.Replace("-", "").Trim();
                    }
                    else
                    {
                        rStr = gStr = bStr = index.ToString();
                        index++;
                    }

                    rValue = Convert.ToByte(rStr);
                    gValue = Convert.ToByte(gStr);
                    bValue = Convert.ToByte(bStr);
                    Palette[index] = Color.FromArgb(rValue, gValue, bValue);
                }
                else
                {
                    throw new Exception("Invalid palette entry!");
                }
            }

            for (int i = 0; i < 256; i++)
            {
                if (Palette[i] == Color.Transparent) Palette[i] = Color.FromArgb(i, i, i);
            }
        }


    }

}
