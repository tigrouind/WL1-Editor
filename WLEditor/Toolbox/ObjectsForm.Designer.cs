namespace WLEditor.Toolbox
{
	partial class ObjectsForm
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
			((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
			SuspendLayout();
			// 
			// pictureBox
			// 
			pictureBox.Location = new System.Drawing.Point(0, 0);
			pictureBox.Margin = new System.Windows.Forms.Padding(0);
			pictureBox.Name = "pictureBox";
			pictureBox.Size = new System.Drawing.Size(91, 102);
			pictureBox.TabIndex = 0;
			pictureBox.TabStop = false;
			pictureBox.Paint += PictureBoxPaint;
			pictureBox.MouseDown += PictureBoxMouseDown;
			// 
			// ObjectsForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			ClientSize = new System.Drawing.Size(191, 173);
			Controls.Add(pictureBox);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ObjectsForm";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Objects";
			FormClosing += ObjectsFormClosing;
			((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox;
	}
}