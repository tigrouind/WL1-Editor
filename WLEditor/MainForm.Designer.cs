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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.levelComboBox = new System.Windows.Forms.ComboBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.regionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.objectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scrollRegionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.collidersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tileNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoom100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoom200ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoom300ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoom400ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.classicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blackWhiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.autumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.switchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.LevelPanel = new System.Windows.Forms.Panel();
			this.levelPictureBox = new WLEditor.LevelPictureBox();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.LevelPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.levelPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// levelComboBox
			// 
			this.levelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.levelComboBox.FormattingEnabled = true;
			this.levelComboBox.Location = new System.Drawing.Point(3, 3);
			this.levelComboBox.Name = "levelComboBox";
			this.levelComboBox.Size = new System.Drawing.Size(252, 21);
			this.levelComboBox.TabIndex = 4;
			this.levelComboBox.Visible = false;
			this.levelComboBox.SelectedIndexChanged += new System.EventHandler(this.LevelComboBoxSelectedIndexChanged);
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.viewToolStripMenuItem,
			this.aboutToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1209, 24);
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
			this.toolboxToolStripMenuItem,
			this.regionsToolStripMenuItem,
			this.objectsToolStripMenuItem,
			this.scrollRegionToolStripMenuItem,
			this.collidersToolStripMenuItem,
			this.tileNumbersToolStripMenuItem,
			this.animationToolStripMenuItem,
			this.zoomToolStripMenuItem,
			this.paletteToolStripMenuItem,
			this.switchToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// toolboxToolStripMenuItem
			// 
			this.toolboxToolStripMenuItem.CheckOnClick = true;
			this.toolboxToolStripMenuItem.Enabled = false;
			this.toolboxToolStripMenuItem.Name = "toolboxToolStripMenuItem";
			this.toolboxToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.toolboxToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.toolboxToolStripMenuItem.Text = "Toolbox";
			this.toolboxToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToolboxToolStripMenuItemCheckedChanged);
			// 
			// regionsToolStripMenuItem
			// 
			this.regionsToolStripMenuItem.Checked = true;
			this.regionsToolStripMenuItem.CheckOnClick = true;
			this.regionsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.regionsToolStripMenuItem.Name = "regionsToolStripMenuItem";
			this.regionsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.regionsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
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
			this.objectsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
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
			this.scrollRegionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.scrollRegionToolStripMenuItem.Text = "Scroll boundaries";
			this.scrollRegionToolStripMenuItem.Click += new System.EventHandler(this.ScrollRegionToolStripMenuItemClick);
			// 
			// collidersToolStripMenuItem
			// 
			this.collidersToolStripMenuItem.CheckOnClick = true;
			this.collidersToolStripMenuItem.Name = "collidersToolStripMenuItem";
			this.collidersToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.collidersToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.collidersToolStripMenuItem.Text = "Colliders";
			this.collidersToolStripMenuItem.Click += new System.EventHandler(this.CollidersToolStripMenuItemClick);
			// 
			// tileNumbersToolStripMenuItem
			// 
			this.tileNumbersToolStripMenuItem.Name = "tileNumbersToolStripMenuItem";
			this.tileNumbersToolStripMenuItem.CheckOnClick = true;
			this.tileNumbersToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
			this.tileNumbersToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.tileNumbersToolStripMenuItem.Text = "Tile numbers";
			this.tileNumbersToolStripMenuItem.Click += new System.EventHandler(this.TileNumbersToolStripMenuItemClick);
			// zoomToolStripMenuItem
			// 
			this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.zoom100ToolStripMenuItem,
			this.zoom200ToolStripMenuItem,
			this.zoom300ToolStripMenuItem,
			this.zoom400ToolStripMenuItem,
			this.toolStripMenuItem1,
			this.zoomInToolStripMenuItem,
			this.zoomOutToolStripMenuItem});
			this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
			this.zoomToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.zoomToolStripMenuItem.Text = "Zoom";
			// 
			// zoom100ToolStripMenuItem
			// 
			this.zoom100ToolStripMenuItem.Checked = true;
			this.zoom100ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.zoom100ToolStripMenuItem.Name = "zoom100ToolStripMenuItem";
			this.zoom100ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
			this.zoom100ToolStripMenuItem.Text = "100%";
			this.zoom100ToolStripMenuItem.Click += new System.EventHandler(this.Zoom100ToolStripMenuItemClick);
			// 
			// zoom200ToolStripMenuItem
			// 
			this.zoom200ToolStripMenuItem.Name = "zoom200ToolStripMenuItem";
			this.zoom200ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
			this.zoom200ToolStripMenuItem.Text = "200%";
			this.zoom200ToolStripMenuItem.Click += new System.EventHandler(this.Zoom200ToolStripMenuItemClick);
			// 
			// zoom300ToolStripMenuItem
			// 
			this.zoom300ToolStripMenuItem.Name = "zoom300ToolStripMenuItem";
			this.zoom300ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
			this.zoom300ToolStripMenuItem.Text = "300%";
			this.zoom300ToolStripMenuItem.Click += new System.EventHandler(this.Zoom300ToolStripMenuItemClick);
			// 
			// zoom400ToolStripMenuItem
			// 
			this.zoom400ToolStripMenuItem.Name = "zoom400ToolStripMenuItem";
			this.zoom400ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
			this.zoom400ToolStripMenuItem.Text = "400%";
			this.zoom400ToolStripMenuItem.Click += new System.EventHandler(this.Zoom400ToolStripMenuItemClick);
			// 
			// zoomInToolStripMenuItem
			// 
			this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
			this.zoomInToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl +";
			this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			this.zoomInToolStripMenuItem.Text = "Zoom in";
			this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.ZoomInToolStripMenuItemClick);
			// 
			// zoomOutToolStripMenuItem
			// 
			this.zoomOutToolStripMenuItem.Enabled = false;
			this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
			this.zoomOutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl -";
			this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			this.zoomOutToolStripMenuItem.Text = "Zoom out";
			this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.ZoomOutToolStripMenuItemClick);
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
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.LevelPanel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.levelComboBox, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1209, 619);
			this.tableLayoutPanel1.TabIndex = 8;
			// 
			// LevelPanel
			// 
			this.LevelPanel.AutoScroll = true;
			this.LevelPanel.Controls.Add(this.levelPictureBox);
			this.LevelPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LevelPanel.Location = new System.Drawing.Point(3, 30);
			this.LevelPanel.Name = "LevelPanel";
			this.LevelPanel.Size = new System.Drawing.Size(1203, 586);
			this.LevelPanel.TabIndex = 5;
			this.LevelPanel.Visible = false;
			// 
			// levelPictureBox
			// 
			this.levelPictureBox.Location = new System.Drawing.Point(0, 0);
			this.levelPictureBox.Name = "levelPictureBox";
			this.levelPictureBox.Size = new System.Drawing.Size(4096, 512);
			this.levelPictureBox.TabIndex = 2;
			this.levelPictureBox.TabStop = false;
			this.levelPictureBox.TileMouseDown += new System.EventHandler<int>(this.LevelPictureBoxTileMouseDown);
			this.levelPictureBox.SectorChanged += new System.EventHandler<int>(this.LevelPictureBoxSectorChanged);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 6);
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Tick += new System.EventHandler(this.TimerTick);
			// 
			// animationToolStripMenuItem
			// 
			this.animationToolStripMenuItem.Checked = true;
			this.animationToolStripMenuItem.CheckOnClick = true;
			this.animationToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.animationToolStripMenuItem.Name = "animationToolStripMenuItem";
			this.animationToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
			this.animationToolStripMenuItem.Text = "Animation";
			this.animationToolStripMenuItem.Click += new System.EventHandler(this.AnimationToolStripMenuItemClick);
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1209, 643);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "WLEditor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.LevelPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.levelPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
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
		private System.Windows.Forms.Panel LevelPanel;
		LevelPictureBox levelPictureBox;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem classicToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blackWhiteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem autumnToolStripMenuItem;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoom100ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoom200ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoom300ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoom400ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolboxToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ToolStripMenuItem animationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tileNumbersToolStripMenuItem;

			
	
		
	}
}
