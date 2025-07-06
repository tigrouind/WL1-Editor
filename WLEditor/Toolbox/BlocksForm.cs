using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor.Toolbox
{
	public partial class BlocksForm : Form
	{
		public event EventHandler<KeyEventArgs> ProcessCommandKey;
		public PictureBox PictureBox => pictureBox;

		int lastTile = -1;
		int zoom;
		readonly Selection selection = new(16);

		public BlocksForm()
		{
			selection.InvalidateSelection += (s, e) => pictureBox.Invalidate(e.ClipRectangle);
			InitializeComponent();
		}

		public int CurrentTile => selection.GetCurrentTile((x, y) => x + y * 8);

		void PictureBoxPaint(object sender, PaintEventArgs e)
		{
			if (Level.LevelData != null && !DesignMode)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(Level.Tiles16x16.Bitmap, 0, 0, 128 * zoom, 256 * zoom);

				selection.DrawSelection(e.Graphics);
			}
		}

		void PictureBoxMouseDown(object sender, MouseEventArgs e)
		{
			int tilePosX = e.Location.X / 16 / zoom;
			int tilePosY = e.Location.Y / 16 / zoom;
			OnMouseEvent(new TileEventArgs(e.Button, TileEventStatus.MouseDown, tilePosX, tilePosY));
		}

		void PictureBoxMouseMove(object sender, MouseEventArgs e)
		{
			if (ClientRectangle.Contains(e.Location))
			{
				int tilePosX = e.Location.X / 16 / zoom;
				int tilePosY = e.Location.Y / 16 / zoom;
				int tilePos = tilePosX + tilePosY * 8;

				if (lastTile == -1)
				{
					statusStrip1.Visible = true;
					pictureBox.Margin = new Padding(0, 0, 0, statusStrip1.Height);
				}

				if (tilePos != lastTile)
				{
					lastTile = tilePos;
					OnMouseEvent(new TileEventArgs(e.Button, TileEventStatus.MouseMove, tilePosX, tilePosY));

					var tileInfo = Level.GetTileInfo((byte)tilePos);
					toolStripStatusLabel1.Text = $"{tilePos:X2} {tileInfo.Text}";
				}
			}
		}

		void OnMouseEvent(TileEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (e.Status == TileEventStatus.MouseDown)
				{
					selection.StartSelection(e.TileX, e.TileY);
				}
				else if (e.Status == TileEventStatus.MouseMove)
				{
					selection.SetSelection(e.TileX, e.TileY);
				}
			}
			else if (e.Button == MouseButtons.Right)
			{
				if (e.Status == TileEventStatus.MouseDown)
				{
					selection.StartSelection(e.TileX, e.TileY);
				}
			}
		}

		public void SetZoom(int zoomlevel)
		{
			pictureBox.Height = 256 * zoomlevel;
			pictureBox.Width = 128 * zoomlevel;
			zoom = zoomlevel;
			selection.SetZoom(zoomlevel);
			pictureBox.Invalidate();
		}

		void PictureBoxMouseLeave(object sender, EventArgs e)
		{
			if (lastTile != -1)
			{
				lastTile = -1;
				statusStrip1.Visible = false;
				pictureBox.Margin = Padding.Empty;
			}
		}

		void BlocksFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			KeyEventArgs args = new(keyData);

			switch (keyData)
			{
				case Keys.Control | Keys.C:
					selection.CopySelection((x, y) => new ClipboardData() { Tile = x + y * 8 });
					selection.ClearSelection();
					return true;
			}

			ProcessCommandKey(this, args);
			if (args.Handled)
			{
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
