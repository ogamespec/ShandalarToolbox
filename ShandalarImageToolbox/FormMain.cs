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
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;

namespace ShandalarImageToolbox
{
 
    public partial class FormMain : Form
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        private List<Color[]> palettes = new List<Color[]>();
        public Color[] lastEmbeddedPalette;
 

       
        public List<ShandalarAsset> loadedImages = new List<ShandalarAsset>();
        public int loadedImageIndex;
        public string windowTitle;
        public int selectedPaletteIndex;
        public bool useLastEmbeddedPalette;
        public static string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads/";
        public FormMain()
        {
            

            InitializeComponent();

            windowTitle = Text;


#if DEBUG
            AllocConsole();
#endif
            SetupGrayPalette();

            paletteComboBox.Enabled = true;
            paletteComboBox.SelectedIndex = 0;
            paletteComboBox.Text = paletteComboBox.SelectedItem.ToString();
            previewModeComboBox.SelectedIndex = 0;
            previewModeComboBox.Text = previewModeComboBox.SelectedItem.ToString();
        }

        public void AddPalette(string name)
        {
            palettes.Add(new Color[256]);
            paletteComboBox.Items.Add(name);

        }
        public void AddPalette(Color[] palette, string name)
        {
            palettes.Add(palette);
            paletteComboBox.Items.Add(name);

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
            AddPalette("Grayscale");
            for(int i=0; i<256; i++)
            {
                palettes[selectedPaletteIndex][i] = GrayColor((byte)i);
            }
        }

        /// <summary>
        /// Load and decode .PIC image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadPicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "pic";
            openFileDialog.Filter = "PIC Files|*.pic|All Files|*.*";
            openFileDialog.Multiselect = true;
            if ( openFileDialog.ShowDialog() == DialogResult.OK)
            {
                assetsListBox.Items.Clear();
                loadedImages.Clear();
                foreach (string fileName in openFileDialog.FileNames)
                {
                    byte[] fileData = File.ReadAllBytes(fileName);
                    previewModeComboBox.SelectedIndex = 0;
                    Text = windowTitle + " - " + Path.GetFileName(fileName);
                    Console.WriteLine("Loaded file path: " + fileName);
                    loadedImages.Add(GetPic(fileData, fileName));
                    assetsListBox.Items.Add(Path.GetFileNameWithoutExtension(fileName));
                }
                exportToolStripMenuItem.Enabled = true;
                exportAllToolStripMenuItem.Enabled = true;
            }
        }

        public ShandalarAsset GetPic ( byte [] data, string name)
        {
            Color[] originalPalette = palettes[selectedPaletteIndex];

            

            PicDecoder decoder = new PicDecoder(data);

           if(decoder.hasPalette) palettes[selectedPaletteIndex] = decoder.embeddedPalette;

            /// Output image as picture box
            

            Bitmap bitmap = new Bitmap(decoder.width, decoder.height);

           byte[,] imageData = new byte[decoder.width,decoder.height];
            decoder.DecodeImage(imageData);
            if (decoder.hasPalette)
            {
                useLastEmbeddedPaletteCheckbox.Enabled = true;
                lastEmbeddedPalette = decoder.embeddedPalette;
            }
            for (int y = 0; y < decoder.height; y++)
            {
                for (int x = 0; x < decoder.width; x++)
                {
                    byte value = imageData[x, y];
                    Color color = palettes[selectedPaletteIndex][value];
                    if (useLastEmbeddedPalette && lastEmbeddedPalette != null) color = lastEmbeddedPalette[value];
                    if (color == Color.Transparent)
                    {
                        if (name.Contains("Cardart") || name.Contains("Duelart") || name.Contains("Exp1art")) color = Color.Black; //The Cardart pic files have a strange palette difference
                        else color = Color.FromArgb(value, value, value);
                    }
                    bitmap.SetPixel(x, y, color);

                }
            }
            ShandalarAsset asset = new ShandalarAsset(name, data, ImageType.Pic);
            asset.image = bitmap;
            if (asset.filePath.Contains("Rogue") || asset.filePath.Contains("Faces") || asset.filePath.Contains("Playface")) //Combine the embedded alpha channel with the main image for the face sprites
            {
                Bitmap transparentSprite = new Bitmap(asset.image.Width / 2, asset.image.Height);
                for(int x = 0; x < asset.image.Width/2; x++)
                {
                    for(int y = 0; y < asset.image.Height; y++)
                    {
                         byte paletteIndex = imageData[x + asset.image.Width/2, y];
                        Color color = palettes[selectedPaletteIndex][paletteIndex];
                        if ((paletteIndex == 0)) transparentSprite.SetPixel(x, y, asset.image.GetPixel(x, y));
                        else transparentSprite.SetPixel(x, y, Color.Transparent);
                       
                    }
                }
                asset.image = transparentSprite;
            }
            palettes[selectedPaletteIndex] = originalPalette;
            asset.hasEmbeddedPalette = decoder.hasPalette;
            asset.filename = Path.GetFileNameWithoutExtension(asset.filePath);
            ClearImagePanel();
            return asset;

        }

        private void LoadSprToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "spr";
            openFileDialog.Filter = "Sprite Files|*.spr|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                exportToolStripMenuItem.Enabled = true;
                byte[] data = File.ReadAllBytes(openFileDialog.FileName);
                previewModeComboBox.SelectedIndex = 0;
                Text = windowTitle + " - " + Path.GetFileName(openFileDialog.FileName);
                string loadedImageFilename = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                Console.WriteLine("Loaded file path: " + openFileDialog.FileName);
                Color[] palette = useLastEmbeddedPalette && lastEmbeddedPalette != null ? lastEmbeddedPalette : palettes[selectedPaletteIndex];
                List<Bitmap> sprites = SprDecoder.GetSprites(data, palette).ToList();
                ClearImagePanel();
                assetsListBox.Items.Clear();
                loadedImages.Clear();
                for(int i = 0; i < sprites.Count; i++)
                {
                    ShandalarAsset asset = new ShandalarAsset(loadedImageFilename, data,ImageType.Spr);
                    asset.image = sprites[i];
                    asset.childIndex = i;
                    asset.parentName = loadedImageFilename;
                    asset.filename = Path.GetFileNameWithoutExtension(asset.filePath) + "_" + i;
                    loadedImages.Add(asset);
                }
                for (int i = 0; i < loadedImages.Count; i++)
                {
                    assetsListBox.Items.Add(loadedImageFilename + "_" +i);
                }
                exportToolStripMenuItem.Enabled = true;
                exportAllToolStripMenuItem.Enabled = true;
                


            }

        }
        private void LoadCATFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "cat";
            openFileDialog.Filter = "Cat Files|*.cat|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cat cat = new Cat(openFileDialog.FileName);
                assetsListBox.Items.Clear();
                loadedImages.Clear();
                for (int i = 0; i < cat.files.Count; i++)
                {
                    string filepath = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    assetsListBox.Items.Add(cat.files[i].name);
                    ShandalarAsset asset = new ShandalarAsset(filepath, cat.files[i].data, ImageType.Cat);
                    asset.childIndex = i;
                    asset.parentName = filepath;
                    asset.filename = cat.files[i].name;
                    loadedImages.Add(asset);
                }
            }
            exportToolStripMenuItem.Enabled = true;
            exportAllToolStripMenuItem.Enabled = true;
        }
        public void ShowImage(Bitmap imageTexture)
        {
            ShandalarAsset image = loadedImages[loadedImageIndex];
            imagePanel.BackgroundImage = imageTexture;
            switch (image.imageType)
            {
                case ImageType.Pic:
                    label2.Text = "Width: " + imageTexture.Width + "\nHeight: " + imageTexture.Height + "\nHas Embedded Palette:" + image.hasEmbeddedPalette;
                    break;
                default:
                    label2.Text = "Width: " + imageTexture.Width + "\n" + "Height: " + imageTexture.Height;
                    break;

            }
            
            if (imageTexture.Width > imagePanel.Width || imageTexture.Height > imagePanel.Height)
                imagePanel.BackgroundImageLayout = ImageLayout.Zoom;
            else
                imagePanel.BackgroundImageLayout = ImageLayout.Center;
        }
        public void ClearImagePanel()
        {
            imagePanel.BackgroundImage = null;
            label2.Text = "";
        }

        /// <summary>
        /// Load and setup palette
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "tr";
            openFileDialog.Filter = "Palette Files|*.tr|All Files|*.*";
            openFileDialog.Multiselect = true;
            if ( openFileDialog.ShowDialog() == DialogResult.OK )
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    string[] text = File.ReadAllLines(fileName);

                    PalDecoder decoder = new PalDecoder(text);

                    AddPalette(decoder.Palette,Path.GetFileNameWithoutExtension(fileName));
                    
                }

                
            }
        }

        private void viewPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPalette paletteView = new FormPalette(palettes[selectedPaletteIndex]);

            paletteView.ShowDialog();
        }



        private void ToolStripSeparator1_Click(object sender, EventArgs e)
        {

        }

        private void exportPngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(loadedImages[loadedImageIndex].filename);
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(loadedImages[loadedImageIndex].filename);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                 loadedImages[loadedImageIndex].image.Save(saveFileDialog1.FileName);

            }
        }
        private void ExportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.InitialFolder = Path.GetDirectoryName(loadedImages[loadedImageIndex].filename);
            if (openFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                string imagesDirectory;

                for (int i = 0; i < loadedImages.Count; i++)
                {

                    switch (loadedImages[i].imageType) {
                        case ImageType.Pic:
                        imagesDirectory = openFolderDialog.SelectedFolder;
                            if (!Directory.Exists(imagesDirectory)) Directory.CreateDirectory(imagesDirectory);
                            loadedImages[i].image.Save(imagesDirectory + "/" + loadedImages[i].filename + ".png");
                        break;
                        case ImageType.Spr:
                        imagesDirectory = openFolderDialog.SelectedFolder + "/" + loadedImages[i].parentName;
                        if (!Directory.Exists(imagesDirectory)) Directory.CreateDirectory(imagesDirectory);
                        loadedImages[i].image.Save(imagesDirectory + "/" + loadedImages[i].filename + ".png");
                        break;
                        case ImageType.Cat:
                            imagesDirectory = openFolderDialog.SelectedFolder + "/" + loadedImages[i].parentName;
                            if (!Directory.Exists(imagesDirectory)) Directory.CreateDirectory(imagesDirectory);
                            File.WriteAllBytes(imagesDirectory + "/" + loadedImages[i].filename + ".wvl",loadedImages[i].data);

                            break;
                    }


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
            Console.WriteLine("New selection! " + System.DateTime.Now.ToString());
            loadedImageIndex = assetsListBox.SelectedIndex;
            if (loadedImageIndex != -1) {
                ShandalarAsset asset = loadedImages[loadedImageIndex];
                if(loadedImages[assetsListBox.SelectedIndex].image != null) ShowImage(asset.image);
                string fileText = Encoding.UTF8.GetString(asset.data);
                string tempText = Regex.Replace(fileText, @"\p{C}+", string.Empty);
                hexEditor1.LoadData(asset.data);
                if (tempText != fileText)
                    textBox1.Text = fileText;
                if (asset.imageType == ImageType.Cat)
                {
                    DecodeCatImage(asset);
                }
            }

        }
        public void DecodeCatImage(ShandalarAsset asset)
        {
            byte[] uncompressedData = Vlc.VlcDecompress(asset.data);

            int width = (int)BitConverter.ToUInt32(asset.data, 0x1c);
            int height = (int)BitConverter.ToUInt32(asset.data, 0x20);
            int smallTableSize = (int)BitConverter.ToUInt32(asset.data, 0x24);

            int var_14;
            int var_24;

            if ((int)BitConverter.ToUInt32(asset.data, 0) != 0)
            {
                /// var_14
                int eax = width;
                int ecx = (int)BitConverter.ToUInt32(asset.data, 0x28) - 1;
                if (ecx == 0)
                    var_14 = eax / 2;
                else
                    var_14 = eax;

                /// var_24
                eax = height;
                ecx = (int)BitConverter.ToUInt32(asset.data, 0x28) - 1;
                if (ecx == 0)
                    var_24 = eax / 2;
                else
                    var_24 = eax;
            }
            else
            {
                var_14 = width;
                var_24 = height;
            }

            int ptr1 = 0;
            int ptr2 = ptr1 + width * width * 4 + 0x80;
            int ptr3 = ptr2 + var_14 * var_14 * 4 + 0x80;

            int[] tempArray = GeneralUtilityFunctions.ByteArrayToIntArray(uncompressedData, 0);

            Wavelet.WaveletDecode(ref tempArray, ptr1, width, smallTableSize);
            Wavelet.WaveletDecode(ref tempArray, ptr2/4, var_14, smallTableSize);
            Wavelet.WaveletDecode(ref tempArray, ptr3/4, var_14, smallTableSize);

            /// YCbCr -> RGB
            Bitmap outputImage = Wavelet.Decode_YCbCrToRGB(tempArray,
            ptr1,
            width,
            height,
            ptr2/4,
            ptr3/4,
            var_14,
            var_24);

            // Save image
            //outputImage.Save(downloadsPath + "wvlDecodedCardImage.png");
            asset.image = outputImage;
            ShowImage(outputImage);
        }

        private void CATFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

       



        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void PreviewModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            imagePanel.Visible = false;
            hexEditor1.Visible = false;
            switch (previewModeComboBox.SelectedIndex)
            {
                case 0:
                    imagePanel.Visible = true;
                    break;
                case 1:
                    hexEditor1.Visible = true;
                    break;
            }
        }

        private void PaletteComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            paletteComboBox.Text = paletteComboBox.SelectedItem.ToString();
            selectedPaletteIndex = paletteComboBox.SelectedIndex;
            if (loadedImages.Count > 0 && loadedImages[loadedImageIndex].data != null)
            {
                foreach (ShandalarAsset asset in loadedImages)
                {
                    switch (asset.imageType)
                    {
                        case ImageType.Pic:
                        asset.image = GetPic(asset.data, asset.filePath).image;
                        break;
                        case ImageType.Spr:
                        List<Bitmap> sprites = SprDecoder.GetSprites(loadedImages[loadedImageIndex].data, palettes[selectedPaletteIndex]);
                        asset.image = sprites[asset.childIndex];
                        break;


                    }
                }

                ShowImage(loadedImages[loadedImageIndex].image);

            }
        }

        private void UseLastEmbeddedPaletteCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            useLastEmbeddedPalette = useLastEmbeddedPaletteCheckbox.Checked;
            Color[] palette = useLastEmbeddedPalette ? lastEmbeddedPalette : palettes[selectedPaletteIndex];
            if (loadedImages.Count > 0 && loadedImages[loadedImageIndex].data != null)
            {
                foreach (ShandalarAsset asset in loadedImages)
                {
                    switch (asset.imageType)
                    {
                        case ImageType.Pic:
                            asset.image = GetPic(asset.data, asset.filename).image;
                            break;
                        case ImageType.Spr:
                            List<Bitmap> sprites = SprDecoder.GetSprites(loadedImages[loadedImageIndex].data, palette);
                            asset.image = sprites[asset.childIndex];
                            break;


                    }
                }

                ShowImage(loadedImages[loadedImageIndex].image);

            }

        }
    }

}
