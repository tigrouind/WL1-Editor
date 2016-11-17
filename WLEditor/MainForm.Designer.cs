/*
 * Created by SharpDevelop.
 * User: Tigrou
 * Date: 1/11/2015
 * Time: 15:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace WLEditor
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.regionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.objectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scrollRegionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.collidersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.classicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blackWhiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.autumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.switchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Location = new System.Drawing.Point(144, 54);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(752, 405);
			this.panel1.TabIndex = 1;
			this.panel1.Visible = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(3, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(4096, 512);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.PictureBox1Click);
			this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox1Paint);
			this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1MouseDown);
			this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1MouseMove);
			// 
			// pictureBox3
			// 
			this.pictureBox3.Location = new System.Drawing.Point(10, 54);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(128, 256);
			this.pictureBox3.TabIndex = 3;
			this.pictureBox3.TabStop = false;
			this.pictureBox3.Visible = false;
			this.pictureBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox3Paint);
			this.pictureBox3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox3MouseDown);
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(12, 27);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(126, 21);
			this.comboBox1.TabIndex = 4;
			this.comboBox1.Visible = false;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.viewToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(908, 24);
			this.menuStrip1.TabIndex = 5;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.loadToolStripMenuItem,
			this.saveToolStripMenuItem,
			this.saveAsToolStripMenuItem,
			this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.loadToolStripMenuItem.Text = "Load rom...";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItemClick);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Enabled = false;
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
			| System.Windows.Forms.Keys.S)));
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.saveAsToolStripMenuItem.Text = "Save As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItemClick);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.regionsToolStripMenuItem,
			this.objectsToolStripMenuItem,
			this.scrollRegionToolStripMenuItem,
			this.collidersToolStripMenuItem,
			this.paletteToolStripMenuItem,
			this.switchToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// regionsToolStripMenuItem
			// 
			this.regionsToolStripMenuItem.Checked = true;
			this.regionsToolStripMenuItem.CheckOnClick = true;
			this.regionsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.regionsToolStripMenuItem.Name = "regionsToolStripMenuItem";
			this.regionsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.regionsToolStripMenuItem.Text = "Sectors";
			this.regionsToolStripMenuItem.Click += new System.EventHandler(this.RegionsToolStripMenuItemClick);
			// 
			// objectsToolStripMenuItem
			// 
			this.objectsToolStripMenuItem.Checked = true;
			this.objectsToolStripMenuItem.CheckOnClick = true;
			this.objectsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.objectsToolStripMenuItem.Name = "objectsToolStripMenuItem";
			this.objectsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.objectsToolStripMenuItem.Text = "Objects";
			this.objectsToolStripMenuItem.Click += new System.EventHandler(this.ObjectsToolStripMenuItemClick);
			// 
			// scrollRegionToolStripMenuItem
			// 
			this.scrollRegionToolStripMenuItem.Checked = true;
			this.scrollRegionToolStripMenuItem.CheckOnClick = true;
			this.scrollRegionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.scrollRegionToolStripMenuItem.Name = "scrollRegionToolStripMenuItem";
			this.scrollRegionToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.scrollRegionToolStripMenuItem.Text = "Scroll boundaries";
			this.scrollRegionToolStripMenuItem.Click += new System.EventHandler(this.ScrollRegionToolStripMenuItemClick);
			// 
			// collidersToolStripMenuItem
			// 
			this.collidersToolStripMenuItem.CheckOnClick = true;
			this.collidersToolStripMenuItem.Name = "collidersToolStripMenuItem";
			this.collidersToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.collidersToolStripMenuItem.Text = "Colliders";
			this.collidersToolStripMenuItem.Click += new System.EventHandler(this.CollidersToolStripMenuItemClick);
			// 
			// paletteToolStripMenuItem
			// 
			this.paletteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.classicToolStripMenuItem,
			this.blackWhiteToolStripMenuItem,
			this.autumnToolStripMenuItem});
			this.paletteToolStripMenuItem.Name = "paletteToolStripMenuItem";
			this.paletteToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.paletteToolStripMenuItem.Text = "Palette";
			// 
			// classicToolStripMenuItem
			// 
			this.classicToolStripMenuItem.Checked = true;
			this.classicToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.classicToolStripMenuItem.Name = "classicToolStripMenuItem";
			this.classicToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.classicToolStripMenuItem.Text = "Classic";
			this.classicToolStripMenuItem.Click += new System.EventHandler(this.ClassicToolStripMenuItemClick);
			// 
			// blackWhiteToolStripMenuItem
			// 
			this.blackWhiteToolStripMenuItem.Name = "blackWhiteToolStripMenuItem";
			this.blackWhiteToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.blackWhiteToolStripMenuItem.Text = "Black && white";
			this.blackWhiteToolStripMenuItem.Click += new System.EventHandler(this.BlackWhiteToolStripMenuItemClick);
			// 
			// autumnToolStripMenuItem
			// 
			this.autumnToolStripMenuItem.Name = "autumnToolStripMenuItem";
			this.autumnToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.autumnToolStripMenuItem.Text = "Autumn";
			this.autumnToolStripMenuItem.Click += new System.EventHandler(this.AutumnToolStripMenuItemClick);
			// 
			// switchToolStripMenuItem
			// 
			this.switchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.noneToolStripMenuItem,
			this.aToolStripMenuItem,
			this.bToolStripMenuItem});
			this.switchToolStripMenuItem.Name = "switchToolStripMenuItem";
			this.switchToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.switchToolStripMenuItem.Text = "Block switch";
			// 
			// noneToolStripMenuItem
			// 
			this.noneToolStripMenuItem.Checked = true;
			this.noneToolStripMenuItem.CheckOnClick = true;
			this.noneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
			this.noneToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.noneToolStripMenuItem.Text = "None";
			this.noneToolStripMenuItem.Click += new System.EventHandler(this.NoneToolStripMenuItemClick);
			// 
			// aToolStripMenuItem
			// 
			this.aToolStripMenuItem.CheckOnClick = true;
			this.aToolStripMenuItem.Name = "aToolStripMenuItem";
			this.aToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.aToolStripMenuItem.Text = "A";
			this.aToolStripMenuItem.Click += new System.EventHandler(this.AToolStripMenuItemClick);
			// 
			// bToolStripMenuItem
			// 
			this.bToolStripMenuItem.CheckOnClick = true;
			this.bToolStripMenuItem.Name = "bToolStripMenuItem";
			this.bToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.bToolStripMenuItem.Text = "B";
			this.bToolStripMenuItem.Click += new System.EventHandler(this.BToolStripMenuItemClick);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Location = new System.Drawing.Point(10, 316);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(64, 64);
			this.pictureBox2.TabIndex = 6;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Visible = false;
			this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox2Paint);
			this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox2MouseDown);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(908, 471);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "WLEditor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem switchToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem collidersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem scrollRegionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem objectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem regionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem classicToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blackWhiteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem autumnToolStripMenuItem;
		

			
	
		
	}
}
