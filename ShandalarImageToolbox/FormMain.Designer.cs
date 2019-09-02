namespace ShandalarImageToolbox
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.decodePicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.cATFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.assetsListBox = new System.Windows.Forms.ListBox();
            this.topPreviewUiPanel = new System.Windows.Forms.Panel();
            this.paletteComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.previewModeComboBox = new System.Windows.Forms.ComboBox();
            this.previewPanel = new System.Windows.Forms.Panel();
            this.imagePanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.hexEditor1 = new ShandalarImageToolbox.HexEditor();
            this.useLastEmbeddedPaletteCheckbox = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.topPreviewUiPanel.SuspendLayout();
            this.previewPanel.SuspendLayout();
            this.imagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decodePicToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1268, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // decodePicToolStripMenuItem
            // 
            this.decodePicToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.exportAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.decodePicToolStripMenuItem.Name = "decodePicToolStripMenuItem";
            this.decodePicToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.decodePicToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.cATFileToolStripMenuItem});
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.toolStripMenuItem1.Text = "Pic";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.loadPicToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(110, 22);
            this.toolStripMenuItem2.Text = "Spr";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.LoadSprToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(110, 22);
            this.toolStripMenuItem3.Text = "Palette";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.loadPaletteToolStripMenuItem_Click);
            // 
            // cATFileToolStripMenuItem
            // 
            this.cATFileToolStripMenuItem.Name = "cATFileToolStripMenuItem";
            this.cATFileToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.cATFileToolStripMenuItem.Text = "CAT";
            this.cATFileToolStripMenuItem.Click += new System.EventHandler(this.LoadCATFileToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pngToolStripMenuItem});
            this.exportToolStripMenuItem.Enabled = false;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // pngToolStripMenuItem
            // 
            this.pngToolStripMenuItem.Name = "pngToolStripMenuItem";
            this.pngToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.pngToolStripMenuItem.Text = "PNG";
            this.pngToolStripMenuItem.Click += new System.EventHandler(this.exportPngToolStripMenuItem_Click);
            // 
            // exportAllToolStripMenuItem
            // 
            this.exportAllToolStripMenuItem.Enabled = false;
            this.exportAllToolStripMenuItem.Name = "exportAllToolStripMenuItem";
            this.exportAllToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exportAllToolStripMenuItem.Text = "Export all";
            this.exportAllToolStripMenuItem.Click += new System.EventHandler(this.ExportAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(120, 6);
            this.toolStripSeparator1.Click += new System.EventHandler(this.ToolStripSeparator1_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewPaletteToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // viewPaletteToolStripMenuItem
            // 
            this.viewPaletteToolStripMenuItem.Name = "viewPaletteToolStripMenuItem";
            this.viewPaletteToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.viewPaletteToolStripMenuItem.Text = "View Palette";
            this.viewPaletteToolStripMenuItem.Click += new System.EventHandler(this.viewPaletteToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "png";
            this.saveFileDialog1.Filter = "PNG files|*.png|All files|*.*";
            this.saveFileDialog1.Title = "Export as PNG";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.topPreviewUiPanel);
            this.splitContainer1.Panel2.Controls.Add(this.previewPanel);
            this.splitContainer1.Panel2MinSize = 400;
            this.splitContainer1.Size = new System.Drawing.Size(1268, 617);
            this.splitContainer1.SplitterDistance = 374;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(372, 615);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.assetsListBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(364, 589);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Files List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // assetsListBox
            // 
            this.assetsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assetsListBox.FormattingEnabled = true;
            this.assetsListBox.Location = new System.Drawing.Point(2, 2);
            this.assetsListBox.Margin = new System.Windows.Forms.Padding(2);
            this.assetsListBox.Name = "assetsListBox";
            this.assetsListBox.Size = new System.Drawing.Size(360, 585);
            this.assetsListBox.TabIndex = 0;
            this.assetsListBox.SelectedIndexChanged += new System.EventHandler(this.AssetsListBox_SelectedIndexChanged);
            // 
            // topPreviewUiPanel
            // 
            this.topPreviewUiPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.topPreviewUiPanel.Controls.Add(this.useLastEmbeddedPaletteCheckbox);
            this.topPreviewUiPanel.Controls.Add(this.paletteComboBox);
            this.topPreviewUiPanel.Controls.Add(this.label1);
            this.topPreviewUiPanel.Controls.Add(this.previewModeComboBox);
            this.topPreviewUiPanel.Location = new System.Drawing.Point(0, 0);
            this.topPreviewUiPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topPreviewUiPanel.Name = "topPreviewUiPanel";
            this.topPreviewUiPanel.Size = new System.Drawing.Size(897, 29);
            this.topPreviewUiPanel.TabIndex = 3;
            // 
            // paletteComboBox
            // 
            this.paletteComboBox.Enabled = false;
            this.paletteComboBox.FormattingEnabled = true;
            this.paletteComboBox.Location = new System.Drawing.Point(185, 2);
            this.paletteComboBox.Name = "paletteComboBox";
            this.paletteComboBox.Size = new System.Drawing.Size(92, 21);
            this.paletteComboBox.TabIndex = 8;
            this.paletteComboBox.SelectedIndexChanged += new System.EventHandler(this.PaletteComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Current Palette:";
            // 
            // previewModeComboBox
            // 
            this.previewModeComboBox.DisplayMember = "0";
            this.previewModeComboBox.FormattingEnabled = true;
            this.previewModeComboBox.Items.AddRange(new object[] {
            "Preview",
            "View as hex"});
            this.previewModeComboBox.Location = new System.Drawing.Point(2, 2);
            this.previewModeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.previewModeComboBox.MaxDropDownItems = 3;
            this.previewModeComboBox.Name = "previewModeComboBox";
            this.previewModeComboBox.Size = new System.Drawing.Size(92, 21);
            this.previewModeComboBox.TabIndex = 6;
            this.previewModeComboBox.SelectedIndexChanged += new System.EventHandler(this.PreviewModeComboBox_SelectedIndexChanged);
            // 
            // previewPanel
            // 
            this.previewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewPanel.Controls.Add(this.imagePanel);
            this.previewPanel.Controls.Add(this.textBox1);
            this.previewPanel.Controls.Add(this.hexEditor1);
            this.previewPanel.Location = new System.Drawing.Point(0, 32);
            this.previewPanel.Margin = new System.Windows.Forms.Padding(2);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.Size = new System.Drawing.Size(899, 577);
            this.previewPanel.TabIndex = 5;
            // 
            // imagePanel
            // 
            this.imagePanel.AutoSize = true;
            this.imagePanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.imagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.imagePanel.Controls.Add(this.label2);
            this.imagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagePanel.Location = new System.Drawing.Point(0, 0);
            this.imagePanel.Margin = new System.Windows.Forms.Padding(0);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(899, 577);
            this.imagePanel.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(2, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(899, 577);
            this.textBox1.TabIndex = 6;
            this.textBox1.Visible = false;
            // 
            // hexEditor1
            // 
            this.hexEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexEditor1.Location = new System.Drawing.Point(0, 0);
            this.hexEditor1.Name = "hexEditor1";
            this.hexEditor1.Size = new System.Drawing.Size(899, 577);
            this.hexEditor1.TabIndex = 0;
            // 
            // useLastEmbeddedPaletteCheckbox
            // 
            this.useLastEmbeddedPaletteCheckbox.AutoSize = true;
            this.useLastEmbeddedPaletteCheckbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.useLastEmbeddedPaletteCheckbox.Enabled = false;
            this.useLastEmbeddedPaletteCheckbox.Location = new System.Drawing.Point(283, 5);
            this.useLastEmbeddedPaletteCheckbox.Name = "useLastEmbeddedPaletteCheckbox";
            this.useLastEmbeddedPaletteCheckbox.Size = new System.Drawing.Size(152, 17);
            this.useLastEmbeddedPaletteCheckbox.TabIndex = 10;
            this.useLastEmbeddedPaletteCheckbox.Text = "Use last embedded palette";
            this.useLastEmbeddedPaletteCheckbox.UseVisualStyleBackColor = true;
            this.useLastEmbeddedPaletteCheckbox.CheckedChanged += new System.EventHandler(this.UseLastEmbeddedPaletteCheckbox_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1268, 642);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(619, 397);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shandalar Image Toolbox";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.topPreviewUiPanel.ResumeLayout(false);
            this.topPreviewUiPanel.PerformLayout();
            this.previewPanel.ResumeLayout(false);
            this.previewPanel.PerformLayout();
            this.imagePanel.ResumeLayout(false);
            this.imagePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem decodePicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPaletteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pngToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem exportAllToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListBox assetsListBox;
        private System.Windows.Forms.Panel imagePanel;
        private System.Windows.Forms.Panel topPreviewUiPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel previewPanel;
        private System.Windows.Forms.ToolStripMenuItem cATFileToolStripMenuItem;
        private HexEditor hexEditor1;
        private System.Windows.Forms.ComboBox previewModeComboBox;
        private System.Windows.Forms.ComboBox paletteComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox useLastEmbeddedPaletteCheckbox;
    }
}

