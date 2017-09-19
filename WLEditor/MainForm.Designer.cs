﻿/*
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
			this.LevelPanel = new System.Windows.Forms.Panel();
			this.levelPictureBox = new System.Windows.Forms.PictureBox();
			this.tilesPictureBox = new System.Windows.Forms.PictureBox();
			this.levelComboBox = new System.Windows.Forms.ComboBox();
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
			this.objectPictureBox = new System.Windows.Forms.PictureBox();
			this.LevelPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.levelPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tilesPictureBox)).BeginInit();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.objectPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// LevelPanel
			// 
			this.LevelPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.LevelPanel.AutoScroll = true;
			this.LevelPanel.Controls.Add(this.levelPictureBox);
			this.LevelPanel.Location = new System.Drawing.Point(144, 54);
			this.LevelPanel.Name = "LevelPanel";
			this.LevelPanel.Size = new System.Drawing.Size(752, 405);
			this.LevelPanel.TabIndex = 1;
			this.LevelPanel.Visible = false;
			// 
			// levelPictureBox
			// 
			this.levelPictureBox.Location = new System.Drawing.Point(3, 3);
			this.levelPictureBox.Name = "levelPictureBox";
			this.levelPictureBox.Size = new System.Drawing.Size(4096, 512);
			this.levelPictureBox.TabIndex = 2;
			this.levelPictureBox.TabStop = false;
			this.levelPictureBox.Click += new System.EventHandler(this.LevelPictureBoxClick);
			this.levelPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.LevelPictureBoxPaint);
			this.levelPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LevelPictureBoxMouseDown);
			this.levelPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LevelPictureBoxMouseMove);
			// 
			// tilesPictureBox
			// 
			this.tilesPictureBox.Location = new System.Drawing.Point(10, 54);
			this.tilesPictureBox.Name = "tilesPictureBox";
			this.tilesPictureBox.Size = new System.Drawing.Size(128, 256);
			this.tilesPictureBox.TabIndex = 3;
			this.tilesPictureBox.TabStop = false;
			this.tilesPictureBox.Visible = false;
			this.tilesPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ObjectsPictureBoxPaint);
			this.tilesPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ObjectsPictureBoxMouseDown);
			// 
			// levelComboBox
			// 
			this.levelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.levelComboBox.FormattingEnabled = true;
			this.levelComboBox.Location = new System.Drawing.Point(12, 27);
			this.levelComboBox.Name = "levelComboBox";
			this.levelComboBox.Size = new System.Drawing.Size(184, 21);
			this.levelComboBox.TabIndex = 4;
			this.levelComboBox.Visible = false;
			this.levelComboBox.SelectedIndexChanged += new System.EventHandler(this.LevelComboBoxSelectedIndexChanged);
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
			// objectPictureBox
			// 
			this.objectPictureBox.Location = new System.Drawing.Point(10, 316);
			this.objectPictureBox.Name = "objectPictureBox";
			this.objectPictureBox.Size = new System.Drawing.Size(64, 64);
			this.objectPictureBox.TabIndex = 6;
			this.objectPictureBox.TabStop = false;
			this.objectPictureBox.Visible = false;
			this.objectPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.TilesPictureBoxPaint);
			this.objectPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TilesPictureBoxMouseDown);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(908, 471);
			this.Controls.Add(this.objectPictureBox);
			this.Controls.Add(this.levelComboBox);
			this.Controls.Add(this.tilesPictureBox);
			this.Controls.Add(this.LevelPanel);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "WLEditor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.LevelPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.levelPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tilesPictureBox)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.objectPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.PictureBox objectPictureBox;
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
		private System.Windows.Forms.ComboBox levelComboBox;
		private System.Windows.Forms.PictureBox tilesPictureBox;
		private System.Windows.Forms.Panel LevelPanel;
		private System.Windows.Forms.PictureBox levelPictureBox;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem classicToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blackWhiteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem autumnToolStripMenuItem;
		

			
	
		
	}
}
