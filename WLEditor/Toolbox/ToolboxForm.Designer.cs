namespace WLEditor
{
	partial class ToolboxForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Panel panel1;
		private WLEditor.ObjectsPictureBox objectsPictureBox;
		private WLEditor.Tiles16x16PictureBox tiles16x16PictureBox;
		private WLEditor.Tiles8x8PictureBox tiles8x8PictureBox;

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
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.objectsPictureBox = new WLEditor.ObjectsPictureBox();
			this.tiles16x16PictureBox = new WLEditor.Tiles16x16PictureBox();
			this.tiles8x8PictureBox = new WLEditor.Tiles8x8PictureBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.objectsPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tiles16x16PictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tiles8x8PictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.comboBox1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(96, 195);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// comboBox1
			// 
			this.comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
			"16x16 Tiles ",
			"8x8 Tiles",
			"Objects"});
			this.comboBox1.Location = new System.Drawing.Point(3, 3);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(90, 21);
			this.comboBox1.TabIndex = 1;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.objectsPictureBox);
			this.panel1.Controls.Add(this.tiles16x16PictureBox);
			this.panel1.Controls.Add(this.tiles8x8PictureBox);
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(53, 168);
			this.panel1.TabIndex = 2;
			// 
			// objectsPictureBox
			// 
			this.objectsPictureBox.Location = new System.Drawing.Point(3, 115);
			this.objectsPictureBox.Name = "objectsPictureBox";
			this.objectsPictureBox.Size = new System.Drawing.Size(47, 50);
			this.objectsPictureBox.TabIndex = 2;
			this.objectsPictureBox.TabStop = false;
			// 
			// tiles16x16PictureBox
			// 
			this.tiles16x16PictureBox.Location = new System.Drawing.Point(3, 59);
			this.tiles16x16PictureBox.Name = "tiles16x16PictureBox";
			this.tiles16x16PictureBox.Size = new System.Drawing.Size(47, 50);
			this.tiles16x16PictureBox.TabIndex = 1;
			this.tiles16x16PictureBox.TabStop = false;
			// 
			// tiles8x8PictureBox
			// 
			this.tiles8x8PictureBox.Location = new System.Drawing.Point(3, 3);
			this.tiles8x8PictureBox.Name = "tiles8x8PictureBox";
			this.tiles8x8PictureBox.Size = new System.Drawing.Size(47, 50);
			this.tiles8x8PictureBox.TabIndex = 0;
			this.tiles8x8PictureBox.TabStop = false;
			// 
			// ToolboxForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(156, 215);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ToolboxForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Toolbox";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ToolBoxFormClosing);
			this.Load += new System.EventHandler(this.ToolBoxFormLoad);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.objectsPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tiles16x16PictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tiles8x8PictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
