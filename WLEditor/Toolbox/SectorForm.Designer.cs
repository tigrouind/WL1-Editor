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
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;

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
			this.panel3 = new System.Windows.Forms.Panel();
			this.cmdCalculatePos = new System.Windows.Forms.Button();
			this.ddlTileSet = new System.Windows.Forms.ComboBox();
			this.labAnimation = new System.Windows.Forms.Label();
			this.txbCameraX = new System.Windows.Forms.NumericUpDown();
			this.labEnemies = new System.Windows.Forms.Label();
			this.labWario = new System.Windows.Forms.Label();
			this.ddlAnimation = new System.Windows.Forms.ComboBox();
			this.txbCameraY = new System.Windows.Forms.NumericUpDown();
			this.ddlCameraType = new System.Windows.Forms.ComboBox();
			this.txbWarioY = new System.Windows.Forms.NumericUpDown();
			this.labCameraType = new System.Windows.Forms.Label();
			this.ddlEnemies = new System.Windows.Forms.ComboBox();
			this.labTileSet = new System.Windows.Forms.Label();
			this.labCamera = new System.Windows.Forms.Label();
			this.txbWarioX = new System.Windows.Forms.NumericUpDown();
			this.ddlAnimationSpeed = new System.Windows.Forms.ComboBox();
			this.labMusic = new System.Windows.Forms.Label();
			this.ddlMusic = new System.Windows.Forms.ComboBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioX)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labWarp
			// 
			this.labWarp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labWarp.Location = new System.Drawing.Point(35, 30);
			this.labWarp.Name = "labWarp";
			this.labWarp.Size = new System.Drawing.Size(48, 20);
			this.labWarp.TabIndex = 4;
			this.labWarp.Text = "Warp";
			this.labWarp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlWarp
			// 
			this.ddlWarp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.ddlWarp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWarp.DropDownWidth = 300;
			this.ddlWarp.FormattingEnabled = true;
			this.ddlWarp.Location = new System.Drawing.Point(88, 30);
			this.ddlWarp.Name = "ddlWarp";
			this.ddlWarp.Size = new System.Drawing.Size(238, 21);
			this.ddlWarp.TabIndex = 5;
			this.ddlWarp.SelectedIndexChanged += new System.EventHandler(this.DdlWarpSelectedIndexChanged);
			// 
			// checkBoxRight
			// 
			this.checkBoxRight.Location = new System.Drawing.Point(108, 4);
			this.checkBoxRight.Name = "checkBoxRight";
			this.checkBoxRight.Size = new System.Drawing.Size(18, 24);
			this.checkBoxRight.TabIndex = 6;
			this.checkBoxRight.UseVisualStyleBackColor = true;
			this.checkBoxRight.CheckedChanged += new System.EventHandler(this.CheckBoxRightCheckedChanged);
			// 
			// checkBoxLeft
			// 
			this.checkBoxLeft.Location = new System.Drawing.Point(88, 4);
			this.checkBoxLeft.Name = "checkBoxLeft";
			this.checkBoxLeft.Size = new System.Drawing.Size(18, 24);
			this.checkBoxLeft.TabIndex = 7;
			this.checkBoxLeft.UseVisualStyleBackColor = true;
			this.checkBoxLeft.CheckedChanged += new System.EventHandler(this.CheckBoxLeftCheckedChanged);
			// 
			// labScroll
			// 
			this.labScroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labScroll.Location = new System.Drawing.Point(8, 4);
			this.labScroll.Name = "labScroll";
			this.labScroll.Size = new System.Drawing.Size(74, 20);
			this.labScroll.TabIndex = 10;
			this.labScroll.Text = "Scroll bounds";
			this.labScroll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel3
			// 
			this.panel3.AutoSize = true;
			this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel3.Controls.Add(this.cmdCalculatePos);
			this.panel3.Controls.Add(this.ddlTileSet);
			this.panel3.Controls.Add(this.labAnimation);
			this.panel3.Controls.Add(this.txbCameraX);
			this.panel3.Controls.Add(this.labEnemies);
			this.panel3.Controls.Add(this.labWario);
			this.panel3.Controls.Add(this.ddlAnimation);
			this.panel3.Controls.Add(this.txbCameraY);
			this.panel3.Controls.Add(this.ddlCameraType);
			this.panel3.Controls.Add(this.txbWarioY);
			this.panel3.Controls.Add(this.labCameraType);
			this.panel3.Controls.Add(this.ddlEnemies);
			this.panel3.Controls.Add(this.labTileSet);
			this.panel3.Controls.Add(this.labCamera);
			this.panel3.Controls.Add(this.txbWarioX);
			this.panel3.Controls.Add(this.ddlAnimationSpeed);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(3, 123);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(333, 193);
			this.panel3.TabIndex = 42;
			// 
			// cmdCalculatePos
			// 
			this.cmdCalculatePos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.cmdCalculatePos.Location = new System.Drawing.Point(201, 136);
			this.cmdCalculatePos.Name = "cmdCalculatePos";
			this.cmdCalculatePos.Size = new System.Drawing.Size(125, 47);
			this.cmdCalculatePos.TabIndex = 30;
			this.cmdCalculatePos.Text = "Recalculate";
			this.cmdCalculatePos.UseVisualStyleBackColor = true;
			this.cmdCalculatePos.Click += new System.EventHandler(this.CmdCalculatePosClick);
			// 
			// ddlTileSet
			// 
			this.ddlTileSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.ddlTileSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlTileSet.FormattingEnabled = true;
			this.ddlTileSet.Location = new System.Drawing.Point(88, 11);
			this.ddlTileSet.Name = "ddlTileSet";
			this.ddlTileSet.Size = new System.Drawing.Size(238, 21);
			this.ddlTileSet.TabIndex = 13;
			this.ddlTileSet.SelectedIndexChanged += new System.EventHandler(this.DdlTileSetSelectedIndexChanged);
			// 
			// labAnimation
			// 
			this.labAnimation.Location = new System.Drawing.Point(9, 41);
			this.labAnimation.Name = "labAnimation";
			this.labAnimation.Size = new System.Drawing.Size(72, 18);
			this.labAnimation.TabIndex = 41;
			this.labAnimation.Text = "Animation set";
			this.labAnimation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txbCameraX
			// 
			this.txbCameraX.Location = new System.Drawing.Point(88, 163);
			this.txbCameraX.Maximum = new decimal(new int[] {
			511,
			0,
			0,
			0});
			this.txbCameraX.Name = "txbCameraX";
			this.txbCameraX.Size = new System.Drawing.Size(50, 20);
			this.txbCameraX.TabIndex = 37;
			this.txbCameraX.ValueChanged += new System.EventHandler(this.TxbCameraXValueChanged);
			// 
			// labEnemies
			// 
			this.labEnemies.Location = new System.Drawing.Point(11, 68);
			this.labEnemies.Name = "labEnemies";
			this.labEnemies.Size = new System.Drawing.Size(71, 18);
			this.labEnemies.TabIndex = 16;
			this.labEnemies.Text = "Enemy set";
			this.labEnemies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labWario
			// 
			this.labWario.Location = new System.Drawing.Point(13, 137);
			this.labWario.Name = "labWario";
			this.labWario.Size = new System.Drawing.Size(66, 18);
			this.labWario.TabIndex = 19;
			this.labWario.Text = "Wario X / Y";
			this.labWario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlAnimation
			// 
			this.ddlAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlAnimation.FormattingEnabled = true;
			this.ddlAnimation.Location = new System.Drawing.Point(88, 40);
			this.ddlAnimation.Name = "ddlAnimation";
			this.ddlAnimation.Size = new System.Drawing.Size(144, 21);
			this.ddlAnimation.TabIndex = 31;
			this.ddlAnimation.SelectedIndexChanged += new System.EventHandler(this.DdlAnimationSelectedIndexChanged);
			// 
			// txbCameraY
			// 
			this.txbCameraY.Location = new System.Drawing.Point(145, 163);
			this.txbCameraY.Maximum = new decimal(new int[] {
			63,
			0,
			0,
			0});
			this.txbCameraY.Name = "txbCameraY";
			this.txbCameraY.Size = new System.Drawing.Size(50, 20);
			this.txbCameraY.TabIndex = 38;
			this.txbCameraY.ValueChanged += new System.EventHandler(this.TxbCameraYValueChanged);
			// 
			// ddlCameraType
			// 
			this.ddlCameraType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.ddlCameraType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlCameraType.FormattingEnabled = true;
			this.ddlCameraType.Location = new System.Drawing.Point(88, 108);
			this.ddlCameraType.Name = "ddlCameraType";
			this.ddlCameraType.Size = new System.Drawing.Size(238, 21);
			this.ddlCameraType.TabIndex = 40;
			this.ddlCameraType.SelectedIndexChanged += new System.EventHandler(this.DdlCameraTypeSelectedIndexChanged);
			// 
			// txbWarioY
			// 
			this.txbWarioY.Location = new System.Drawing.Point(145, 136);
			this.txbWarioY.Maximum = new decimal(new int[] {
			63,
			0,
			0,
			0});
			this.txbWarioY.Name = "txbWarioY";
			this.txbWarioY.Size = new System.Drawing.Size(50, 20);
			this.txbWarioY.TabIndex = 36;
			this.txbWarioY.ValueChanged += new System.EventHandler(this.TxbWarioYValueChanged);
			// 
			// labCameraType
			// 
			this.labCameraType.Location = new System.Drawing.Point(17, 109);
			this.labCameraType.Name = "labCameraType";
			this.labCameraType.Size = new System.Drawing.Size(66, 18);
			this.labCameraType.TabIndex = 28;
			this.labCameraType.Text = "Camera type";
			this.labCameraType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlEnemies
			// 
			this.ddlEnemies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.ddlEnemies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlEnemies.DropDownWidth = 380;
			this.ddlEnemies.FormattingEnabled = true;
			this.ddlEnemies.Location = new System.Drawing.Point(88, 68);
			this.ddlEnemies.Name = "ddlEnemies";
			this.ddlEnemies.Size = new System.Drawing.Size(238, 21);
			this.ddlEnemies.TabIndex = 15;
			this.ddlEnemies.SelectedIndexChanged += new System.EventHandler(this.DdlEnemiesSelectedIndexChanged);
			// 
			// labTileSet
			// 
			this.labTileSet.Location = new System.Drawing.Point(35, 12);
			this.labTileSet.Name = "labTileSet";
			this.labTileSet.Size = new System.Drawing.Size(46, 18);
			this.labTileSet.TabIndex = 14;
			this.labTileSet.Text = "Tile set";
			this.labTileSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labCamera
			// 
			this.labCamera.Location = new System.Drawing.Point(8, 163);
			this.labCamera.Name = "labCamera";
			this.labCamera.Size = new System.Drawing.Size(71, 18);
			this.labCamera.TabIndex = 20;
			this.labCamera.Text = "Camera X / Y";
			this.labCamera.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txbWarioX
			// 
			this.txbWarioX.Location = new System.Drawing.Point(88, 136);
			this.txbWarioX.Maximum = new decimal(new int[] {
			511,
			0,
			0,
			0});
			this.txbWarioX.Name = "txbWarioX";
			this.txbWarioX.Size = new System.Drawing.Size(50, 20);
			this.txbWarioX.TabIndex = 35;
			this.txbWarioX.ValueChanged += new System.EventHandler(this.TxbWarioXValueChanged);
			// 
			// ddlAnimationSpeed
			// 
			this.ddlAnimationSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.ddlAnimationSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlAnimationSpeed.FormattingEnabled = true;
			this.ddlAnimationSpeed.Location = new System.Drawing.Point(238, 40);
			this.ddlAnimationSpeed.Name = "ddlAnimationSpeed";
			this.ddlAnimationSpeed.Size = new System.Drawing.Size(88, 21);
			this.ddlAnimationSpeed.TabIndex = 39;
			this.ddlAnimationSpeed.SelectedIndexChanged += new System.EventHandler(this.DdlAnimationSpeedSelectedIndexChanged);
			// 
			// labMusic
			// 
			this.labMusic.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labMusic.Location = new System.Drawing.Point(37, 30);
			this.labMusic.Name = "labMusic";
			this.labMusic.Size = new System.Drawing.Size(47, 20);
			this.labMusic.TabIndex = 14;
			this.labMusic.Text = "Music";
			this.labMusic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ddlMusic
			// 
			this.ddlMusic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.ddlMusic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlMusic.DropDownWidth = 300;
			this.ddlMusic.FormattingEnabled = true;
			this.ddlMusic.Location = new System.Drawing.Point(88, 30);
			this.ddlMusic.Name = "ddlMusic";
			this.ddlMusic.Size = new System.Drawing.Size(238, 21);
			this.ddlMusic.TabIndex = 15;
			this.ddlMusic.SelectedIndexChanged += new System.EventHandler(this.DdlMusicSelectedIndexChanged);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(339, 319);
			this.tableLayoutPanel1.TabIndex = 42;
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.ddlWarp);
			this.panel1.Controls.Add(this.labWarp);
			this.panel1.Controls.Add(this.labScroll);
			this.panel1.Controls.Add(this.checkBoxRight);
			this.panel1.Controls.Add(this.checkBoxLeft);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(333, 54);
			this.panel1.TabIndex = 43;
			// 
			// panel2
			// 
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel2.Controls.Add(this.labMusic);
			this.panel2.Controls.Add(this.ddlMusic);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 63);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(333, 54);
			this.panel2.TabIndex = 44;
			// 
			// SectorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SectorForm";
			this.Size = new System.Drawing.Size(339, 319);
			this.VisibleChanged += new System.EventHandler(this.SectorFormVisibleChanged);
			this.panel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txbCameraX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbCameraY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txbWarioX)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}
