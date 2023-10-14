using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class Tiles16x16PictureBox : PictureBox
	{
		public int CurrentTile;
		public bool ShowColliders = true;
		public int SwitchType;
		public int SwitchMode;
		public event EventHandler<TileEventArgs> TileMouseMove;
		public event EventHandler TileMouseLeave;

		int lastTile;
		readonly DirectBitmap tiles = new DirectBitmap(128, 256);
		int zoom;

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Level.LevelData != null && !DesignMode)
			{
				using (Graphics g = Graphics.FromImage(tiles.Bitmap))
				{
					g.DrawImage(Level.Tiles16x16.Bitmap, 0, 0, 128, 256);

					if (ShowColliders)
					{
						DrawTiles(g, e.ClipRectangle);
					}

					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.DrawImage(tiles.Bitmap, 0, 0, 128 * zoom, 256 * zoom);
				}

				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				{
					e.Graphics.FillRectangle(brush, (CurrentTile % 8) * 16 * zoom, (CurrentTile / 8) * 16 * zoom, 16 * zoom, 16 * zoom);
				}

				void DrawTiles(Graphics g, Rectangle clipRectangle)
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(64, 0, 0, 0)))
					{
						for (int j = 0; j < 16; j++)
						{
							for (int i = 0; i < 8; i++)
							{
								Rectangle destRect = new Rectangle(i * 16, j * 16, 16, 16);

								if (destRect.IntersectsWith(clipRectangle))
								{
									int tileIndex = i + j * 8;
									tileIndex = Level.SwitchTile(tileIndex, SwitchMode);

									if (ShowColliders)
									{
										int specialTile = Level.GetTileInfo(tileIndex, SwitchType).Type;
										if (specialTile != -1)
										{
											g.FillRectangle(LevelPictureBox.TransparentBrushes[specialTile], destRect);
										}
									}
								}
							}
						}
					}
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
