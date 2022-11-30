
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class ObjectsPictureBox : PictureBox
	{
		public int CurrentObject = -1;	
		int zoom;
		
		protected override void OnPaint(PaintEventArgs e)
		{			
			if(Level.LevelData != null && !DesignMode)
			{
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Font font = new Font("Arial", 8 * zoom))
				{
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;					
					e.Graphics.FillRectangle(LevelPictureBox.EnemyBrush, 0, 0, Width, Height);
					e.Graphics.FillRectangle(Brushes.Black, new Rectangle(32 * 3 * zoom, 32 * 3 * zoom, 32 * zoom, 32 * zoom));
					
					for(int index = 0 ; index < 15 ; index++)
					{
						DrawTile(e.Graphics, e.ClipRectangle, font, format, index);
					}
					
					if(CurrentObject != -1)
					{
						int index = CurrentObject - 1;
						e.Graphics.FillRectangle(brush, (index % 4) * 32 * zoom, (index / 4) * 32 * zoom, 32 * zoom, 32 * zoom);
					}
				}
			}		
		}
		
		void DrawTile(Graphics g, Rectangle clipRectangle, Font font, StringFormat format, int index)
		{			
			int x = (index % 4) * 32;
			int y = (index / 4) * 32;
			
			Rectangle destRect = new Rectangle(x * zoom, y * zoom, 32 * zoom, 32 * zoom);							
			if (destRect.IntersectsWith(clipRectangle))
			{
				if(index < 6) //enemy
				{																	
					if(Level.LoadedSprites[index] != Rectangle.Empty)
					{
						Rectangle enemyRect = Level.LoadedSprites[index];
						g.DrawImage(Level.TilesEnemies.Bitmap, destRect, new Rectangle(enemyRect.X - 16 + enemyRect.Width / 2, enemyRect.Y - 16 + enemyRect.Height / 2, 32, 32), GraphicsUnit.Pixel);
					}
					else 
					{
						g.DrawString((index + 1).ToString(), font, Brushes.White, (x + 16) * zoom, (y + 16) * zoom, format);
					}
				}
				else //power up
				{
					g.DrawImage(Level.TilesObjects.Bitmap, new Rectangle((x + 8) * zoom, (y + 8) * zoom, 16 * zoom, 16 * zoom), new Rectangle((index - 6) * 16, 0, 16, 16),  GraphicsUnit.Pixel);
				}
			}
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			int index = e.Location.X / 32 / zoom + (e.Location.Y / 32 / zoom) * 4;
			if (index < 15)
			{
				CurrentObject = index + 1;
				Invalidate();
			}
		}
		
		public void SetZoom(int zoomlevel)
		{
			Height = 128 * zoomlevel;
			Width = 128 * zoomlevel;
			zoom = zoomlevel;
			Invalidate();			
		}						
	}
}
