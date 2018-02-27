/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 26/02/2018
 * Time: 21:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace WLEditor
{
	partial class ToolboxForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.PictureBox tiles16x16PictureBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.PictureBox objectPictureBox;
		private System.Windows.Forms.PictureBox tiles8x8PictureBox;
		
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
			this.tiles16x16PictureBox = new System.Windows.Forms.PictureBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tiles8x8PictureBox = new System.Windows.Forms.PictureBox();
			this.objectPictureBox = new System.Windows.Forms.PictureBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.tiles16x16PictureBox)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tiles8x8PictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.objectPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// tiles16x16PictureBox
			// 
			this.tiles16x16PictureBox.Location = new System.Drawing.Point(0, 27);
			this.tiles16x16PictureBox.Margin = new System.Windows.Forms.Padding(0);
			this.tiles16x16PictureBox.Name = "tiles16x16PictureBox";
			this.tiles16x16PictureBox.Size = new System.Drawing.Size(128, 256);
			this.tiles16x16PictureBox.TabIndex = 0;
			this.tiles16x16PictureBox.TabStop = false;
			this.tiles16x16PictureBox.Visible = false;
			this.tiles16x16PictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.TilesPictureBoxPaint);
			this.tiles16x16PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TilesPictureBoxMouseDown);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.tiles8x8PictureBox, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.objectPictureBox, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.tiles16x16PictureBox, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.comboBox1, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(128, 475);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// tiles8x8PictureBox
			// 
			this.tiles8x8PictureBox.Location = new System.Drawing.Point(0, 411);
			this.tiles8x8PictureBox.Margin = new System.Windows.Forms.Padding(0);
			this.tiles8x8PictureBox.Name = "tiles8x8PictureBox";
			this.tiles8x8PictureBox.Size = new System.Drawing.Size(128, 64);
			this.tiles8x8PictureBox.TabIndex = 3;
			this.tiles8x8PictureBox.TabStop = false;
			this.tiles8x8PictureBox.Visible = false;
			this.tiles8x8PictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.Tiles8x8PictureBoxPaint);
			// 
			// objectPictureBox
			// 
			this.objectPictureBox.Location = new System.Drawing.Point(0, 283);
			this.objectPictureBox.Margin = new System.Windows.Forms.Padding(0);
			this.objectPictureBox.Name = "objectPictureBox";
			this.objectPictureBox.Size = new System.Drawing.Size(128, 128);
			this.objectPictureBox.TabIndex = 2;
			this.objectPictureBox.TabStop = false;
			this.objectPictureBox.Visible = false;
			this.objectPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ObjectFormPaint);
			this.objectPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ObjectPictureBoxMouseDown);
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
			this.comboBox1.Size = new System.Drawing.Size(122, 21);
			this.comboBox1.TabIndex = 1;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
			// 
			// ToolboxForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(156, 648);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ToolboxForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Tiles16x16FormFormClosing);
			this.Load += new System.EventHandler(this.Tiles16x16FormLoad);
			((System.ComponentModel.ISupportInitialize)(this.tiles16x16PictureBox)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tiles8x8PictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.objectPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
