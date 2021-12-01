using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace WLEditor
{
	public class Tiles16x16PictureBox : PictureBox
	{
		public int CurrentTile = -1;
		public bool ShowColliders;
		public bool ShowTileNumbers;
		public int SwitchMode;		
		DirectBitmap tiles = new DirectBitmap(128, 256);
		int zoom;
				
		protected override void OnPaint(PaintEventArgs e)
		{						
			if(Level.LevelData != null)
			{				
				using (Graphics g = Graphics.FromImage(tiles.Bitmap))
				{			
					g.DrawImage(Level.Tiles16x16.Bitmap, 0, 0, 128, 256);
									
					if (ShowColliders || ShowTileNumbers)
					{			
						DrawTiles(g, e.ClipRectangle);
					}	
					
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.DrawImage(tiles.Bitmap, 0, 0, 128 * zoom, 256 * zoom);	
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
		
		void DrawTiles(Graphics g, Rectangle clipRectangle)
		{
			using (Font font = new Font("Verdana", 7))
			using (StringFormat format = new StringFormat())
			using (Brush brush = new SolidBrush(Color.FromArgb(64, 0, 0, 0)))
			{
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;

				for(int j = 0 ; j < 16 ; j++)
				{
					for(int i = 0 ; i < 8 ; i++)
					{						
						Rectangle destRect = new Rectangle(i * 16, j * 16, 16, 16);

						if(destRect.IntersectsWith(clipRectangle))
						{
							int tileIndex = i + j * 8;
							tileIndex = Level.SwitchTile(tileIndex, SwitchMode);
							
							if (ShowColliders)
							{
								int specialTile = Level.IsSpecialTile(tileIndex);
								if(specialTile != -1)
								{
									g.FillRectangle(LevelPictureBox.TransparentBrushes[specialTile], destRect);
								}		
							}
							
							if (ShowTileNumbers)
							{
								g.FillRectangle(brush, destRect);
								g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
								g.DrawString(tileIndex.ToString("X2"), font, Brushes.White, i * 16 + 8, j * 16 + 8, format);	
								g.TextRenderingHint = TextRenderingHint.SystemDefault;
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
			Invalidate();	
		}
	}
}
