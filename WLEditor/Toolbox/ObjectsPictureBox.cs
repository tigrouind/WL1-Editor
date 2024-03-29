
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class ObjectsPictureBox : PictureBox
	{
		public int CurrentObject;
		int zoom;

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Level.LevelData != null && !DesignMode)
			{
				StringFormat format = new StringFormat
				{
					LineAlignment = StringAlignment.Center,
					Alignment = StringAlignment.Center
				};

				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Pen pen = new Pen(Color.White, 1.5f * zoom))
				using (Font font = new Font("Arial", 8 * zoom))
				{
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.FillRectangle(LevelPictureBox.EnemyBrush, 0, 0, Width, Height);

					for (int index = 0; index < 16; index++)
					{
						DrawTile(e.ClipRectangle, pen, font, format, index);
					}

					e.Graphics.FillRectangle(brush, (CurrentObject % 4) * 32 * zoom, (CurrentObject / 4) * 32 * zoom, 32 * zoom, 32 * zoom);
				}
			}

			void DrawTile(Rectangle clipRectangle, Pen pen, Font font, StringFormat format, int index)
			{
				int x = (index % 4) * 32;
				int y = (index / 4) * 32;

				Rectangle destRect = new Rectangle(x * zoom, y * zoom, 32 * zoom, 32 * zoom);
				if (destRect.IntersectsWith(clipRectangle))
				{
					if (index == 0)
					{
						e.Graphics.DrawLine(pen, (x + 8) * zoom, (y + 8) * zoom, (x + 24) * zoom, (y + 24) * zoom);
						e.Graphics.DrawLine(pen, (x + 24) * zoom, (y + 8) * zoom, (x + 8) * zoom, (y + 24) * zoom);
					}
					if (index >= 1 && index <= 6) //enemy
					{
						Rectangle enemyRect = Sprite.LoadedSprites[index - 1].Rectangle;
						if (enemyRect != Rectangle.Empty)
						{
							var imgRect = new Rectangle((index - 1) * 40 + 4, 4, 32, 32);
							e.Graphics.DrawImage(Sprite.TilesEnemies.Bitmap, destRect, imgRect, GraphicsUnit.Pixel);
						}
						else
						{
							e.Graphics.DrawString(index.ToString(), font, Brushes.White, (x + 16) * zoom, (y + 16) * zoom, format);
						}
					}
					else //power up
					{
						e.Graphics.DrawImage(Sprite.TilesObjects.Bitmap, new Rectangle((x + 8) * zoom, (y + 8) * zoom, 16 * zoom, 16 * zoom), new Rectangle((index - 7) * 16, 0, 16, 16), GraphicsUnit.Pixel);
					}
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
	}
}
