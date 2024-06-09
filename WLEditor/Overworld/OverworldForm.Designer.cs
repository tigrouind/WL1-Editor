namespace WLEditor
{
	partial class OverworldForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.ComboBox WorldComboBox;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.WorldComboBox = new System.Windows.Forms.ComboBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.musicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tileDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tileModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.eventModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pathModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.transparentPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.tableLayoutPanel2.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 32);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(268, 134);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(3, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(128, 128);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox1Paint);
			this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1MouseDown);
			this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1MouseMove);
			this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1MouseUp);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Location = new System.Drawing.Point(137, 3);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(128, 128);
			this.pictureBox2.TabIndex = 2;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox2Paint);
			this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox2MouseDown);
			this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox2MouseMove);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.WorldComboBox, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 28);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(5);
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(525, 248);
			this.tableLayoutPanel2.TabIndex = 3;
			// 
			// WorldComboBox
			// 
			this.WorldComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.WorldComboBox.FormattingEnabled = true;
			this.WorldComboBox.Location = new System.Drawing.Point(8, 8);
			this.WorldComboBox.Name = "WorldComboBox";
			this.WorldComboBox.Size = new System.Drawing.Size(166, 21);
			this.WorldComboBox.TabIndex = 0;
			this.WorldComboBox.SelectedIndexChanged += new System.EventHandler(this.WorldComboBoxSelectedIndexChanged);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 276);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(525, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 4;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(525, 28);
			this.menuStrip1.TabIndex = 5;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.musicToolStripMenuItem,
            this.tileDataToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// musicToolStripMenuItem
			// 
			this.musicToolStripMenuItem.Name = "musicToolStripMenuItem";
			this.musicToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.musicToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
			this.musicToolStripMenuItem.Text = "Change music...";
			this.musicToolStripMenuItem.Click += new System.EventHandler(this.MusicToolStripMenuItem_Click);
			// 
			// tileDataToolStripMenuItem
			// 
			this.tileDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportTilesToolStripMenuItem,
            this.exportMapToolStripMenuItem});
			this.tileDataToolStripMenuItem.Name = "tileDataToolStripMenuItem";
			this.tileDataToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
			this.tileDataToolStripMenuItem.Text = "Tile data";
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
			this.importToolStripMenuItem.Text = "Import...";
			this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItem_Click);
			// 
			// exportTilesToolStripMenuItem
			// 
			this.exportTilesToolStripMenuItem.Name = "exportTilesToolStripMenuItem";
			this.exportTilesToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
			this.exportTilesToolStripMenuItem.Text = "Export tiles...";
			this.exportTilesToolStripMenuItem.Click += new System.EventHandler(this.ExportTilesToolStripMenuItem_Click);
			// 
			// exportMapToolStripMenuItem
			// 
			this.exportMapToolStripMenuItem.Name = "exportMapToolStripMenuItem";
			this.exportMapToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
			this.exportMapToolStripMenuItem.Text = "Export map...";
			this.exportMapToolStripMenuItem.Click += new System.EventHandler(this.ExportMapToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileModeToolStripMenuItem,
            this.eventModeToolStripMenuItem,
            this.pathModeToolStripMenuItem,
            this.transparentPathToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// tileModeToolStripMenuItem
			// 
			this.tileModeToolStripMenuItem.Checked = true;
			this.tileModeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tileModeToolStripMenuItem.Name = "tileModeToolStripMenuItem";
			this.tileModeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.tileModeToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
			this.tileModeToolStripMenuItem.Text = "Tile mode";
			this.tileModeToolStripMenuItem.Click += new System.EventHandler(this.TileModeToolStripMenuItem_Click);
			// 
			// eventModeToolStripMenuItem
			// 
			this.eventModeToolStripMenuItem.Name = "eventModeToolStripMenuItem";
			this.eventModeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.eventModeToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
			this.eventModeToolStripMenuItem.Text = "Event mode";
			this.eventModeToolStripMenuItem.Click += new System.EventHandler(this.EventModeToolStripMenuItem_Click);
			// 
			// pathModeToolStripMenuItem
			// 
			this.pathModeToolStripMenuItem.Name = "pathModeToolStripMenuItem";
			this.pathModeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.pathModeToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
			this.pathModeToolStripMenuItem.Text = "Path mode";
			this.pathModeToolStripMenuItem.Click += new System.EventHandler(this.PathModeToolStripMenuItem_Click);
			// 
			// transparentPathToolStripMenuItem
			// 
			this.transparentPathToolStripMenuItem.CheckOnClick = true;
			this.transparentPathToolStripMenuItem.Enabled = false;
			this.transparentPathToolStripMenuItem.Name = "transparentPathToolStripMenuItem";
			this.transparentPathToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.transparentPathToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
			this.transparentPathToolStripMenuItem.Text = "Transparent paths";
			this.transparentPathToolStripMenuItem.Click += new System.EventHandler(this.TransparentPathToolStripMenuItem_Click);
			// 
			// OverworldForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(525, 298);
			this.Controls.Add(this.tableLayoutPanel2);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OverworldForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Overworld";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OverworldFormClosing);
			this.VisibleChanged += new System.EventHandler(this.OverworldVisibleChanged);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tileModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem eventModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pathModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem musicToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem transparentPathToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tileDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportTilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportMapToolStripMenuItem;
	}
}
