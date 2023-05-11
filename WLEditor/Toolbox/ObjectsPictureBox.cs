
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class ObjectsPictureBox : PictureBox
	{
		public int CurrentObject;
		public event EventHandler<TileEventArgs> TileMouseMove;
		int zoom;
		int lastTile;

		protected override void OnPaint(PaintEventArgs e)
		{
			if(Level.LevelData != null && !DesignMode)
			{
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Pen pen = new Pen(Color.White, 1.5f * zoom))
				using (Font font = new Font("Arial", 8 * zoom))
				{
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.FillRectangle(LevelPictureBox.EnemyBrush, 0, 0, Width, Height);

					for(int index = 0 ; index < 16 ; index++)
					{
						DrawTile(e.Graphics, e.ClipRectangle, pen, font, format, index);
					}

					e.Graphics.FillRectangle(brush, (CurrentObject % 4) * 32 * zoom, (CurrentObject / 4) * 32 * zoom, 32 * zoom, 32 * zoom);
				}
			}
		}

		void DrawTile(Graphics g, Rectangle clipRectangle, Pen pen, Font font, StringFormat format, int index)
		{
			int x = (index % 4) * 32;
			int y = (index / 4) * 32;

			Rectangle destRect = new Rectangle(x * zoom, y * zoom, 32 * zoom, 32 * zoom);
			if (destRect.IntersectsWith(clipRectangle))
			{
				if (index == 0)
				{
					g.DrawLine(pen, (x + 8) * zoom, (y + 8) * zoom, (x + 24) * zoom, (y + 24) * zoom);
					g.DrawLine(pen, (x + 24) * zoom, (y + 8) * zoom, (x + 8) * zoom, (y + 24) * zoom);
				}
				if(index >= 1 && index <= 6) //enemy
				{
					if(Sprite.LoadedSprites[index - 1] != Rectangle.Empty)
					{
						Rectangle enemyRect = Sprite.LoadedSprites[index - 1];
						g.DrawImage(Sprite.TilesEnemies.Bitmap, destRect, new Rectangle(enemyRect.X - 16 + enemyRect.Width / 2, enemyRect.Y - 16 + enemyRect.Height / 2, 32, 32), GraphicsUnit.Pixel);
					}
					else
					{
						g.DrawString(index.ToString(), font, Brushes.White, (x + 16) * zoom, (y + 16) * zoom, format);
					}
				}
				else //power up
				{
					g.DrawImage(Sprite.TilesObjects.Bitmap, new Rectangle((x + 8) * zoom, (y + 8) * zoom, 16 * zoom, 16 * zoom), new Rectangle((index - 7) * 16, 0, 16, 16),  GraphicsUnit.Pixel);
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			int index = e.Location.X / 32 / zoom + (e.Location.Y / 32 / zoom) * 4;
			CurrentObject = index;
			Invalidate();
		}

		public void SetZoom(int zoomlevel)
		{
			Height = 128 * zoomlevel;
			Width = 128 * zoomlevel;
			zoom = zoomlevel;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			int tilePosX = e.Location.X / 32 / zoom;
			int tilePosY = e.Location.Y / 32 / zoom;
			int tilePos = tilePosX + tilePosY * 4;

			if (tilePos != lastTile)
			{
				lastTile = tilePos;
				RaiseTileMouseMoveEvent();
			}
		}

		void RaiseTileMouseMoveEvent()
		{
			if (TileMouseMove != null)
			{
				TileMouseMove(this, new TileEventArgs(MouseButtons.None, TileEventStatus.MouseDown, lastTile % 4, lastTile / 4));
			}
		}
	}
}
