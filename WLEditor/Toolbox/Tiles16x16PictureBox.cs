using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class Tiles16x16PictureBox : PictureBox
	{
		public int CurrentTile = -1;
		public bool ShowColliders;
		public int SwitchMode;		
		int zoom;
				
		protected override void OnPaint(PaintEventArgs e)
		{			
			if(Level.levelData != null)
			{				
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(Level.tiles16x16.Bitmap, 0, 0, 128 * zoom, 256 * zoom);
										
				if(ShowColliders)
				{						
					for(int j = 0 ; j < 16 ; j++)
					{
						for(int i = 0 ; i < 8 ; i++)
						{						
							Rectangle destRect = new Rectangle(i * 16 * zoom, j * 16 * zoom, 16 * zoom, 16 * zoom);
	
							if(destRect.IntersectsWith(e.ClipRectangle))
							{
								int tileIndex = i + j * 8;
								tileIndex = Level.SwitchTile(tileIndex, SwitchMode);
								
								int specialTile = Level.IsSpecialTile(tileIndex);
								if(specialTile != -1)
								{
									e.Graphics.FillRectangle(Level.transparentBrushes[specialTile], destRect);
								}	
							}
						}	
					}						
				}				
	
				if(CurrentTile != -1)
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
					{
						e.Graphics.FillRectangle(brush, (CurrentTile % 8) * 16 * zoom, (CurrentTile / 8) * 16 * zoom, 16 * zoom, 16 * zoom);
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
			Invalidate();	
		}
	}
}
