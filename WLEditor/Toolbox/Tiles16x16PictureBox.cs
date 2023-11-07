using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class Tiles16x16PictureBox : PictureBox
	{
		public event EventHandler<TileEventArgs> TileMouseMove;
		public event EventHandler TileMouseLeave;

		int lastTile;
		int zoom;
		readonly Selection selection = new Selection(16);

		public Tiles16x16PictureBox()
		{
			selection.InvalidateSelection += (s, e) => Invalidate(e.ClipRectangle);
		}

		public int CurrentTile => selection.GetCurrentTile((x, y) => x + y * 8);

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Level.LevelData != null && !DesignMode)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(Level.Tiles16x16.Bitmap, 0, 0, 128 * zoom, 256 * zoom);

				selection.DrawSelection(e.Graphics);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			int tilePosX = e.Location.X / 16 / zoom;
			int tilePosY = e.Location.Y / 16 / zoom;
			OnMouseEvent(new TileEventArgs(e.Button, TileEventStatus.MouseDown, tilePosX, tilePosY));
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
			else if(e.Button == MouseButtons.Right)
			{
				if (e.Status == TileEventStatus.MouseDown)
				{
					selection.StartSelection(e.TileX, e.TileY);
				}
			}
		}

		public void SetZoom(int zoomlevel)
		{
			Height = 256 * zoomlevel;
			Width = 128 * zoomlevel;
			zoom = zoomlevel;
			selection.SetZoom(zoomlevel);
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
					OnMouseEvent(new TileEventArgs(e.Button, TileEventStatus.MouseMove, tilePosX, tilePosY));
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

		public bool ProcessCommandKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Control | Keys.C:
					selection.CopySelection((x, y) => new ClipboardData() {Tile = x + y * 8 });
					selection.ClearSelection();
					return true;
			}

			return false;
		}
	}
}
