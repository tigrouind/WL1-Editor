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

				blocksForm.Dispose();
				objectsForm.Dispose();
				overworldForm.Dispose();
				sectorForm.Dispose();
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
			components = new System.ComponentModel.Container();
			levelComboBox = new System.Windows.Forms.ComboBox();
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pasteLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			regionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			objectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			collidersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			collectiblesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			switchBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			scrollBoundaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			zoom100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			zoom200ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			zoom300ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			zoom400ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			blocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			objectsFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			sectorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			overworldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			LevelPanel = new PanelUnScrollable();
			levelPictureBox = new LevelPictureBox();
			timer = new System.Windows.Forms.Timer(components);
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			menuStrip1.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			LevelPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)levelPictureBox).BeginInit();
			statusStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// levelComboBox
			// 
			levelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			levelComboBox.FormattingEnabled = true;
			levelComboBox.Location = new System.Drawing.Point(3, 3);
			levelComboBox.Name = "levelComboBox";
			levelComboBox.Size = new System.Drawing.Size(252, 28);
			levelComboBox.TabIndex = 4;
			levelComboBox.Visible = false;
			levelComboBox.SelectedIndexChanged += LevelComboBoxSelectedIndexChanged;
			// 
			// menuStrip1
			// 
			menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, windowToolStripMenuItem, aboutToolStripMenuItem });
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new System.Drawing.Size(1209, 28);
			menuStrip1.TabIndex = 5;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { loadToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
			fileToolStripMenuItem.Text = "&File";
			// 
			// loadToolStripMenuItem
			// 
			loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			loadToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
			loadToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			loadToolStripMenuItem.Text = "Load rom...";
			loadToolStripMenuItem.Click += LoadToolStripMenuItemClick;
			// 
			// saveToolStripMenuItem
			// 
			saveToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
			saveToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			saveToolStripMenuItem.Text = "Save";
			saveToolStripMenuItem.Click += SaveToolStripMenuItemClick;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S;
			saveAsToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			saveAsToolStripMenuItem.Text = "Save As...";
			saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItemClick;
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			exitToolStripMenuItem.Text = "Exit";
			exitToolStripMenuItem.Click += ExitToolStripMenuItemClick;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { copyLevelToolStripMenuItem, pasteLevelToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
			editToolStripMenuItem.Text = "Edit";
			// 
			// copyLevelToolStripMenuItem
			// 
			copyLevelToolStripMenuItem.Enabled = false;
			copyLevelToolStripMenuItem.Name = "copyLevelToolStripMenuItem";
			copyLevelToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
			copyLevelToolStripMenuItem.Text = "Copy level";
			copyLevelToolStripMenuItem.Click += CopyLevelToolStripMenuItem_Click;
			// 
			// pasteLevelToolStripMenuItem
			// 
			pasteLevelToolStripMenuItem.Enabled = false;
			pasteLevelToolStripMenuItem.Name = "pasteLevelToolStripMenuItem";
			pasteLevelToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
			pasteLevelToolStripMenuItem.Text = "Paste level";
			pasteLevelToolStripMenuItem.Click += PasteLevelToolStripMenuItem_Click;
			// 
			// viewToolStripMenuItem
			// 
			viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { regionsToolStripMenuItem, objectsToolStripMenuItem, collidersToolStripMenuItem, collectiblesToolStripMenuItem, switchBlockToolStripMenuItem, scrollBoundaryToolStripMenuItem, animationToolStripMenuItem, zoomToolStripMenuItem });
			viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
			viewToolStripMenuItem.Text = "&View";
			// 
			// regionsToolStripMenuItem
			// 
			regionsToolStripMenuItem.Checked = true;
			regionsToolStripMenuItem.CheckOnClick = true;
			regionsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			regionsToolStripMenuItem.Name = "regionsToolStripMenuItem";
			regionsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			regionsToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			regionsToolStripMenuItem.Text = "Sectors";
			regionsToolStripMenuItem.Click += RegionsToolStripMenuItemClick;
			// 
			// objectsToolStripMenuItem
			// 
			objectsToolStripMenuItem.Checked = true;
			objectsToolStripMenuItem.CheckOnClick = true;
			objectsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			objectsToolStripMenuItem.Name = "objectsToolStripMenuItem";
			objectsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
			objectsToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			objectsToolStripMenuItem.Text = "Objects";
			objectsToolStripMenuItem.Click += ObjectsToolStripMenuItemClick;
			// 
			// collidersToolStripMenuItem
			// 
			collidersToolStripMenuItem.Checked = true;
			collidersToolStripMenuItem.CheckOnClick = true;
			collidersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			collidersToolStripMenuItem.Name = "collidersToolStripMenuItem";
			collidersToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			collidersToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			collidersToolStripMenuItem.Text = "Block types";
			collidersToolStripMenuItem.Click += CollidersToolStripMenuItemClick;
			// 
			// collectiblesToolStripMenuItem
			// 
			collectiblesToolStripMenuItem.Checked = true;
			collectiblesToolStripMenuItem.CheckOnClick = true;
			collectiblesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			collectiblesToolStripMenuItem.Name = "collectiblesToolStripMenuItem";
			collectiblesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
			collectiblesToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			collectiblesToolStripMenuItem.Text = "Collectibles";
			collectiblesToolStripMenuItem.Click += CollectiblesToolStripMenuItem_Click;
			// 
			// switchBlockToolStripMenuItem
			// 
			switchBlockToolStripMenuItem.Enabled = false;
			switchBlockToolStripMenuItem.Name = "switchBlockToolStripMenuItem";
			switchBlockToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			switchBlockToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			switchBlockToolStripMenuItem.Text = "[!] Block mode";
			switchBlockToolStripMenuItem.Click += SwitchBlockToolStripMenuItem_Click;
			// 
			// scrollBoundaryToolStripMenuItem
			// 
			scrollBoundaryToolStripMenuItem.Name = "scrollBoundaryToolStripMenuItem";
			scrollBoundaryToolStripMenuItem.ShortcutKeyDisplayString = "";
			scrollBoundaryToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
			scrollBoundaryToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			scrollBoundaryToolStripMenuItem.Text = "Scroll boundaries";
			scrollBoundaryToolStripMenuItem.Click += ScrollBoundaryToolStripMenuItem_Click;
			// 
			// animationToolStripMenuItem
			// 
			animationToolStripMenuItem.Checked = true;
			animationToolStripMenuItem.CheckOnClick = true;
			animationToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			animationToolStripMenuItem.Name = "animationToolStripMenuItem";
			animationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
			animationToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			animationToolStripMenuItem.Text = "Animation";
			animationToolStripMenuItem.Click += AnimationToolStripMenuItemClick;
			// 
			// zoomToolStripMenuItem
			// 
			zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { zoom100ToolStripMenuItem, zoom200ToolStripMenuItem, zoom300ToolStripMenuItem, zoom400ToolStripMenuItem, toolStripMenuItem1, zoomInToolStripMenuItem, zoomOutToolStripMenuItem });
			zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
			zoomToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
			zoomToolStripMenuItem.Text = "Zoom";
			// 
			// zoom100ToolStripMenuItem
			// 
			zoom100ToolStripMenuItem.Checked = true;
			zoom100ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			zoom100ToolStripMenuItem.Name = "zoom100ToolStripMenuItem";
			zoom100ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			zoom100ToolStripMenuItem.Text = "100%";
			zoom100ToolStripMenuItem.Click += Zoom100ToolStripMenuItemClick;
			// 
			// zoom200ToolStripMenuItem
			// 
			zoom200ToolStripMenuItem.Name = "zoom200ToolStripMenuItem";
			zoom200ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			zoom200ToolStripMenuItem.Text = "200%";
			zoom200ToolStripMenuItem.Click += Zoom200ToolStripMenuItemClick;
			// 
			// zoom300ToolStripMenuItem
			// 
			zoom300ToolStripMenuItem.Name = "zoom300ToolStripMenuItem";
			zoom300ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			zoom300ToolStripMenuItem.Text = "300%";
			zoom300ToolStripMenuItem.Click += Zoom300ToolStripMenuItemClick;
			// 
			// zoom400ToolStripMenuItem
			// 
			zoom400ToolStripMenuItem.Name = "zoom400ToolStripMenuItem";
			zoom400ToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			zoom400ToolStripMenuItem.Text = "400%";
			zoom400ToolStripMenuItem.Click += Zoom400ToolStripMenuItemClick;
			// 
			// toolStripMenuItem1
			// 
			toolStripMenuItem1.Name = "toolStripMenuItem1";
			toolStripMenuItem1.Size = new System.Drawing.Size(183, 6);
			// 
			// zoomInToolStripMenuItem
			// 
			zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
			zoomInToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl +";
			zoomInToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			zoomInToolStripMenuItem.Text = "Zoom in";
			zoomInToolStripMenuItem.Click += ZoomInToolStripMenuItemClick;
			// 
			// zoomOutToolStripMenuItem
			// 
			zoomOutToolStripMenuItem.Enabled = false;
			zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
			zoomOutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl -";
			zoomOutToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			zoomOutToolStripMenuItem.Text = "Zoom out";
			zoomOutToolStripMenuItem.Click += ZoomOutToolStripMenuItemClick;
			// 
			// windowToolStripMenuItem
			// 
			windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { blocksToolStripMenuItem, objectsFormToolStripMenuItem, sectorsToolStripMenuItem, toolStripMenuItem2, overworldToolStripMenuItem });
			windowToolStripMenuItem.Name = "windowToolStripMenuItem";
			windowToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
			windowToolStripMenuItem.Text = "Window";
			// 
			// blocksToolStripMenuItem
			// 
			blocksToolStripMenuItem.CheckOnClick = true;
			blocksToolStripMenuItem.Enabled = false;
			blocksToolStripMenuItem.Name = "blocksToolStripMenuItem";
			blocksToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F1;
			blocksToolStripMenuItem.Size = new System.Drawing.Size(333, 24);
			blocksToolStripMenuItem.Text = "Blocks...";
			blocksToolStripMenuItem.CheckedChanged += BlocksToolStripMenuItemClick;
			// 
			// objectsFormToolStripMenuItem
			// 
			objectsFormToolStripMenuItem.CheckOnClick = true;
			objectsFormToolStripMenuItem.Enabled = false;
			objectsFormToolStripMenuItem.Name = "objectsFormToolStripMenuItem";
			objectsFormToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2;
			objectsFormToolStripMenuItem.Size = new System.Drawing.Size(333, 24);
			objectsFormToolStripMenuItem.Text = "Objects...";
			objectsFormToolStripMenuItem.CheckedChanged += ObjectsFormToolStripMenuItemClick;
			// 
			// sectorsToolStripMenuItem
			// 
			sectorsToolStripMenuItem.CheckOnClick = true;
			sectorsToolStripMenuItem.Enabled = false;
			sectorsToolStripMenuItem.Name = "sectorsToolStripMenuItem";
			sectorsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3;
			sectorsToolStripMenuItem.Size = new System.Drawing.Size(333, 24);
			sectorsToolStripMenuItem.Text = "Sectors / Level header...";
			sectorsToolStripMenuItem.CheckedChanged += SectorsToolStripMenuItemClick;
			// 
			// toolStripMenuItem2
			// 
			toolStripMenuItem2.Name = "toolStripMenuItem2";
			toolStripMenuItem2.Size = new System.Drawing.Size(330, 6);
			// 
			// overworldToolStripMenuItem
			// 
			overworldToolStripMenuItem.CheckOnClick = true;
			overworldToolStripMenuItem.Enabled = false;
			overworldToolStripMenuItem.Name = "overworldToolStripMenuItem";
			overworldToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F4;
			overworldToolStripMenuItem.Size = new System.Drawing.Size(333, 24);
			overworldToolStripMenuItem.Text = "Overworld...";
			overworldToolStripMenuItem.CheckedChanged += OverworldToolStripMenuItemClick;
			// 
			// aboutToolStripMenuItem
			// 
			aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
			aboutToolStripMenuItem.Text = "About";
			aboutToolStripMenuItem.Click += AboutToolStripMenuItemClick;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(LevelPanel, 0, 1);
			tableLayoutPanel1.Controls.Add(levelComboBox, 0, 0);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 28);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 2;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new System.Drawing.Size(1209, 593);
			tableLayoutPanel1.TabIndex = 8;
			// 
			// LevelPanel
			// 
			LevelPanel.AutoScroll = true;
			LevelPanel.Controls.Add(levelPictureBox);
			LevelPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			LevelPanel.Location = new System.Drawing.Point(3, 37);
			LevelPanel.Name = "LevelPanel";
			LevelPanel.Size = new System.Drawing.Size(1203, 553);
			LevelPanel.TabIndex = 5;
			LevelPanel.Visible = false;
			// 
			// levelPictureBox
			// 
			levelPictureBox.Location = new System.Drawing.Point(0, 0);
			levelPictureBox.Name = "levelPictureBox";
			levelPictureBox.Size = new System.Drawing.Size(4096, 512);
			levelPictureBox.TabIndex = 2;
			levelPictureBox.TabStop = false;
			// 
			// timer
			// 
			timer.Enabled = true;
			timer.Interval = 120;
			timer.Tick += TimerTick;
			// 
			// statusStrip1
			// 
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
			statusStrip1.Location = new System.Drawing.Point(0, 621);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new System.Drawing.Size(1209, 22);
			statusStrip1.TabIndex = 3;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			// 
			// MainForm
			// 
			AllowDrop = true;
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			ClientSize = new System.Drawing.Size(1209, 643);
			Controls.Add(tableLayoutPanel1);
			Controls.Add(menuStrip1);
			Controls.Add(statusStrip1);
			MainMenuStrip = menuStrip1;
			Name = "MainForm";
			Text = "WLEditor";
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			tableLayoutPanel1.ResumeLayout(false);
			LevelPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)levelPictureBox).EndInit();
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem collidersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem objectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem regionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ComboBox levelComboBox;
		PanelUnScrollable LevelPanel;
		LevelPictureBox levelPictureBox;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoom100ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoom200ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoom300ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoom400ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ToolStripMenuItem animationToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripMenuItem collectiblesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blocksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sectorsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem overworldToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem switchBlockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem scrollBoundaryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem objectsFormToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyLevelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteLevelToolStripMenuItem;
	}
}
