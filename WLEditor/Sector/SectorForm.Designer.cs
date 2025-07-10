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
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}

				TilesEnemies.Dispose();
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
			labWarp = new System.Windows.Forms.Label();
			ddlWarp = new System.Windows.Forms.ComboBox();
			checkBoxRight = new System.Windows.Forms.CheckBox();
			checkBoxLeft = new System.Windows.Forms.CheckBox();
			labScroll = new System.Windows.Forms.Label();
			ddlTileSet = new System.Windows.Forms.ComboBox();
			labAnimation = new System.Windows.Forms.Label();
			ddlAnimation = new System.Windows.Forms.ComboBox();
			labTileSet = new System.Windows.Forms.Label();
			ddlAnimationSpeed = new System.Windows.Forms.ComboBox();
			cmdCalculatePos = new System.Windows.Forms.Button();
			txbCameraX = new System.Windows.Forms.NumericUpDown();
			labEnemies = new System.Windows.Forms.Label();
			labWario = new System.Windows.Forms.Label();
			txbCameraY = new System.Windows.Forms.NumericUpDown();
			ddlCameraType = new System.Windows.Forms.ComboBox();
			txbWarioY = new System.Windows.Forms.NumericUpDown();
			labCameraType = new System.Windows.Forms.Label();
			ddlEnemies = new System.Windows.Forms.ComboBox();
			labCamera = new System.Windows.Forms.Label();
			txbWarioX = new System.Windows.Forms.NumericUpDown();
			labMusic = new System.Windows.Forms.Label();
			ddlMusic = new System.Windows.Forms.ComboBox();
			panelCamera = new System.Windows.Forms.Panel();
			labWarioStatusLevel = new System.Windows.Forms.Label();
			ddlWarioStatus = new System.Windows.Forms.ComboBox();
			panelTileset = new System.Windows.Forms.Panel();
			labWarioStatusSector = new System.Windows.Forms.Label();
			ddlWarioAttributesSector = new System.Windows.Forms.ComboBox();
			panelMusic = new System.Windows.Forms.Panel();
			checkBoxCheckpoint = new System.Windows.Forms.CheckBox();
			panelStatusLevel = new System.Windows.Forms.Panel();
			ddlWarioAttributesLevel = new System.Windows.Forms.ComboBox();
			panelStatusSector = new System.Windows.Forms.Panel();
			panelWarp = new System.Windows.Forms.Panel();
			flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			ddlWarpType = new System.Windows.Forms.ComboBox();
			panelCheckpoint = new System.Windows.Forms.Panel();
			label1 = new System.Windows.Forms.Label();
			panelScroll = new System.Windows.Forms.Panel();
			flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)txbCameraX).BeginInit();
			((System.ComponentModel.ISupportInitialize)txbCameraY).BeginInit();
			((System.ComponentModel.ISupportInitialize)txbWarioY).BeginInit();
			((System.ComponentModel.ISupportInitialize)txbWarioX).BeginInit();
			panelCamera.SuspendLayout();
			panelTileset.SuspendLayout();
			panelMusic.SuspendLayout();
			panelStatusLevel.SuspendLayout();
			panelStatusSector.SuspendLayout();
			panelWarp.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			panelCheckpoint.SuspendLayout();
			panelScroll.SuspendLayout();
			flowLayoutPanel2.SuspendLayout();
			SuspendLayout();
			// 
			// labWarp
			// 
			labWarp.Location = new System.Drawing.Point(13, 3);
			labWarp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labWarp.Name = "labWarp";
			labWarp.Size = new System.Drawing.Size(89, 31);
			labWarp.TabIndex = 4;
			labWarp.Text = "Target";
			labWarp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarp
			// 
			ddlWarp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlWarp.DropDownWidth = 300;
			ddlWarp.FormattingEnabled = true;
			ddlWarp.Location = new System.Drawing.Point(117, 5);
			ddlWarp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlWarp.Name = "ddlWarp";
			ddlWarp.Size = new System.Drawing.Size(392, 28);
			ddlWarp.TabIndex = 3;
			ddlWarp.SelectedIndexChanged += DdlWarpSelectedIndexChanged;
			// 
			// checkBoxRight
			// 
			checkBoxRight.Location = new System.Drawing.Point(189, 5);
			checkBoxRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			checkBoxRight.Name = "checkBoxRight";
			checkBoxRight.Size = new System.Drawing.Size(68, 31);
			checkBoxRight.TabIndex = 2;
			checkBoxRight.Text = "Right";
			checkBoxRight.UseVisualStyleBackColor = true;
			checkBoxRight.CheckedChanged += CheckBoxRightCheckedChanged;
			// 
			// checkBoxLeft
			// 
			checkBoxLeft.Location = new System.Drawing.Point(118, 5);
			checkBoxLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			checkBoxLeft.Name = "checkBoxLeft";
			checkBoxLeft.Size = new System.Drawing.Size(63, 31);
			checkBoxLeft.TabIndex = 1;
			checkBoxLeft.Text = "Left";
			checkBoxLeft.UseVisualStyleBackColor = true;
			checkBoxLeft.CheckedChanged += CheckBoxLeftCheckedChanged;
			// 
			// labScroll
			// 
			labScroll.Location = new System.Drawing.Point(-4, 2);
			labScroll.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labScroll.Name = "labScroll";
			labScroll.Size = new System.Drawing.Size(106, 31);
			labScroll.TabIndex = 10;
			labScroll.Text = "Camera limits";
			labScroll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlTileSet
			// 
			ddlTileSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlTileSet.FormattingEnabled = true;
			ddlTileSet.Location = new System.Drawing.Point(113, 12);
			ddlTileSet.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlTileSet.Name = "ddlTileSet";
			ddlTileSet.Size = new System.Drawing.Size(392, 28);
			ddlTileSet.TabIndex = 4;
			ddlTileSet.SelectedIndexChanged += DdlTileSetSelectedIndexChanged;
			// 
			// labAnimation
			// 
			labAnimation.Location = new System.Drawing.Point(6, 49);
			labAnimation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labAnimation.Name = "labAnimation";
			labAnimation.Size = new System.Drawing.Size(96, 28);
			labAnimation.TabIndex = 41;
			labAnimation.Text = "Animation";
			labAnimation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlAnimation
			// 
			ddlAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlAnimation.FormattingEnabled = true;
			ddlAnimation.Location = new System.Drawing.Point(113, 48);
			ddlAnimation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlAnimation.Name = "ddlAnimation";
			ddlAnimation.Size = new System.Drawing.Size(191, 28);
			ddlAnimation.TabIndex = 5;
			ddlAnimation.SelectedIndexChanged += DdlAnimationSelectedIndexChanged;
			// 
			// labTileSet
			// 
			labTileSet.Location = new System.Drawing.Point(41, 12);
			labTileSet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labTileSet.Name = "labTileSet";
			labTileSet.Size = new System.Drawing.Size(61, 28);
			labTileSet.TabIndex = 14;
			labTileSet.Text = "Tile set";
			labTileSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlAnimationSpeed
			// 
			ddlAnimationSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlAnimationSpeed.FormattingEnabled = true;
			ddlAnimationSpeed.Location = new System.Drawing.Point(313, 48);
			ddlAnimationSpeed.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlAnimationSpeed.Name = "ddlAnimationSpeed";
			ddlAnimationSpeed.Size = new System.Drawing.Size(192, 28);
			ddlAnimationSpeed.TabIndex = 6;
			ddlAnimationSpeed.SelectedIndexChanged += DdlAnimationSpeedSelectedIndexChanged;
			// 
			// cmdCalculatePos
			// 
			cmdCalculatePos.Location = new System.Drawing.Point(264, 47);
			cmdCalculatePos.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			cmdCalculatePos.Name = "cmdCalculatePos";
			cmdCalculatePos.Size = new System.Drawing.Size(243, 71);
			cmdCalculatePos.TabIndex = 13;
			cmdCalculatePos.Text = "Recalculate";
			cmdCalculatePos.UseVisualStyleBackColor = true;
			cmdCalculatePos.Click += CmdCalculatePosClick;
			// 
			// txbCameraX
			// 
			txbCameraX.Location = new System.Drawing.Point(113, 90);
			txbCameraX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			txbCameraX.Maximum = new decimal(new int[] { 511, 0, 0, 0 });
			txbCameraX.Name = "txbCameraX";
			txbCameraX.Size = new System.Drawing.Size(67, 27);
			txbCameraX.TabIndex = 11;
			txbCameraX.ValueChanged += TxbCameraXValueChanged;
			// 
			// labEnemies
			// 
			labEnemies.Location = new System.Drawing.Point(17, 87);
			labEnemies.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labEnemies.Name = "labEnemies";
			labEnemies.Size = new System.Drawing.Size(85, 38);
			labEnemies.TabIndex = 16;
			labEnemies.Text = "Objects";
			labEnemies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labWario
			// 
			labWario.Location = new System.Drawing.Point(14, 49);
			labWario.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labWario.Name = "labWario";
			labWario.Size = new System.Drawing.Size(88, 31);
			labWario.TabIndex = 19;
			labWario.Text = "Wario X / Y";
			labWario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txbCameraY
			// 
			txbCameraY.Location = new System.Drawing.Point(189, 90);
			txbCameraY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			txbCameraY.Maximum = new decimal(new int[] { 63, 0, 0, 0 });
			txbCameraY.Name = "txbCameraY";
			txbCameraY.Size = new System.Drawing.Size(67, 27);
			txbCameraY.TabIndex = 12;
			txbCameraY.ValueChanged += TxbCameraYValueChanged;
			// 
			// ddlCameraType
			// 
			ddlCameraType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlCameraType.FormattingEnabled = true;
			ddlCameraType.Location = new System.Drawing.Point(113, 12);
			ddlCameraType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlCameraType.Name = "ddlCameraType";
			ddlCameraType.Size = new System.Drawing.Size(392, 28);
			ddlCameraType.TabIndex = 8;
			ddlCameraType.SelectedIndexChanged += DdlCameraTypeSelectedIndexChanged;
			// 
			// txbWarioY
			// 
			txbWarioY.Location = new System.Drawing.Point(189, 51);
			txbWarioY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			txbWarioY.Maximum = new decimal(new int[] { 63, 0, 0, 0 });
			txbWarioY.Name = "txbWarioY";
			txbWarioY.Size = new System.Drawing.Size(67, 27);
			txbWarioY.TabIndex = 10;
			txbWarioY.ValueChanged += TxbWarioYValueChanged;
			// 
			// labCameraType
			// 
			labCameraType.Location = new System.Drawing.Point(6, 12);
			labCameraType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labCameraType.Name = "labCameraType";
			labCameraType.Size = new System.Drawing.Size(96, 28);
			labCameraType.TabIndex = 28;
			labCameraType.Text = "Camera type";
			labCameraType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlEnemies
			// 
			ddlEnemies.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			ddlEnemies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlEnemies.FormattingEnabled = true;
			ddlEnemies.IntegralHeight = false;
			ddlEnemies.ItemHeight = 32;
			ddlEnemies.Location = new System.Drawing.Point(113, 87);
			ddlEnemies.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlEnemies.Name = "ddlEnemies";
			ddlEnemies.Size = new System.Drawing.Size(392, 38);
			ddlEnemies.TabIndex = 7;
			ddlEnemies.DrawItem += DdlEnemiesDrawItem;
			ddlEnemies.SelectedIndexChanged += DdlEnemiesSelectedIndexChanged;
			// 
			// labCamera
			// 
			labCamera.Location = new System.Drawing.Point(7, 89);
			labCamera.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labCamera.Name = "labCamera";
			labCamera.Size = new System.Drawing.Size(95, 28);
			labCamera.TabIndex = 20;
			labCamera.Text = "Camera X / Y";
			labCamera.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txbWarioX
			// 
			txbWarioX.Location = new System.Drawing.Point(113, 51);
			txbWarioX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			txbWarioX.Maximum = new decimal(new int[] { 511, 0, 0, 0 });
			txbWarioX.Name = "txbWarioX";
			txbWarioX.Size = new System.Drawing.Size(67, 27);
			txbWarioX.TabIndex = 9;
			txbWarioX.ValueChanged += TxbWarioXValueChanged;
			// 
			// labMusic
			// 
			labMusic.Location = new System.Drawing.Point(39, 3);
			labMusic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labMusic.Name = "labMusic";
			labMusic.Size = new System.Drawing.Size(63, 31);
			labMusic.TabIndex = 14;
			labMusic.Text = "Music";
			labMusic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlMusic
			// 
			ddlMusic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlMusic.DropDownWidth = 300;
			ddlMusic.FormattingEnabled = true;
			ddlMusic.Location = new System.Drawing.Point(118, 5);
			ddlMusic.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlMusic.Name = "ddlMusic";
			ddlMusic.Size = new System.Drawing.Size(392, 28);
			ddlMusic.TabIndex = 0;
			ddlMusic.SelectedIndexChanged += DdlMusicSelectedIndexChanged;
			// 
			// panelCamera
			// 
			panelCamera.AutoSize = true;
			panelCamera.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panelCamera.Controls.Add(labCameraType);
			panelCamera.Controls.Add(txbWarioY);
			panelCamera.Controls.Add(txbCameraX);
			panelCamera.Controls.Add(labWario);
			panelCamera.Controls.Add(labCamera);
			panelCamera.Controls.Add(txbCameraY);
			panelCamera.Controls.Add(txbWarioX);
			panelCamera.Controls.Add(ddlCameraType);
			panelCamera.Controls.Add(cmdCalculatePos);
			panelCamera.Location = new System.Drawing.Point(0, 206);
			panelCamera.Margin = new System.Windows.Forms.Padding(0);
			panelCamera.Name = "panelCamera";
			panelCamera.Size = new System.Drawing.Size(511, 123);
			panelCamera.TabIndex = 3;
			// 
			// labWarioStatusLevel
			// 
			labWarioStatusLevel.Location = new System.Drawing.Point(7, 6);
			labWarioStatusLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labWarioStatusLevel.Name = "labWarioStatusLevel";
			labWarioStatusLevel.Size = new System.Drawing.Size(95, 28);
			labWarioStatusLevel.TabIndex = 30;
			labWarioStatusLevel.Text = "Wario status";
			labWarioStatusLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarioStatus
			// 
			ddlWarioStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlWarioStatus.FormattingEnabled = true;
			ddlWarioStatus.Location = new System.Drawing.Point(319, 5);
			ddlWarioStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlWarioStatus.Name = "ddlWarioStatus";
			ddlWarioStatus.Size = new System.Drawing.Size(187, 28);
			ddlWarioStatus.TabIndex = 29;
			ddlWarioStatus.SelectedIndexChanged += DdlWarioStatus_SelectedIndexChanged;
			// 
			// panelTileset
			// 
			panelTileset.AutoSize = true;
			panelTileset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panelTileset.Controls.Add(ddlTileSet);
			panelTileset.Controls.Add(labTileSet);
			panelTileset.Controls.Add(ddlAnimation);
			panelTileset.Controls.Add(labEnemies);
			panelTileset.Controls.Add(labAnimation);
			panelTileset.Controls.Add(ddlEnemies);
			panelTileset.Controls.Add(ddlAnimationSpeed);
			panelTileset.Location = new System.Drawing.Point(0, 76);
			panelTileset.Margin = new System.Windows.Forms.Padding(0);
			panelTileset.Name = "panelTileset";
			panelTileset.Size = new System.Drawing.Size(509, 130);
			panelTileset.TabIndex = 2;
			// 
			// labWarioStatusSector
			// 
			labWarioStatusSector.Location = new System.Drawing.Point(6, 6);
			labWarioStatusSector.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labWarioStatusSector.Name = "labWarioStatusSector";
			labWarioStatusSector.Size = new System.Drawing.Size(96, 28);
			labWarioStatusSector.TabIndex = 33;
			labWarioStatusSector.Text = "Wario status";
			labWarioStatusSector.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarioAttributesSector
			// 
			ddlWarioAttributesSector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlWarioAttributesSector.FormattingEnabled = true;
			ddlWarioAttributesSector.Location = new System.Drawing.Point(113, 5);
			ddlWarioAttributesSector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlWarioAttributesSector.Name = "ddlWarioAttributesSector";
			ddlWarioAttributesSector.Size = new System.Drawing.Size(392, 28);
			ddlWarioAttributesSector.TabIndex = 32;
			ddlWarioAttributesSector.SelectedIndexChanged += DdlWarioAttributesSector_SelectedIndexChanged;
			// 
			// panelMusic
			// 
			panelMusic.AutoSize = true;
			panelMusic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panelMusic.Controls.Add(ddlMusic);
			panelMusic.Controls.Add(labMusic);
			panelMusic.Location = new System.Drawing.Point(0, 38);
			panelMusic.Margin = new System.Windows.Forms.Padding(0);
			panelMusic.Name = "panelMusic";
			panelMusic.Size = new System.Drawing.Size(514, 38);
			panelMusic.TabIndex = 0;
			// 
			// checkBoxCheckpoint
			// 
			checkBoxCheckpoint.Location = new System.Drawing.Point(118, 5);
			checkBoxCheckpoint.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			checkBoxCheckpoint.Name = "checkBoxCheckpoint";
			checkBoxCheckpoint.Size = new System.Drawing.Size(14, 31);
			checkBoxCheckpoint.TabIndex = 48;
			checkBoxCheckpoint.CheckedChanged += CheckBoxCheckpoint_CheckedChanged;
			// 
			// panelStatusLevel
			// 
			panelStatusLevel.AutoSize = true;
			panelStatusLevel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panelStatusLevel.Controls.Add(ddlWarioAttributesLevel);
			panelStatusLevel.Controls.Add(labWarioStatusLevel);
			panelStatusLevel.Controls.Add(ddlWarioStatus);
			panelStatusLevel.Location = new System.Drawing.Point(0, 0);
			panelStatusLevel.Margin = new System.Windows.Forms.Padding(0);
			panelStatusLevel.Name = "panelStatusLevel";
			panelStatusLevel.Size = new System.Drawing.Size(510, 38);
			panelStatusLevel.TabIndex = 0;
			// 
			// ddlWarioAttributesLevel
			// 
			ddlWarioAttributesLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlWarioAttributesLevel.FormattingEnabled = true;
			ddlWarioAttributesLevel.Location = new System.Drawing.Point(113, 5);
			ddlWarioAttributesLevel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlWarioAttributesLevel.Name = "ddlWarioAttributesLevel";
			ddlWarioAttributesLevel.Size = new System.Drawing.Size(196, 28);
			ddlWarioAttributesLevel.TabIndex = 31;
			ddlWarioAttributesLevel.SelectedIndexChanged += DdlWarioAttributesLevel_SelectedIndexChanged;
			// 
			// panelStatusSector
			// 
			panelStatusSector.AutoSize = true;
			panelStatusSector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panelStatusSector.Controls.Add(ddlWarioAttributesSector);
			panelStatusSector.Controls.Add(labWarioStatusSector);
			panelStatusSector.Location = new System.Drawing.Point(0, 38);
			panelStatusSector.Margin = new System.Windows.Forms.Padding(0);
			panelStatusSector.Name = "panelStatusSector";
			panelStatusSector.Size = new System.Drawing.Size(509, 38);
			panelStatusSector.TabIndex = 0;
			// 
			// panelWarp
			// 
			panelWarp.AutoSize = true;
			panelWarp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panelWarp.Controls.Add(labWarp);
			panelWarp.Controls.Add(ddlWarp);
			panelWarp.Location = new System.Drawing.Point(0, 158);
			panelWarp.Margin = new System.Windows.Forms.Padding(0);
			panelWarp.Name = "panelWarp";
			panelWarp.Size = new System.Drawing.Size(513, 38);
			panelWarp.TabIndex = 1;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(223, 207, 192);
			flowLayoutPanel1.Controls.Add(ddlWarpType);
			flowLayoutPanel1.Controls.Add(panelMusic);
			flowLayoutPanel1.Controls.Add(panelCheckpoint);
			flowLayoutPanel1.Controls.Add(panelScroll);
			flowLayoutPanel1.Controls.Add(panelWarp);
			flowLayoutPanel1.Controls.Add(flowLayoutPanel2);
			flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			flowLayoutPanel1.Location = new System.Drawing.Point(11, 12);
			flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new System.Drawing.Size(519, 535);
			flowLayoutPanel1.TabIndex = 46;
			// 
			// ddlWarpType
			// 
			ddlWarpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ddlWarpType.FormattingEnabled = true;
			ddlWarpType.ImeMode = System.Windows.Forms.ImeMode.On;
			ddlWarpType.Items.AddRange(new object[] { "Level header", "Checkpoint", "Sector 00", "Sector 01", "Sector 02", "Sector 03", "Sector 04", "Sector 05", "Sector 06", "Sector 07", "Sector 08", "Sector 09", "Sector 10", "Sector 11", "Sector 12", "Sector 13", "Sector 14", "Sector 15", "Sector 16", "Sector 17", "Sector 18", "Sector 19", "Sector 20", "Sector 21", "Sector 22", "Sector 23", "Sector 24", "Sector 25", "Sector 26", "Sector 27", "Sector 28", "Sector 29", "Sector 30", "Sector 31", "Treasure A", "Treasure B", "Treasure C", "Treasure D", "Treasure E", "Treasure F", "Treasure G", "Treasure H", "Treasure I", "Treasure J", "Treasure K", "Treasure L", "Treasure M", "Treasure N", "Treasure O" });
			ddlWarpType.Location = new System.Drawing.Point(4, 5);
			ddlWarpType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			ddlWarpType.Name = "ddlWarpType";
			ddlWarpType.Size = new System.Drawing.Size(509, 28);
			ddlWarpType.TabIndex = 47;
			ddlWarpType.SelectedIndexChanged += DdlWarpType_SelectedIndexChanged;
			// 
			// panelCheckpoint
			// 
			panelCheckpoint.AutoSize = true;
			panelCheckpoint.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panelCheckpoint.Controls.Add(checkBoxCheckpoint);
			panelCheckpoint.Controls.Add(label1);
			panelCheckpoint.Location = new System.Drawing.Point(0, 76);
			panelCheckpoint.Margin = new System.Windows.Forms.Padding(0);
			panelCheckpoint.Name = "panelCheckpoint";
			panelCheckpoint.Size = new System.Drawing.Size(136, 41);
			panelCheckpoint.TabIndex = 49;
			// 
			// label1
			// 
			label1.Location = new System.Drawing.Point(12, 6);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(90, 26);
			label1.TabIndex = 11;
			label1.Text = "Checkpoint";
			label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panelScroll
			// 
			panelScroll.AutoSize = true;
			panelScroll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panelScroll.Controls.Add(checkBoxRight);
			panelScroll.Controls.Add(labScroll);
			panelScroll.Controls.Add(checkBoxLeft);
			panelScroll.Location = new System.Drawing.Point(0, 117);
			panelScroll.Margin = new System.Windows.Forms.Padding(0);
			panelScroll.Name = "panelScroll";
			panelScroll.Size = new System.Drawing.Size(261, 41);
			panelScroll.TabIndex = 48;
			// 
			// flowLayoutPanel2
			// 
			flowLayoutPanel2.AutoSize = true;
			flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			flowLayoutPanel2.BackColor = System.Drawing.SystemColors.Control;
			flowLayoutPanel2.Controls.Add(panelStatusLevel);
			flowLayoutPanel2.Controls.Add(panelStatusSector);
			flowLayoutPanel2.Controls.Add(panelTileset);
			flowLayoutPanel2.Controls.Add(panelCamera);
			flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			flowLayoutPanel2.ForeColor = System.Drawing.SystemColors.ControlText;
			flowLayoutPanel2.Location = new System.Drawing.Point(4, 201);
			flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			flowLayoutPanel2.Name = "flowLayoutPanel2";
			flowLayoutPanel2.Size = new System.Drawing.Size(511, 329);
			flowLayoutPanel2.TabIndex = 48;
			// 
			// SectorForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			ClientSize = new System.Drawing.Size(555, 643);
			Controls.Add(flowLayoutPanel1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "SectorForm";
			Padding = new System.Windows.Forms.Padding(7, 8, 7, 8);
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Sectors / Level header";
			FormClosing += SectorFormFormClosing;
			VisibleChanged += SectorFormVisibleChanged;
			((System.ComponentModel.ISupportInitialize)txbCameraX).EndInit();
			((System.ComponentModel.ISupportInitialize)txbCameraY).EndInit();
			((System.ComponentModel.ISupportInitialize)txbWarioY).EndInit();
			((System.ComponentModel.ISupportInitialize)txbWarioX).EndInit();
			panelCamera.ResumeLayout(false);
			panelTileset.ResumeLayout(false);
			panelMusic.ResumeLayout(false);
			panelStatusLevel.ResumeLayout(false);
			panelStatusSector.ResumeLayout(false);
			panelWarp.ResumeLayout(false);
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			panelCheckpoint.ResumeLayout(false);
			panelScroll.ResumeLayout(false);
			flowLayoutPanel2.ResumeLayout(false);
			flowLayoutPanel2.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		private System.Windows.Forms.Panel panelCamera;
		private System.Windows.Forms.Panel panelTileset;
		private System.Windows.Forms.Panel panelMusic;
		private System.Windows.Forms.Panel panelStatusLevel;
		private System.Windows.Forms.Panel panelStatusSector;
		private System.Windows.Forms.Panel panelWarp;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label labWarioStatusLevel;
		private System.Windows.Forms.ComboBox ddlWarioStatus;
		private System.Windows.Forms.ComboBox ddlWarioAttributesLevel;
		private System.Windows.Forms.CheckBox checkBoxCheckpoint;
		private System.Windows.Forms.ComboBox ddlWarpType;
		private System.Windows.Forms.Label labWarioStatusSector;
		private System.Windows.Forms.ComboBox ddlWarioAttributesSector;
		private System.Windows.Forms.Panel panelScroll;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panelCheckpoint;
	}
}
