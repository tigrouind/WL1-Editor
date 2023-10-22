using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class Tiles16x16PictureBox : PictureBox
	{
		public int CurrentTile;
		public event EventHandler<TileEventArgs> TileMouseMove;
		public event EventHandler TileMouseLeave;

		int lastTile;
		int zoom;

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Level.LevelData != null && !DesignMode)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(Level.Tiles16x16.Bitmap, 0, 0, 128 * zoom, 256 * zoom);

				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				{
					e.Graphics.FillRectangle(brush, (CurrentTile % 8) * 16 * zoom, (CurrentTile / 8) * 16 * zoom, 16 * zoom, 16 * zoom);
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			CurrentTile = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 8;
			Invalidate();
		}

		public void SetZoom(int zoomlevel)
		{
			Height = 256 * zoomlevel;
			Width = 128 * zoomlevel;
			zoom = zoomlevel;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (ClientRectangle.Contains(e.Location))
			{
				int tilePosX = e.Location.X / 16 / zoom;
				int tilePosY = e.Location.Y / 16 / zoom;
				int tilePos = tilePosX + tilePosY * 8;

				if (tilePos != lastTile)
				{
					lastTile = tilePos;
					RaiseTileMouseMoveEvent();
				}
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (lastTile != -1)
			{
				lastTile = -1;
				TileMouseLeave?.Invoke(this, EventArgs.Empty);
			}
		}

		void RaiseTileMouseMoveEvent()
		{
			TileMouseMove?.Invoke(this, new TileEventArgs(MouseButtons.None, TileEventStatus.None, lastTile % 8, lastTile / 8));
		}
	}
}
