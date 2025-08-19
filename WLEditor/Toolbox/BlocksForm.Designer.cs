namespace WLEditor.Toolbox
{
	partial class BlocksForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			pictureBox = new System.Windows.Forms.PictureBox();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
			statusStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// pictureBox
			// 
			pictureBox.Location = new System.Drawing.Point(0, 0);
			pictureBox.Margin = new System.Windows.Forms.Padding(0);
			pictureBox.Name = "pictureBox";
			pictureBox.Size = new System.Drawing.Size(68, 82);
			pictureBox.TabIndex = 0;
			pictureBox.TabStop = false;
			pictureBox.Paint += PictureBoxPaint;
			pictureBox.MouseDown += PictureBoxMouseDown;
			pictureBox.MouseLeave += PictureBoxMouseLeave;
			pictureBox.MouseMove += PictureBoxMouseMove;
			// 
			// statusStrip1
			// 
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
			statusStrip1.Location = new System.Drawing.Point(0, 328);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
			statusStrip1.Size = new System.Drawing.Size(341, 38);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 3;
			statusStrip1.Text = "statusStrip1";
			statusStrip1.Visible = false;
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(151, 33);
			toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// BlocksForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			ClientSize = new System.Drawing.Size(339, 366);
			Controls.Add(statusStrip1);
			Controls.Add(pictureBox);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "BlocksForm";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Blocks";
			FormClosing += BlocksFormClosing;
			((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
	}
}