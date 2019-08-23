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
 
    public partial class FormMain : Form
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
        public string windowTitle;

        public FormMain()
        {
            

            InitializeComponent();

            windowTitle = Text;

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
                Text = windowTitle + " - " + Path.GetFileName(openFileDialog1.FileName);
                Console.WriteLine("Loaded file path: " + openFileDialog1.FileName);
                ShowPic(SavedImageData);
                assetsListBox.Items.Clear();
                assetsListBox.Items.Add(loadedImageFilename);
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
            ShowImage(bitmap);

        }

        private void LoadSprToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                exportToolStripMenuItem.Enabled = true;
                byte[] data = File.ReadAllBytes(openFileDialog3.FileName);
                Text = windowTitle + " - " + Path.GetFileName(openFileDialog3.FileName);
                SavedImageData = data;
                loadedImageType = ImageType.Spr;
                loadedImageFilename = Path.GetFileNameWithoutExtension(openFileDialog3.FileName);
                imagePath = Path.GetDirectoryName(openFileDialog3.FileName);
                Console.WriteLine("Loaded file path: " + openFileDialog3.FileName);
                loadedImages = SprDecoder.GetSprites(data, palette);
                ShowImage(loadedImages[0]);
                loadedImageIndex = 0;
                assetsListBox.Items.Clear();
                for (int i = 0; i < loadedImages.Length; i++)
                {
                    assetsListBox.Items.Add(loadedImageFilename + i);
                }
                exportToolStripMenuItem.Enabled = true;
                exportAllToolStripMenuItem.Enabled = true;
                


            }

        }
        public void ShowImage(Bitmap imageTexture)
        {
            imagePanel.BackgroundImage = imageTexture;
            label2.Text = "Width: " + imageTexture.Width + "  " + "Height: " + imageTexture.Height;
            if (imageTexture.Width > imagePanel.Width || imageTexture.Height > imagePanel.Height)
                imagePanel.BackgroundImageLayout = ImageLayout.Zoom;
            else
                imagePanel.BackgroundImageLayout = ImageLayout.Center;
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
                            ShowImage(loadedImages[loadedImageIndex]);
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
                 loadedImages[0].Save(saveFileDialog1.FileName);

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




        private void AssetsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadedImageIndex = assetsListBox.SelectedIndex;
            ShowImage(loadedImages[assetsListBox.SelectedIndex]);

        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }

}
