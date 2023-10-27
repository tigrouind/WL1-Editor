using System;
using System.Drawing;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class ToolboxForm : Form
	{
		public event EventHandler<KeyEventArgs> ProcessCommandKey;
		readonly Control[] controls;

		public ToolboxForm()
		{
			InitializeComponent();

			controls = new Control[] { tiles16x16PictureBox, objectsPictureBox, tiles8x8PictureBox };
			tiles16x16PictureBox.TileMouseMove += ToolBoxTile16x16MouseMove;
			tiles16x16PictureBox.TileMouseLeave += ToolBoxTile16x16MouseLeave;
		}

		public void SetZoom(int zoom)
		{
			tiles16x16PictureBox.SetZoom(zoom);
			tiles8x8PictureBox.SetZoom(zoom);
			objectsPictureBox.SetZoom(zoom);
		}

		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			SuspendLayout();

			Control controlToShow = controls[comboBox1.SelectedIndex];
			controlToShow.Visible = true;
			controlToShow.Location = new Point(controlToShow.Margin.Left, controlToShow.Margin.Top);

			foreach (Control control in panel1.Controls)
			{
				if (control != controlToShow)
				{
					control.Visible = false;
				}
			}

			ResumeLayout();
		}

		void ToolBoxFormLoad(object sender, EventArgs e)
		{
			comboBox1.SelectedIndex = 0;
		}

		void ToolBoxFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			KeyEventArgs args = new KeyEventArgs(keyData);

			ProcessCommandKey(this, args);
			if (args.Handled)
			{
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		public Control SelectedPanel => controls[comboBox1.SelectedIndex];

		public Tiles16x16PictureBox Tiles16x16 => tiles16x16PictureBox;

		public Tiles8x8PictureBox Tiles8x8 => tiles8x8PictureBox;

		public ObjectsPictureBox Objects => objectsPictureBox;

		void ToolBoxTile16x16MouseMove(object sender, TileEventArgs e)
		{
			byte tileIndex = (byte)(e.TileX + e.TileY * 8);
			var tileInfo = Level.GetTileInfo(tileIndex);
			toolStripStatusLabel1.Text = string.Format($"{tileIndex:X2} {tileInfo.Text}");
			statusStrip1.Visible = true;
		}

		void ToolBoxTile16x16MouseLeave(object sender, EventArgs e)
		{
			statusStrip1.Visible = false;
		}
	}
}
