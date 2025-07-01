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
			this.panelCamera = new System.Windows.Forms.Panel();
			this.labWarioStatusLevel = new System.Windows.Forms.Label();
			this.ddlWarioStatus = new System.Windows.Forms.ComboBox();
			this.panelTileset = new System.Windows.Forms.Panel();
			this.labWarioStatusSector = new System.Windows.Forms.Label();
			this.ddlWarioAttributesSector = new System.Windows.Forms.ComboBox();
			this.panelMusic = new System.Windows.Forms.Panel();
			this.checkBoxCheckpoint = new System.Windows.Forms.CheckBox();
			this.panelStatusLevel = new System.Windows.Forms.Panel();
			this.ddlWarioAttributesLevel = new System.Windows.Forms.ComboBox();
			this.panelStatusSector = new System.Windows.Forms.Panel();
			this.panelWarp = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.ddlWarpType = new System.Windows.Forms.ComboBox();
			this.panelScroll = new System.Windows.Forms.Panel();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioX)).BeginInit();
			this.panelCamera.SuspendLayout();
			this.panelTileset.SuspendLayout();
			this.panelMusic.SuspendLayout();
			this.panelStatusLevel.SuspendLayout();
			this.panelStatusSector.SuspendLayout();
			this.panelWarp.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.panelScroll.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labWarp
			// 
			this.labWarp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labWarp.Location = new System.Drawing.Point(9, 2);
			this.labWarp.Name = "labWarp";
			this.labWarp.Size = new System.Drawing.Size(67, 20);
			this.labWarp.TabIndex = 4;
			this.labWarp.Text = "Target";
			this.labWarp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarp
			// 
			this.ddlWarp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarp.DropDownWidth = 300;
			this.ddlWarp.FormattingEnabled = true;
			this.ddlWarp.Location = new System.Drawing.Point(88, 3);
			this.ddlWarp.Name = "ddlWarp";
			this.ddlWarp.Size = new System.Drawing.Size(295, 21);
			this.ddlWarp.TabIndex = 3;
			this.ddlWarp.SelectedIndexChanged += new System.EventHandler(this.DdlWarpSelectedIndexChanged);
			// 
			// checkBoxRight
			// 
			this.checkBoxRight.Location = new System.Drawing.Point(138, 3);
			this.checkBoxRight.Name = "checkBoxRight";
			this.checkBoxRight.Size = new System.Drawing.Size(51, 20);
			this.checkBoxRight.TabIndex = 2;
			this.checkBoxRight.Text = "Right";
			this.checkBoxRight.UseVisualStyleBackColor = true;
			this.checkBoxRight.CheckedChanged += new System.EventHandler(this.CheckBoxRightCheckedChanged);
			// 
			// checkBoxLeft
			// 
			this.checkBoxLeft.Location = new System.Drawing.Point(85, 3);
			this.checkBoxLeft.Name = "checkBoxLeft";
			this.checkBoxLeft.Size = new System.Drawing.Size(47, 20);
			this.checkBoxLeft.TabIndex = 1;
			this.checkBoxLeft.Text = "Left";
			this.checkBoxLeft.UseVisualStyleBackColor = true;
			this.checkBoxLeft.CheckedChanged += new System.EventHandler(this.CheckBoxLeftCheckedChanged);
			// 
			// labScroll
			// 
			this.labScroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labScroll.Location = new System.Drawing.Point(8, 1);
			this.labScroll.Name = "labScroll";
			this.labScroll.Size = new System.Drawing.Size(71, 20);
			this.labScroll.TabIndex = 10;
			this.labScroll.Text = "Camera limits";
			this.labScroll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlTileSet
			// 
			this.ddlTileSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlTileSet.FormattingEnabled = true;
			this.ddlTileSet.Location = new System.Drawing.Point(85, 11);
			this.ddlTileSet.Name = "ddlTileSet";
			this.ddlTileSet.Size = new System.Drawing.Size(295, 21);
			this.ddlTileSet.TabIndex = 4;
			this.ddlTileSet.SelectedIndexChanged += new System.EventHandler(this.DdlTileSetSelectedIndexChanged);
			// 
			// labAnimation
			// 
			this.labAnimation.Location = new System.Drawing.Point(6, 38);
			this.labAnimation.Name = "labAnimation";
			this.labAnimation.Size = new System.Drawing.Size(72, 18);
			this.labAnimation.TabIndex = 41;
			this.labAnimation.Text = "Animation";
			this.labAnimation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlAnimation
			// 
			this.ddlAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlAnimation.FormattingEnabled = true;
			this.ddlAnimation.Location = new System.Drawing.Point(85, 37);
			this.ddlAnimation.Name = "ddlAnimation";
			this.ddlAnimation.Size = new System.Drawing.Size(144, 21);
			this.ddlAnimation.TabIndex = 5;
			this.ddlAnimation.SelectedIndexChanged += new System.EventHandler(this.DdlAnimationSelectedIndexChanged);
			// 
			// labTileSet
			// 
			this.labTileSet.Location = new System.Drawing.Point(32, 12);
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
			this.ddlAnimationSpeed.Location = new System.Drawing.Point(235, 37);
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
			this.labEnemies.Location = new System.Drawing.Point(14, 65);
			this.labEnemies.Name = "labEnemies";
			this.labEnemies.Size = new System.Drawing.Size(64, 38);
			this.labEnemies.TabIndex = 16;
			this.labEnemies.Text = "Enemy";
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
			this.ddlEnemies.Location = new System.Drawing.Point(85, 65);
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
			this.labMusic.Location = new System.Drawing.Point(30, 3);
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
			this.ddlMusic.Size = new System.Drawing.Size(298, 21);
			this.ddlMusic.TabIndex = 0;
			this.ddlMusic.SelectedIndexChanged += new System.EventHandler(this.DdlMusicSelectedIndexChanged);
			// 
			// panelCamera
			// 
			this.panelCamera.AutoSize = true;
			this.panelCamera.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelCamera.Controls.Add(this.labCameraType);
			this.panelCamera.Controls.Add(this.txbWarioY);
			this.panelCamera.Controls.Add(this.txbCameraX);
			this.panelCamera.Controls.Add(this.labWario);
			this.panelCamera.Controls.Add(this.labCamera);
			this.panelCamera.Controls.Add(this.txbCameraY);
			this.panelCamera.Controls.Add(this.txbWarioX);
			this.panelCamera.Controls.Add(this.ddlCameraType);
			this.panelCamera.Controls.Add(this.cmdCalculatePos);
			this.panelCamera.Location = new System.Drawing.Point(0, 160);
			this.panelCamera.Margin = new System.Windows.Forms.Padding(0);
			this.panelCamera.Name = "panelCamera";
			this.panelCamera.Size = new System.Drawing.Size(383, 93);
			this.panelCamera.TabIndex = 3;
			// 
			// labWarioStatusLevel
			// 
			this.labWarioStatusLevel.Location = new System.Drawing.Point(11, 4);
			this.labWarioStatusLevel.Name = "labWarioStatusLevel";
			this.labWarioStatusLevel.Size = new System.Drawing.Size(66, 18);
			this.labWarioStatusLevel.TabIndex = 30;
			this.labWarioStatusLevel.Text = "Wario status";
			this.labWarioStatusLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarioStatus
			// 
			this.ddlWarioStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarioStatus.FormattingEnabled = true;
			this.ddlWarioStatus.Location = new System.Drawing.Point(239, 3);
			this.ddlWarioStatus.Name = "ddlWarioStatus";
			this.ddlWarioStatus.Size = new System.Drawing.Size(141, 21);
			this.ddlWarioStatus.TabIndex = 29;
			this.ddlWarioStatus.SelectedIndexChanged += new System.EventHandler(this.DdlWarioStatus_SelectedIndexChanged);
			// 
			// panelTileset
			// 
			this.panelTileset.AutoSize = true;
			this.panelTileset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelTileset.Controls.Add(this.ddlTileSet);
			this.panelTileset.Controls.Add(this.labTileSet);
			this.panelTileset.Controls.Add(this.ddlAnimation);
			this.panelTileset.Controls.Add(this.labEnemies);
			this.panelTileset.Controls.Add(this.labAnimation);
			this.panelTileset.Controls.Add(this.ddlEnemies);
			this.panelTileset.Controls.Add(this.ddlAnimationSpeed);
			this.panelTileset.Location = new System.Drawing.Point(0, 54);
			this.panelTileset.Margin = new System.Windows.Forms.Padding(0);
			this.panelTileset.Name = "panelTileset";
			this.panelTileset.Size = new System.Drawing.Size(383, 106);
			this.panelTileset.TabIndex = 2;
			// 
			// labWarioStatusSector
			// 
			this.labWarioStatusSector.Location = new System.Drawing.Point(11, 4);
			this.labWarioStatusSector.Name = "labWarioStatusSector";
			this.labWarioStatusSector.Size = new System.Drawing.Size(66, 18);
			this.labWarioStatusSector.TabIndex = 33;
			this.labWarioStatusSector.Text = "Wario status";
			this.labWarioStatusSector.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarioAttributesSector
			// 
			this.ddlWarioAttributesSector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarioAttributesSector.FormattingEnabled = true;
			this.ddlWarioAttributesSector.Location = new System.Drawing.Point(85, 3);
			this.ddlWarioAttributesSector.Name = "ddlWarioAttributesSector";
			this.ddlWarioAttributesSector.Size = new System.Drawing.Size(295, 21);
			this.ddlWarioAttributesSector.TabIndex = 32;
			this.ddlWarioAttributesSector.SelectedIndexChanged += new System.EventHandler(this.DdlWarioAttributesSector_SelectedIndexChanged);
			// 
			// panelMusic
			// 
			this.panelMusic.AutoSize = true;
			this.panelMusic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelMusic.Controls.Add(this.ddlMusic);
			this.panelMusic.Controls.Add(this.labMusic);
			this.panelMusic.Location = new System.Drawing.Point(0, 27);
			this.panelMusic.Margin = new System.Windows.Forms.Padding(0);
			this.panelMusic.Name = "panelMusic";
			this.panelMusic.Size = new System.Drawing.Size(386, 27);
			this.panelMusic.TabIndex = 0;
			// 
			// checkBoxCheckpoint
			// 
			this.checkBoxCheckpoint.Location = new System.Drawing.Point(3, 57);
			this.checkBoxCheckpoint.Name = "checkBoxCheckpoint";
			this.checkBoxCheckpoint.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkBoxCheckpoint.Size = new System.Drawing.Size(96, 20);
			this.checkBoxCheckpoint.TabIndex = 48;
			this.checkBoxCheckpoint.Text = "Checkpoint";
			this.checkBoxCheckpoint.CheckedChanged += new System.EventHandler(this.CheckBoxCheckpoint_CheckedChanged);
			// 
			// panelStatusLevel
			// 
			this.panelStatusLevel.AutoSize = true;
			this.panelStatusLevel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelStatusLevel.Controls.Add(this.ddlWarioAttributesLevel);
			this.panelStatusLevel.Controls.Add(this.labWarioStatusLevel);
			this.panelStatusLevel.Controls.Add(this.ddlWarioStatus);
			this.panelStatusLevel.Location = new System.Drawing.Point(0, 0);
			this.panelStatusLevel.Margin = new System.Windows.Forms.Padding(0);
			this.panelStatusLevel.Name = "panelStatusLevel";
			this.panelStatusLevel.Size = new System.Drawing.Size(383, 27);
			this.panelStatusLevel.TabIndex = 0;
			// 
			// ddlWarioAttributesLevel
			// 
			this.ddlWarioAttributesLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarioAttributesLevel.FormattingEnabled = true;
			this.ddlWarioAttributesLevel.Location = new System.Drawing.Point(85, 3);
			this.ddlWarioAttributesLevel.Name = "ddlWarioAttributesLevel";
			this.ddlWarioAttributesLevel.Size = new System.Drawing.Size(148, 21);
			this.ddlWarioAttributesLevel.TabIndex = 31;
			this.ddlWarioAttributesLevel.SelectedIndexChanged += new System.EventHandler(this.DdlWarioAttributesLevel_SelectedIndexChanged);
			// 
			// panelStatusSector
			// 
			this.panelStatusSector.AutoSize = true;
			this.panelStatusSector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelStatusSector.Controls.Add(this.ddlWarioAttributesSector);
			this.panelStatusSector.Controls.Add(this.labWarioStatusSector);
			this.panelStatusSector.Location = new System.Drawing.Point(0, 27);
			this.panelStatusSector.Margin = new System.Windows.Forms.Padding(0);
			this.panelStatusSector.Name = "panelStatusSector";
			this.panelStatusSector.Size = new System.Drawing.Size(383, 27);
			this.panelStatusSector.TabIndex = 0;
			// 
			// panelWarp
			// 
			this.panelWarp.AutoSize = true;
			this.panelWarp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelWarp.Controls.Add(this.labWarp);
			this.panelWarp.Controls.Add(this.ddlWarp);
			this.panelWarp.Location = new System.Drawing.Point(0, 106);
			this.panelWarp.Margin = new System.Windows.Forms.Padding(0);
			this.panelWarp.Name = "panelWarp";
			this.panelWarp.Size = new System.Drawing.Size(386, 27);
			this.panelWarp.TabIndex = 1;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(207)))), ((int)(((byte)(192)))));
			this.flowLayoutPanel1.Controls.Add(this.ddlWarpType);
			this.flowLayoutPanel1.Controls.Add(this.panelMusic);
			this.flowLayoutPanel1.Controls.Add(this.checkBoxCheckpoint);
			this.flowLayoutPanel1.Controls.Add(this.panelScroll);
			this.flowLayoutPanel1.Controls.Add(this.panelWarp);
			this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 8);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(389, 392);
			this.flowLayoutPanel1.TabIndex = 46;
			// 
			// ddlWarpType
			// 
			this.ddlWarpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarpType.FormattingEnabled = true;
			this.ddlWarpType.ImeMode = System.Windows.Forms.ImeMode.On;
			this.ddlWarpType.Items.AddRange(new object[] {
            "Level header",
            "Checkpoint",
            "Sector 00",
            "Sector 01",
            "Sector 02",
            "Sector 03",
            "Sector 04",
            "Sector 05",
            "Sector 06",
            "Sector 07",
            "Sector 08",
            "Sector 09",
            "Sector 10",
            "Sector 11",
            "Sector 12",
            "Sector 13",
            "Sector 14",
            "Sector 15",
            "Sector 16",
            "Sector 17",
            "Sector 18",
            "Sector 19",
            "Sector 20",
            "Sector 21",
            "Sector 22",
            "Sector 23",
            "Sector 24",
            "Sector 25",
            "Sector 26",
            "Sector 27",
            "Sector 28",
            "Sector 29",
            "Sector 30",
            "Sector 31",
            "Treasure A",
            "Treasure B",
            "Treasure C",
            "Treasure D",
            "Treasure E",
            "Treasure F",
            "Treasure G",
            "Treasure H",
            "Treasure I",
            "Treasure J",
            "Treasure K",
            "Treasure L",
            "Treasure M",
            "Treasure N",
            "Treasure O"});
			this.ddlWarpType.Location = new System.Drawing.Point(3, 3);
			this.ddlWarpType.Name = "ddlWarpType";
			this.ddlWarpType.Size = new System.Drawing.Size(383, 21);
			this.ddlWarpType.TabIndex = 47;
			this.ddlWarpType.SelectedIndexChanged += new System.EventHandler(this.DdlWarpType_SelectedIndexChanged);
			// 
			// panelScroll
			// 
			this.panelScroll.AutoSize = true;
			this.panelScroll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelScroll.Controls.Add(this.checkBoxRight);
			this.panelScroll.Controls.Add(this.labScroll);
			this.panelScroll.Controls.Add(this.checkBoxLeft);
			this.panelScroll.Location = new System.Drawing.Point(0, 80);
			this.panelScroll.Margin = new System.Windows.Forms.Padding(0);
			this.panelScroll.Name = "panelScroll";
			this.panelScroll.Size = new System.Drawing.Size(192, 26);
			this.panelScroll.TabIndex = 48;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel2.BackColor = System.Drawing.SystemColors.Control;
			this.flowLayoutPanel2.Controls.Add(this.panelStatusLevel);
			this.flowLayoutPanel2.Controls.Add(this.panelStatusSector);
			this.flowLayoutPanel2.Controls.Add(this.panelTileset);
			this.flowLayoutPanel2.Controls.Add(this.panelCamera);
			this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 136);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(383, 253);
			this.flowLayoutPanel2.TabIndex = 48;
			// 
			// SectorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(416, 418);
			this.Controls.Add(this.flowLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SectorForm";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Sectors / Level header";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SectorFormFormClosing);
			this.VisibleChanged += new System.EventHandler(this.SectorFormVisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.txbCameraX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioX)).EndInit();
			this.panelCamera.ResumeLayout(false);
			this.panelTileset.ResumeLayout(false);
			this.panelMusic.ResumeLayout(false);
			this.panelStatusLevel.ResumeLayout(false);
			this.panelStatusSector.ResumeLayout(false);
			this.panelWarp.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.panelScroll.ResumeLayout(false);
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
	}
}
