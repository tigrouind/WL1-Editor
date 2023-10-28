namespace WLEditor
{
	partial class SectorForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ComboBox ddlWarp;
		private System.Windows.Forms.Label labWarp;
		private System.Windows.Forms.CheckBox checkBoxRight;
		private System.Windows.Forms.CheckBox checkBoxLeft;
		private System.Windows.Forms.Label labScroll;
		private System.Windows.Forms.ComboBox ddlTileSet;
		private System.Windows.Forms.Label labTileSet;
		private System.Windows.Forms.Label labEnemies;
		private System.Windows.Forms.ComboBox ddlEnemies;
		private System.Windows.Forms.Label labCamera;
		private System.Windows.Forms.Label labWario;
		private System.Windows.Forms.NumericUpDown txbWarioY;
		private System.Windows.Forms.NumericUpDown txbWarioX;
		private System.Windows.Forms.Label labCameraType;
		private System.Windows.Forms.Button cmdCalculatePos;
		private System.Windows.Forms.ComboBox ddlAnimation;
		private System.Windows.Forms.NumericUpDown txbCameraY;
		private System.Windows.Forms.NumericUpDown txbCameraX;
		private System.Windows.Forms.ComboBox ddlCameraType;
		private System.Windows.Forms.ComboBox ddlAnimationSpeed;
		private System.Windows.Forms.Label labAnimation;
		private System.Windows.Forms.Label labMusic;
		private System.Windows.Forms.ComboBox ddlMusic;

		/// <summary>
		/// Disposes resources used by the control.
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
			this.labWarp = new System.Windows.Forms.Label();
			this.ddlWarp = new System.Windows.Forms.ComboBox();
			this.checkBoxRight = new System.Windows.Forms.CheckBox();
			this.checkBoxLeft = new System.Windows.Forms.CheckBox();
			this.labScroll = new System.Windows.Forms.Label();
			this.ddlTileSet = new System.Windows.Forms.ComboBox();
			this.labAnimation = new System.Windows.Forms.Label();
			this.ddlAnimation = new System.Windows.Forms.ComboBox();
			this.labTileSet = new System.Windows.Forms.Label();
			this.ddlAnimationSpeed = new System.Windows.Forms.ComboBox();
			this.cmdCalculatePos = new System.Windows.Forms.Button();
			this.txbCameraX = new System.Windows.Forms.NumericUpDown();
			this.labEnemies = new System.Windows.Forms.Label();
			this.labWario = new System.Windows.Forms.Label();
			this.txbCameraY = new System.Windows.Forms.NumericUpDown();
			this.ddlCameraType = new System.Windows.Forms.ComboBox();
			this.txbWarioY = new System.Windows.Forms.NumericUpDown();
			this.labCameraType = new System.Windows.Forms.Label();
			this.ddlEnemies = new System.Windows.Forms.ComboBox();
			this.labCamera = new System.Windows.Forms.Label();
			this.txbWarioX = new System.Windows.Forms.NumericUpDown();
			this.labMusic = new System.Windows.Forms.Label();
			this.ddlMusic = new System.Windows.Forms.ComboBox();
			this.grpCamera = new System.Windows.Forms.GroupBox();
			this.labWarioStatus = new System.Windows.Forms.Label();
			this.ddlWarioStatus = new System.Windows.Forms.ComboBox();
			this.grpTileset = new System.Windows.Forms.GroupBox();
			this.panelMusic = new System.Windows.Forms.Panel();
			this.panelStatus = new System.Windows.Forms.Panel();
			this.ddlWarioAttributes = new System.Windows.Forms.ComboBox();
			this.panelWarp = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.labCheckpoint = new System.Windows.Forms.Label();
			this.checkBoxCheckpoint = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioX)).BeginInit();
			this.grpCamera.SuspendLayout();
			this.grpTileset.SuspendLayout();
			this.panelMusic.SuspendLayout();
			this.panelStatus.SuspendLayout();
			this.panelWarp.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labWarp
			// 
			this.labWarp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labWarp.Location = new System.Drawing.Point(28, 28);
			this.labWarp.Name = "labWarp";
			this.labWarp.Size = new System.Drawing.Size(48, 20);
			this.labWarp.TabIndex = 4;
			this.labWarp.Text = "Warp";
			this.labWarp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarp
			// 
			this.ddlWarp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarp.DropDownWidth = 300;
			this.ddlWarp.FormattingEnabled = true;
			this.ddlWarp.Location = new System.Drawing.Point(85, 29);
			this.ddlWarp.Name = "ddlWarp";
			this.ddlWarp.Size = new System.Drawing.Size(295, 21);
			this.ddlWarp.TabIndex = 3;
			this.ddlWarp.SelectedIndexChanged += new System.EventHandler(this.DdlWarpSelectedIndexChanged);
			// 
			// checkBoxRight
			// 
			this.checkBoxRight.Location = new System.Drawing.Point(105, 6);
			this.checkBoxRight.Name = "checkBoxRight";
			this.checkBoxRight.Size = new System.Drawing.Size(18, 20);
			this.checkBoxRight.TabIndex = 2;
			this.checkBoxRight.UseVisualStyleBackColor = true;
			this.checkBoxRight.CheckedChanged += new System.EventHandler(this.CheckBoxRightCheckedChanged);
			// 
			// checkBoxLeft
			// 
			this.checkBoxLeft.Location = new System.Drawing.Point(85, 6);
			this.checkBoxLeft.Name = "checkBoxLeft";
			this.checkBoxLeft.Size = new System.Drawing.Size(18, 20);
			this.checkBoxLeft.TabIndex = 1;
			this.checkBoxLeft.UseVisualStyleBackColor = true;
			this.checkBoxLeft.CheckedChanged += new System.EventHandler(this.CheckBoxLeftCheckedChanged);
			// 
			// labScroll
			// 
			this.labScroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labScroll.Location = new System.Drawing.Point(3, 4);
			this.labScroll.Name = "labScroll";
			this.labScroll.Size = new System.Drawing.Size(71, 20);
			this.labScroll.TabIndex = 10;
			this.labScroll.Text = "Scroll bounds";
			this.labScroll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlTileSet
			// 
			this.ddlTileSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlTileSet.FormattingEnabled = true;
			this.ddlTileSet.Location = new System.Drawing.Point(85, 14);
			this.ddlTileSet.Name = "ddlTileSet";
			this.ddlTileSet.Size = new System.Drawing.Size(295, 21);
			this.ddlTileSet.TabIndex = 4;
			this.ddlTileSet.SelectedIndexChanged += new System.EventHandler(this.DdlTileSetSelectedIndexChanged);
			// 
			// labAnimation
			// 
			this.labAnimation.Location = new System.Drawing.Point(6, 41);
			this.labAnimation.Name = "labAnimation";
			this.labAnimation.Size = new System.Drawing.Size(72, 18);
			this.labAnimation.TabIndex = 41;
			this.labAnimation.Text = "Animation set";
			this.labAnimation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlAnimation
			// 
			this.ddlAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlAnimation.FormattingEnabled = true;
			this.ddlAnimation.Location = new System.Drawing.Point(85, 40);
			this.ddlAnimation.Name = "ddlAnimation";
			this.ddlAnimation.Size = new System.Drawing.Size(144, 21);
			this.ddlAnimation.TabIndex = 5;
			this.ddlAnimation.SelectedIndexChanged += new System.EventHandler(this.DdlAnimationSelectedIndexChanged);
			// 
			// labTileSet
			// 
			this.labTileSet.Location = new System.Drawing.Point(32, 15);
			this.labTileSet.Name = "labTileSet";
			this.labTileSet.Size = new System.Drawing.Size(46, 18);
			this.labTileSet.TabIndex = 14;
			this.labTileSet.Text = "Tile set";
			this.labTileSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlAnimationSpeed
			// 
			this.ddlAnimationSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlAnimationSpeed.FormattingEnabled = true;
			this.ddlAnimationSpeed.Location = new System.Drawing.Point(235, 40);
			this.ddlAnimationSpeed.Name = "ddlAnimationSpeed";
			this.ddlAnimationSpeed.Size = new System.Drawing.Size(145, 21);
			this.ddlAnimationSpeed.TabIndex = 6;
			this.ddlAnimationSpeed.SelectedIndexChanged += new System.EventHandler(this.DdlAnimationSpeedSelectedIndexChanged);
			// 
			// cmdCalculatePos
			// 
			this.cmdCalculatePos.Location = new System.Drawing.Point(198, 44);
			this.cmdCalculatePos.Name = "cmdCalculatePos";
			this.cmdCalculatePos.Size = new System.Drawing.Size(182, 46);
			this.cmdCalculatePos.TabIndex = 13;
			this.cmdCalculatePos.Text = "Recalculate";
			this.cmdCalculatePos.UseVisualStyleBackColor = true;
			this.cmdCalculatePos.Click += new System.EventHandler(this.CmdCalculatePosClick);
			// 
			// txbCameraX
			// 
			this.txbCameraX.Location = new System.Drawing.Point(85, 70);
			this.txbCameraX.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
			this.txbCameraX.Name = "txbCameraX";
			this.txbCameraX.Size = new System.Drawing.Size(50, 20);
			this.txbCameraX.TabIndex = 11;
			this.txbCameraX.ValueChanged += new System.EventHandler(this.TxbCameraXValueChanged);
			// 
			// labEnemies
			// 
			this.labEnemies.Location = new System.Drawing.Point(14, 68);
			this.labEnemies.Name = "labEnemies";
			this.labEnemies.Size = new System.Drawing.Size(64, 38);
			this.labEnemies.TabIndex = 16;
			this.labEnemies.Text = "Enemy set";
			this.labEnemies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labWario
			// 
			this.labWario.Location = new System.Drawing.Point(10, 44);
			this.labWario.Name = "labWario";
			this.labWario.Size = new System.Drawing.Size(66, 20);
			this.labWario.TabIndex = 19;
			this.labWario.Text = "Wario X / Y";
			this.labWario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txbCameraY
			// 
			this.txbCameraY.Location = new System.Drawing.Point(142, 70);
			this.txbCameraY.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
			this.txbCameraY.Name = "txbCameraY";
			this.txbCameraY.Size = new System.Drawing.Size(50, 20);
			this.txbCameraY.TabIndex = 12;
			this.txbCameraY.ValueChanged += new System.EventHandler(this.TxbCameraYValueChanged);
			// 
			// ddlCameraType
			// 
			this.ddlCameraType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlCameraType.FormattingEnabled = true;
			this.ddlCameraType.Location = new System.Drawing.Point(85, 15);
			this.ddlCameraType.Name = "ddlCameraType";
			this.ddlCameraType.Size = new System.Drawing.Size(295, 21);
			this.ddlCameraType.TabIndex = 8;
			this.ddlCameraType.SelectedIndexChanged += new System.EventHandler(this.DdlCameraTypeSelectedIndexChanged);
			// 
			// txbWarioY
			// 
			this.txbWarioY.Location = new System.Drawing.Point(142, 43);
			this.txbWarioY.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
			this.txbWarioY.Name = "txbWarioY";
			this.txbWarioY.Size = new System.Drawing.Size(50, 20);
			this.txbWarioY.TabIndex = 10;
			this.txbWarioY.ValueChanged += new System.EventHandler(this.TxbWarioYValueChanged);
			// 
			// labCameraType
			// 
			this.labCameraType.Location = new System.Drawing.Point(14, 16);
			this.labCameraType.Name = "labCameraType";
			this.labCameraType.Size = new System.Drawing.Size(66, 18);
			this.labCameraType.TabIndex = 28;
			this.labCameraType.Text = "Camera type";
			this.labCameraType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlEnemies
			// 
			this.ddlEnemies.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.ddlEnemies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlEnemies.FormattingEnabled = true;
			this.ddlEnemies.IntegralHeight = false;
			this.ddlEnemies.ItemHeight = 32;
			this.ddlEnemies.Location = new System.Drawing.Point(85, 68);
			this.ddlEnemies.Name = "ddlEnemies";
			this.ddlEnemies.Size = new System.Drawing.Size(295, 38);
			this.ddlEnemies.TabIndex = 7;
			this.ddlEnemies.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DdlEnemiesDrawItem);
			this.ddlEnemies.SelectedIndexChanged += new System.EventHandler(this.DdlEnemiesSelectedIndexChanged);
			// 
			// labCamera
			// 
			this.labCamera.Location = new System.Drawing.Point(5, 70);
			this.labCamera.Name = "labCamera";
			this.labCamera.Size = new System.Drawing.Size(71, 18);
			this.labCamera.TabIndex = 20;
			this.labCamera.Text = "Camera X / Y";
			this.labCamera.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txbWarioX
			// 
			this.txbWarioX.Location = new System.Drawing.Point(85, 43);
			this.txbWarioX.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
			this.txbWarioX.Name = "txbWarioX";
			this.txbWarioX.Size = new System.Drawing.Size(50, 20);
			this.txbWarioX.TabIndex = 9;
			this.txbWarioX.ValueChanged += new System.EventHandler(this.TxbWarioXValueChanged);
			// 
			// labMusic
			// 
			this.labMusic.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labMusic.Location = new System.Drawing.Point(30, 2);
			this.labMusic.Name = "labMusic";
			this.labMusic.Size = new System.Drawing.Size(47, 20);
			this.labMusic.TabIndex = 14;
			this.labMusic.Text = "Music";
			this.labMusic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlMusic
			// 
			this.ddlMusic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlMusic.DropDownWidth = 300;
			this.ddlMusic.FormattingEnabled = true;
			this.ddlMusic.Location = new System.Drawing.Point(85, 3);
			this.ddlMusic.Name = "ddlMusic";
			this.ddlMusic.Size = new System.Drawing.Size(295, 21);
			this.ddlMusic.TabIndex = 0;
			this.ddlMusic.SelectedIndexChanged += new System.EventHandler(this.DdlMusicSelectedIndexChanged);
			// 
			// grpCamera
			// 
			this.grpCamera.AutoSize = true;
			this.grpCamera.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.grpCamera.Controls.Add(this.labCameraType);
			this.grpCamera.Controls.Add(this.txbWarioY);
			this.grpCamera.Controls.Add(this.txbCameraX);
			this.grpCamera.Controls.Add(this.labWario);
			this.grpCamera.Controls.Add(this.labCamera);
			this.grpCamera.Controls.Add(this.txbCameraY);
			this.grpCamera.Controls.Add(this.txbWarioX);
			this.grpCamera.Controls.Add(this.ddlCameraType);
			this.grpCamera.Controls.Add(this.cmdCalculatePos);
			this.grpCamera.Location = new System.Drawing.Point(0, 253);
			this.grpCamera.Margin = new System.Windows.Forms.Padding(0);
			this.grpCamera.Name = "grpCamera";
			this.grpCamera.Padding = new System.Windows.Forms.Padding(0);
			this.grpCamera.Size = new System.Drawing.Size(383, 106);
			this.grpCamera.TabIndex = 3;
			this.grpCamera.TabStop = false;
			// 
			// labWarioStatus
			// 
			this.labWarioStatus.Location = new System.Drawing.Point(11, 4);
			this.labWarioStatus.Name = "labWarioStatus";
			this.labWarioStatus.Size = new System.Drawing.Size(66, 18);
			this.labWarioStatus.TabIndex = 30;
			this.labWarioStatus.Text = "Wario status";
			this.labWarioStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarioStatus
			// 
			this.ddlWarioStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarioStatus.FormattingEnabled = true;
			this.ddlWarioStatus.Location = new System.Drawing.Point(85, 3);
			this.ddlWarioStatus.Name = "ddlWarioStatus";
			this.ddlWarioStatus.Size = new System.Drawing.Size(141, 21);
			this.ddlWarioStatus.TabIndex = 29;
			this.ddlWarioStatus.SelectedIndexChanged += new System.EventHandler(this.DdlWarioStatus_SelectedIndexChanged);
			// 
			// grpTileset
			// 
			this.grpTileset.AutoSize = true;
			this.grpTileset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.grpTileset.Controls.Add(this.ddlTileSet);
			this.grpTileset.Controls.Add(this.labTileSet);
			this.grpTileset.Controls.Add(this.ddlAnimation);
			this.grpTileset.Controls.Add(this.labEnemies);
			this.grpTileset.Controls.Add(this.labAnimation);
			this.grpTileset.Controls.Add(this.ddlEnemies);
			this.grpTileset.Controls.Add(this.ddlAnimationSpeed);
			this.grpTileset.Location = new System.Drawing.Point(0, 131);
			this.grpTileset.Margin = new System.Windows.Forms.Padding(0);
			this.grpTileset.Name = "grpTileset";
			this.grpTileset.Padding = new System.Windows.Forms.Padding(0);
			this.grpTileset.Size = new System.Drawing.Size(383, 122);
			this.grpTileset.TabIndex = 2;
			this.grpTileset.TabStop = false;
			// 
			// panelMusic
			// 
			this.panelMusic.AutoSize = true;
			this.panelMusic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelMusic.Controls.Add(this.labCheckpoint);
			this.panelMusic.Controls.Add(this.checkBoxCheckpoint);
			this.panelMusic.Controls.Add(this.ddlMusic);
			this.panelMusic.Controls.Add(this.labMusic);
			this.panelMusic.Location = new System.Drawing.Point(0, 27);
			this.panelMusic.Margin = new System.Windows.Forms.Padding(0);
			this.panelMusic.Name = "panelMusic";
			this.panelMusic.Size = new System.Drawing.Size(383, 51);
			this.panelMusic.TabIndex = 0;
			// 
			// panelStatus
			// 
			this.panelStatus.AutoSize = true;
			this.panelStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelStatus.Controls.Add(this.ddlWarioAttributes);
			this.panelStatus.Controls.Add(this.labWarioStatus);
			this.panelStatus.Controls.Add(this.ddlWarioStatus);
			this.panelStatus.Location = new System.Drawing.Point(0, 0);
			this.panelStatus.Margin = new System.Windows.Forms.Padding(0);
			this.panelStatus.Name = "panelStatus";
			this.panelStatus.Size = new System.Drawing.Size(383, 27);
			this.panelStatus.TabIndex = 0;
			// 
			// ddlWarioAttributes
			// 
			this.ddlWarioAttributes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarioAttributes.FormattingEnabled = true;
			this.ddlWarioAttributes.Location = new System.Drawing.Point(232, 3);
			this.ddlWarioAttributes.Name = "ddlWarioAttributes";
			this.ddlWarioAttributes.Size = new System.Drawing.Size(148, 21);
			this.ddlWarioAttributes.TabIndex = 31;
			this.ddlWarioAttributes.SelectedIndexChanged += new System.EventHandler(this.DdlWarioAttributes_SelectedIndexChanged);
			// 
			// panelWarp
			// 
			this.panelWarp.AutoSize = true;
			this.panelWarp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelWarp.Controls.Add(this.labScroll);
			this.panelWarp.Controls.Add(this.checkBoxLeft);
			this.panelWarp.Controls.Add(this.checkBoxRight);
			this.panelWarp.Controls.Add(this.labWarp);
			this.panelWarp.Controls.Add(this.ddlWarp);
			this.panelWarp.Location = new System.Drawing.Point(0, 78);
			this.panelWarp.Margin = new System.Windows.Forms.Padding(0);
			this.panelWarp.Name = "panelWarp";
			this.panelWarp.Size = new System.Drawing.Size(383, 53);
			this.panelWarp.TabIndex = 1;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this.panelStatus);
			this.flowLayoutPanel1.Controls.Add(this.panelMusic);
			this.flowLayoutPanel1.Controls.Add(this.panelWarp);
			this.flowLayoutPanel1.Controls.Add(this.grpTileset);
			this.flowLayoutPanel1.Controls.Add(this.grpCamera);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 8);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(383, 359);
			this.flowLayoutPanel1.TabIndex = 46;
			// 
			// labCheckpoint
			// 
			this.labCheckpoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labCheckpoint.Location = new System.Drawing.Point(6, 26);
			this.labCheckpoint.Name = "labCheckpoint";
			this.labCheckpoint.Size = new System.Drawing.Size(71, 20);
			this.labCheckpoint.TabIndex = 16;
			this.labCheckpoint.Text = "Checkpoint";
			this.labCheckpoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBoxCheckpoint
			// 
			this.checkBoxCheckpoint.Location = new System.Drawing.Point(85, 28);
			this.checkBoxCheckpoint.Name = "checkBoxCheckpoint";
			this.checkBoxCheckpoint.Size = new System.Drawing.Size(18, 20);
			this.checkBoxCheckpoint.TabIndex = 15;
			this.checkBoxCheckpoint.UseVisualStyleBackColor = true;
			this.checkBoxCheckpoint.CheckedChanged += new System.EventHandler(this.CheckBoxCheckpoint_CheckedChanged);
			// 
			// SectorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(409, 404);
			this.Controls.Add(this.flowLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SectorForm";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Warp / Level header";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SectorFormFormClosing);
			this.VisibleChanged += new System.EventHandler(this.SectorFormVisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.txbCameraX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioX)).EndInit();
			this.grpCamera.ResumeLayout(false);
			this.grpTileset.ResumeLayout(false);
			this.panelMusic.ResumeLayout(false);
			this.panelStatus.ResumeLayout(false);
			this.panelWarp.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.GroupBox grpCamera;
		private System.Windows.Forms.GroupBox grpTileset;
		private System.Windows.Forms.Panel panelMusic;
		private System.Windows.Forms.Panel panelStatus;
		private System.Windows.Forms.Panel panelWarp;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label labWarioStatus;
		private System.Windows.Forms.ComboBox ddlWarioStatus;
		private System.Windows.Forms.ComboBox ddlWarioAttributes;
		private System.Windows.Forms.Label labCheckpoint;
		private System.Windows.Forms.CheckBox checkBoxCheckpoint;
	}
}
