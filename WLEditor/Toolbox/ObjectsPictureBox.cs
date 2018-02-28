
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
			if(Level.levelData != null)
			{
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

				using (Brush enemyBrush = new SolidBrush(Level.enemyPalette[2]))
				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Pen pen = new Pen(Color.White, 1.5f * zoom))
				using (Font font = new Font("Arial", 16 * zoom))
				{
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					
					e.Graphics.FillRectangle(enemyBrush, 0, 0, Width, Height);
					
					for(int j = 0 ; j < 16 ; j++)
					{
						int index = GUIToObjectIndex(j);
						int x = (j % 4) * 32;
						int y = (j / 4) * 32;
						
						Rectangle destRect = new Rectangle(x * zoom, y * zoom, 32 * zoom, 32 * zoom);							
						if (destRect.IntersectsWith(e.ClipRectangle))
					    {
						   	if(index == 0)
							{							
								e.Graphics.DrawLine(pen, (x + 8) * zoom, (y + 8) * zoom, (x + 24) * zoom, (y + 24) * zoom);
								e.Graphics.DrawLine(pen, (x + 24) * zoom, (y + 8) * zoom, (x + 8) * zoom, (y + 24) * zoom);
							}
							else if(index <= 6)
							{																	
								if(Level.enemiesAvailable[index - 1])
								{
									Rectangle enemyRect = Level.loadedSprites[index - 1];
									if(enemyRect != Rectangle.Empty)
									{									
										e.Graphics.DrawImage(Level.tilesEnemies.Bitmap, destRect, new Rectangle(enemyRect.X - 16 + enemyRect.Width / 2, enemyRect.Y - 16 + enemyRect.Height / 2, 32, 32), GraphicsUnit.Pixel);
									}	
									else
									{
										e.Graphics.DrawString(index.ToString(), font,  Brushes.White, (x + 16) * zoom, (y + 16) * zoom, format);
									}
								}
							}
							else
							{
								e.Graphics.DrawImage(Level.tilesObjects.Bitmap, new Rectangle((x + 8) * zoom, (y + 8) * zoom, 16 * zoom, 16 * zoom), new Rectangle((index - 7) * 16, 0, 16, 16),  GraphicsUnit.Pixel);
							}
					    }						
					}
					
					if(CurrentObject != -1)
					{
						int index = ObjectIndexToGUI(CurrentObject);
						e.Graphics.FillRectangle(brush, (index % 4) * 32 * zoom, (index / 4) * 32 * zoom, 32 * zoom, 32 * zoom);
					}
				}
			}		
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			int index = e.Location.X / 32 / zoom + (e.Location.Y / 32 / zoom) * 4;
			index = GUIToObjectIndex(index);
			
			if(!(index >= 1 && index <= 6) || Level.enemiesAvailable[index - 1])
			{
				CurrentObject = index;
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
		
		int ObjectIndexToGUI(int index)
		{
			if(index >= 1 && index <= 6) return index + 9;
			if(index > 6) return index - 6;
			return 0;
		}
		
		int GUIToObjectIndex(int index)
		{
			if(index >= 1 && index <= 9) return index + 6;
			if(index > 9) return index - 9;
			return 0;
		}							
	}
}
