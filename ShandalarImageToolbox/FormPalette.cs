using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShandalarImageToolbox
{
    public partial class FormPalette : Form
    {
        private const int cellSize = 16;

        public FormPalette(Color [] palette)
        {
            InitializeComponent();

            pictureBox1.Image = BuildPaletteImage(palette);
        }

        private Image BuildPaletteImage (Color [] palette)
        {
            Bitmap bitmap = new Bitmap(16 * cellSize, 16 * cellSize);

            int colorIndex = 0;

            for (int cellY=0; cellY<16 * cellSize; cellY+=cellSize)
            {
                for (int cellX=0; cellX<16 * cellSize; cellX+=cellSize)
                {
                    Color color = palette[colorIndex];

                    for(int y=0; y<cellSize; y++)
                    {
                        for(int x = 0; x<cellSize; x++)
                        {
                            bitmap.SetPixel(cellX + x, cellY + y, color);
                        }
                    }

                    colorIndex++;
                }
            }

            return bitmap;
        }

    }
}
