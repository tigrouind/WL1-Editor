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

				tilesWorld8x8.Dispose();
				tilesWorld.Dispose();
				tilesWorldScroll.Dispose();
				pathForm.Dispose();
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
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			pictureBox2 = new System.Windows.Forms.PictureBox();
			tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			WorldComboBox = new System.Windows.Forms.ComboBox();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			musicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			tileDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			tileModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			eventModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pathModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			transparentPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
			tableLayoutPanel2.SuspendLayout();
			statusStrip1.SuspendLayout();
			menuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.Controls.Add(pictureBox1, 0, 0);
			tableLayoutPanel1.Controls.Add(pictureBox2, 1, 0);
			tableLayoutPanel1.Location = new System.Drawing.Point(7, 46);
			tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.Size = new System.Drawing.Size(358, 207);
			tableLayoutPanel1.TabIndex = 2;
			// 
			// pictureBox1
			// 
			pictureBox1.Location = new System.Drawing.Point(4, 5);
			pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(171, 197);
			pictureBox1.TabIndex = 3;
			pictureBox1.TabStop = false;
			pictureBox1.Paint += PictureBox1Paint;
			pictureBox1.MouseDown += PictureBox1MouseDown;
			pictureBox1.MouseMove += PictureBox1MouseMove;
			pictureBox1.MouseUp += PictureBox1MouseUp;
			// 
			// pictureBox2
			// 
			pictureBox2.Location = new System.Drawing.Point(183, 5);
			pictureBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			pictureBox2.Name = "pictureBox2";
			pictureBox2.Size = new System.Drawing.Size(171, 197);
			pictureBox2.TabIndex = 2;
			pictureBox2.TabStop = false;
			pictureBox2.Paint += PictureBox2Paint;
			pictureBox2.MouseDown += PictureBox2MouseDown;
			pictureBox2.MouseMove += PictureBox2MouseMove;
			// 
			// tableLayoutPanel2
			// 
			tableLayoutPanel2.AutoSize = true;
			tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel2.ColumnCount = 1;
			tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel2.Controls.Add(WorldComboBox, 0, 0);
			tableLayoutPanel2.Controls.Add(tableLayoutPanel1, 0, 1);
			tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel2.Location = new System.Drawing.Point(0, 30);
			tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
			tableLayoutPanel2.Name = "tableLayoutPanel2";
			tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(7, 8, 7, 8);
			tableLayoutPanel2.RowCount = 2;
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel2.Size = new System.Drawing.Size(700, 406);
			tableLayoutPanel2.TabIndex = 3;
			// 
			// WorldComboBox
			// 
			WorldComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			WorldComboBox.FormattingEnabled = true;
			WorldComboBox.Location = new System.Drawing.Point(11, 13);
			WorldComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			WorldComboBox.Name = "WorldComboBox";
			WorldComboBox.Size = new System.Drawing.Size(220, 28);
			WorldComboBox.TabIndex = 0;
			WorldComboBox.SelectedIndexChanged += WorldComboBoxSelectedIndexChanged;
			WorldComboBox.KeyDown += WorldComboBox_KeyDown;
			// 
			// statusStrip1
			// 
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
			statusStrip1.Location = new System.Drawing.Point(0, 436);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
			statusStrip1.Size = new System.Drawing.Size(700, 22);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 4;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editToolStripMenuItem, viewToolStripMenuItem });
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
			menuStrip1.Size = new System.Drawing.Size(700, 30);
			menuStrip1.TabIndex = 5;
			menuStrip1.Text = "menuStrip1";
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { musicToolStripMenuItem, tileDataToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
			editToolStripMenuItem.Text = "Edit";
			// 
			// musicToolStripMenuItem
			// 
			musicToolStripMenuItem.Name = "musicToolStripMenuItem";
			musicToolStripMenuItem.ShortcutKeyDisplayString = "";
			musicToolStripMenuItem.Size = new System.Drawing.Size(179, 24);
			musicToolStripMenuItem.Text = "Change music...";
			musicToolStripMenuItem.Click += MusicToolStripMenuItem_Click;
			// 
			// tileDataToolStripMenuItem
			// 
			tileDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { importToolStripMenuItem, exportTilesToolStripMenuItem, exportMapToolStripMenuItem });
			tileDataToolStripMenuItem.Name = "tileDataToolStripMenuItem";
			tileDataToolStripMenuItem.Size = new System.Drawing.Size(179, 24);
			tileDataToolStripMenuItem.Text = "Tile data";
			// 
			// importToolStripMenuItem
			// 
			importToolStripMenuItem.Name = "importToolStripMenuItem";
			importToolStripMenuItem.Size = new System.Drawing.Size(164, 24);
			importToolStripMenuItem.Text = "Import...";
			importToolStripMenuItem.Click += ImportToolStripMenuItem_Click;
			// 
			// exportTilesToolStripMenuItem
			// 
			exportTilesToolStripMenuItem.Name = "exportTilesToolStripMenuItem";
			exportTilesToolStripMenuItem.Size = new System.Drawing.Size(164, 24);
			exportTilesToolStripMenuItem.Text = "Export tiles...";
			exportTilesToolStripMenuItem.Click += ExportTilesToolStripMenuItem_Click;
			// 
			// exportMapToolStripMenuItem
			// 
			exportMapToolStripMenuItem.Name = "exportMapToolStripMenuItem";
			exportMapToolStripMenuItem.Size = new System.Drawing.Size(164, 24);
			exportMapToolStripMenuItem.Text = "Export map...";
			exportMapToolStripMenuItem.Click += ExportMapToolStripMenuItem_Click;
			// 
			// viewToolStripMenuItem
			// 
			viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tileModeToolStripMenuItem, eventModeToolStripMenuItem, pathModeToolStripMenuItem, transparentPathToolStripMenuItem });
			viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
			viewToolStripMenuItem.Text = "View";
			// 
			// tileModeToolStripMenuItem
			// 
			tileModeToolStripMenuItem.Checked = true;
			tileModeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			tileModeToolStripMenuItem.Name = "tileModeToolStripMenuItem";
			tileModeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			tileModeToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
			tileModeToolStripMenuItem.Text = "Tile mode";
			tileModeToolStripMenuItem.Click += TileModeToolStripMenuItem_Click;
			// 
			// eventModeToolStripMenuItem
			// 
			eventModeToolStripMenuItem.Name = "eventModeToolStripMenuItem";
			eventModeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
			eventModeToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
			eventModeToolStripMenuItem.Text = "Event mode";
			eventModeToolStripMenuItem.Click += EventModeToolStripMenuItem_Click;
			// 
			// pathModeToolStripMenuItem
			// 
			pathModeToolStripMenuItem.Name = "pathModeToolStripMenuItem";
			pathModeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			pathModeToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
			pathModeToolStripMenuItem.Text = "Path mode";
			pathModeToolStripMenuItem.Click += PathModeToolStripMenuItem_Click;
			// 
			// transparentPathToolStripMenuItem
			// 
			transparentPathToolStripMenuItem.CheckOnClick = true;
			transparentPathToolStripMenuItem.Enabled = false;
			transparentPathToolStripMenuItem.Name = "transparentPathToolStripMenuItem";
			transparentPathToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
			transparentPathToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
			transparentPathToolStripMenuItem.Text = "Transparent paths";
			transparentPathToolStripMenuItem.Click += TransparentPathToolStripMenuItem_Click;
			// 
			// OverworldForm
			// 
			AllowDrop = true;
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			ClientSize = new System.Drawing.Size(700, 458);
			Controls.Add(tableLayoutPanel2);
			Controls.Add(statusStrip1);
			Controls.Add(menuStrip1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			MainMenuStrip = menuStrip1;
			Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "OverworldForm";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Overworld";
			FormClosing += OverworldFormClosing;
			VisibleChanged += OverworldVisibleChanged;
			tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
			tableLayoutPanel2.ResumeLayout(false);
			tableLayoutPanel2.PerformLayout();
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

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
