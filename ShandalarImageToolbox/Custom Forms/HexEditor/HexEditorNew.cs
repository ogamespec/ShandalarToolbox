using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace ShandalarImageToolbox
{
    public partial class HexEditor : UserControl
    {
        FindOptions _findOptions = new FindOptions();
        ByteViewer ByteViewer;


        public HexEditor()
        {
            InitializeComponent();

            ByteViewer = new ByteViewer();

            ByteViewer.Dock = DockStyle.Fill;
            stPanel1.Controls.Add(ByteViewer);
        }

        public void OnControlClosing()
        {
            Cleanup();
            ByteViewer.Dispose();
        }

        private void Cleanup()
        {
         
        }


        public void LoadData(byte[] data)
        {
            Cleanup();
            ByteViewer.SetBytes(data);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
        }
    }
}
