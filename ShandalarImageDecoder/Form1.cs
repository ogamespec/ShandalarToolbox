/// Microprose MTG .PIC and .TR Viewer

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Runtime.InteropServices;

namespace ShandalarImageDecoder
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        private Color[] palette = new Color[256];

        public enum ImageType
        {
            Pic,
            Spr
        }
        public ImageType loadedImageType;
        private byte[] SavedImageData;
        public Bitmap[] loadedImages;
        public string loadedImageFilename, imagePath;
        public int loadedImageIndex;


        public Form1()
        {
            InitializeComponent();

#if DEBUG
            AllocConsole();
#endif

            SetupGrayPalette();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        Random rnd = new Random();

        private Color RandomColor()
        {
            return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }

        private Color GrayColor(byte value)
        {
            return Color.FromArgb(value, value, value);
        }

        private void SetupGrayPalette ()
        {
            for(int i=0; i<256; i++)
            {
                palette[i] = GrayColor((byte)i);
            }
        }

        /// <summary>
        /// Load and decode .PIC image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadPicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ( openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                SavedImageData = File.ReadAllBytes(openFileDialog1.FileName);
                loadedImageFilename = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                imagePath = Path.GetDirectoryName(openFileDialog1.FileName);
                loadedImageType = ImageType.Pic;
                Text = "PIC View - " + Path.GetFileName(openFileDialog1.FileName);
                Console.WriteLine("Loaded file path: " + openFileDialog1.FileName);
                ShowPic(SavedImageData);
                exportToolStripMenuItem.Enabled = true;
                exportAllToolStripMenuItem.Enabled = true;
            }
        }

        private void ShowPic ( byte [] data)
        {
            int dataOffset = 0;

            string magic = Encoding.UTF8.GetString(data, 0, 2);
            dataOffset += 2;
            if (magic[0] == 'M')
            {
                bool is3fRange = magic == "M0";
                dataOffset += 2; //skip the palette data length value
                byte startIndex = data[dataOffset++];
                byte endIndex = data[dataOffset++];

                Color[] picFilePalette = new Color[256];
                for(int i = 0; i < endIndex - startIndex + 1; i++)
                {
                    int factor = is3fRange ? 4 : 1;
                    int r = factor * data[dataOffset++];
                    int g = factor * data[dataOffset++];
                    int b = factor * data[dataOffset++];

                    picFilePalette[i] = Color.FromArgb(r,g,b);
                }
                palette = picFilePalette;
            }

            PicDecoder decoder = new PicDecoder(data);

            /// Output image as picture box

            Bitmap bitmap = new Bitmap(decoder.width, decoder.height);

           byte[,] imageData = new byte[decoder.width,decoder.height];
            decoder.DecodeImage(imageData);

            for (int y = 0; y < decoder.height; y++)
            {
                for (int x = 0; x < decoder.width; x++)
                {
                    byte value = imageData[x,y];
                    bitmap.SetPixel(x, y, palette[value]);

                }
            }
            loadedImages = new Bitmap[] { bitmap };
            numericUpDown1.Value = 0;
            pictureBox1.Image = bitmap;
            label2.Text = "Width: " + pictureBox1.Width + "  " + "Height: " + pictureBox1.Height;
        }

        private void LoadSprToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                exportToolStripMenuItem.Enabled = true;
                byte[] data = File.ReadAllBytes(openFileDialog3.FileName);
                Text = "PIC View - " + Path.GetFileName(openFileDialog3.FileName);
                SavedImageData = data;
                loadedImageType = ImageType.Spr;
                loadedImageFilename = Path.GetFileNameWithoutExtension(openFileDialog3.FileName);
                imagePath = Path.GetDirectoryName(openFileDialog3.FileName);
                Console.WriteLine("Loaded file path: " + openFileDialog3.FileName);
                loadedImages = SprDecoder.GetSprites(data, palette);
                pictureBox1.Image = loadedImages[0];
                numericUpDown1.Value = 0;
                loadedImageIndex = 0;
                exportToolStripMenuItem.Enabled = true;
                exportAllToolStripMenuItem.Enabled = true;
                label2.Text = "Width: " + pictureBox1.Width + "  " + "Height: " + pictureBox1.Height;


            }

        }

        /// <summary>
        /// Load and setup palette
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ( openFileDialog2.ShowDialog() == DialogResult.OK )
            {
                string [] text = File.ReadAllLines(openFileDialog2.FileName);

                PalDecoder decoder = new PalDecoder(text);

                palette = decoder.Palette;

                if (SavedImageData != null)
                {
                    switch (loadedImageType)
                    {
                        case ImageType.Pic:
                            ShowPic(SavedImageData);
                            break;
                        case ImageType.Spr:
                            loadedImages = SprDecoder.GetSprites(SavedImageData, palette);
                            pictureBox1.Image = loadedImages[loadedImageIndex];
                            numericUpDown1.Value = loadedImageIndex;
                            break;


                    }
                    
                }
            }
        }

        private void viewPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPalette paletteView = new FormPalette(palette);

            paletteView.ShowDialog();
        }



        private void ToolStripSeparator1_Click(object sender, EventArgs e)
        {

        }

        private void exportPngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = loadedImageFilename;
            saveFileDialog1.InitialDirectory = imagePath;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);

            }
        }
        private void ExportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.InitialFolder = imagePath;
            if (openFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                string imagesDirectory = openFolderDialog.SelectedFolder + "/" + loadedImageFilename;
                if (!Directory.Exists(imagesDirectory)) Directory.CreateDirectory(imagesDirectory);
                for (int i = 0; i < loadedImages.Length; i++)
                {
                    loadedImages[i].Save(imagesDirectory + "/" + loadedImageFilename + "_" + i + ".png");
                }

                Console.WriteLine("Finished exporting all images.");
            }

        }

        private void OpenFileDialog3_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void OpenFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }


        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (loadedImages == null)
            {
                numericUpDown1.Value = 0;
                return;
            }
            if (numericUpDown1.Value >= loadedImages.Length) numericUpDown1.Value = loadedImages.Length - 1;
            loadedImageIndex = (int)numericUpDown1.Value;
            pictureBox1.Image = loadedImages[(int)numericUpDown1.Value];
        }


        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }

}
