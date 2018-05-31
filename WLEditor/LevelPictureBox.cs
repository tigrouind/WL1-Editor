using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class LevelPictureBox : PictureBox
	{			
		readonly bool[] invalidTiles = new bool[256 * 32];	
		DirectBitmap levelTiles = new DirectBitmap(4096, 512);		
		int zoom = 1;
		int lastTileIndex = -1;
				
		public bool ShowSectors = true;
		public bool ShowScrollInfo = true;
		public bool ShowObjects = true;		
		public bool ShowColliders;
		public int SwitchMode;
		public int CurrentSector = -1;		
		
		public event EventHandler<int> TileMouseDown;		
		public event EventHandler<int> SectorChanged;
				
		protected override void OnPaint(PaintEventArgs e)
		{			
			if(Level.levelData != null)
			{
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;	
			
				var sectorsToDraw = GetVisibleSectors(e.ClipRectangle);

				using (Brush enemyBrush = new SolidBrush(Level.enemyPalette[2]))
				using (Brush redBrush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Font font = new Font("Arial", 8 * zoom))				
				using (Graphics g = Graphics.FromImage(levelTiles.Bitmap))
				{
					//draw tiles to cache
					DrawTiles(redBrush, g, e.ClipRectangle, ShowColliders);

					//draw tiles from cache
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;					
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.DrawImage(levelTiles.Bitmap, 0, 0, 4096 * zoom, 512 * zoom);					
					
					//sector objects (enemies, powerups)
					if(ShowObjects)
					{
						DrawObjects(font, format, e, enemyBrush);			
						
						//wario position
						int index = Level.warioRightFacing ? 0 : 1;
						Rectangle playerRectangle = Level.playerRectangles[index];
						Rectangle destRect = new Rectangle((Level.warioPosition % 8192 + playerRectangle.X - 32) * zoom, (Level.warioPosition / 8192 + playerRectangle.Y - 56 - index * 64) * zoom, playerRectangle.Width * zoom, playerRectangle.Height * zoom);
						if (destRect.IntersectsWith(e.ClipRectangle))
						{
							e.Graphics.DrawImage(Level.playerSprite.Bitmap, destRect, playerRectangle, GraphicsUnit.Pixel);
						}
					}
					
					//scroll
					if(ShowScrollInfo)
					{
						foreach (Point point in sectorsToDraw)
						{
							DrawScrollInfo(point.X, point.Y, e.Graphics);
						}
					}

					//sectors
					if(ShowSectors)
					{			
						foreach (Point point in sectorsToDraw)
						{						
							DrawSectorInfo(point.X, point.Y, e.Graphics, font, format);
						}
						
						DrawSectorBorders(e.Graphics, e.ClipRectangle);
						
						if (CurrentSector != -1)
						{
							Rectangle sectorRect = new Rectangle((CurrentSector % 16) * 256 * zoom, (CurrentSector / 16) * 256 * zoom, 256 * zoom, 256 * zoom);
							if (sectorRect.IntersectsWith(e.ClipRectangle))
							{
								using (Brush blue = new SolidBrush(Color.FromArgb(64, 0, 0, 255)))
								{
									e.Graphics.FillRectangle(blue, sectorRect);
								}
							}
						}														
					}
				}			
			}
		}
		
		void DrawTiles(Brush brush, Graphics g, Rectangle clipRectangle, bool viewColliders)
		{			
			clipRectangle = GetClipRectangle(clipRectangle, 16 * zoom);			
			
			for(int j = clipRectangle.Top ; j < clipRectangle.Bottom ; j++)
			{
				for(int i = clipRectangle.Left ; i < clipRectangle.Right ; i++)
				{
					if(!invalidTiles[i + j * 256])
					{
						invalidTiles[i + j * 256] = true;
						DrawTileToBitmap(i, j);

						if(viewColliders)
						{
							byte tileIndex = Level.levelData[i + j * 256 + 0x1000];
							if(Level.IsCollidable(Level.SwitchTile(tileIndex, SwitchMode)))
							{
								g.FillRectangle(brush, new Rectangle(i * 16, j * 16, 16, 16));
							}
						}
					}
				}
			}
		}

		void DrawTileToBitmap(int x, int y)
		{
			byte tileIndex = Level.levelData[x + y * 256 + 0x1000];
			Point dest = new Point(x * 16, y * 16);
			Point src = new Point((tileIndex % 8) * 16, (tileIndex / 8) * 16);

			for(int i = 0 ; i < 16 ; i++)
			{
				Array.Copy(Level.tiles16x16.Bits, src.X + (src.Y + i) * Level.tiles16x16.Width, levelTiles.Bits, dest.X + (dest.Y + i) * levelTiles.Width, 16);
			}
		}

		void DrawObjects(Font font, StringFormat format, PaintEventArgs e, Brush brush)
		{			
			for(int j = 0 ; j < 32 ; j++)
			{
				for(int i = 0 ; i < 256 ; i++)
				{
					byte data = Level.objectsData[i + j * 256];
					if(data > 0)
					{
						Rectangle destRect = new Rectangle(i * 16 * zoom, j * 16 * zoom, 16 * zoom, 16 * zoom);
						if(destRect.IntersectsWith(e.ClipRectangle))
						{
							//objects						
							e.Graphics.FillRectangle(brush, destRect);
							
							if(data > 6)
							{
								e.Graphics.DrawImage(Level.tilesObjects.Bitmap, destRect, new Rectangle((data - 7) * 16, 0, 16, 16), GraphicsUnit.Pixel);								
							}
							else if(Level.loadedSprites[data - 1] == Rectangle.Empty)
							{
								e.Graphics.DrawString(data.ToString(), font, Brushes.White, (i * 16 + 8) * zoom, (j * 16 + 8) * zoom, format);								
							}
						}
						
						//objects sprites																	
						if(data <= 6 && Level.loadedSprites[data - 1] != Rectangle.Empty)
						{
							Rectangle enemyRect = Level.loadedSprites[data - 1];
							destRect = new Rectangle((i * 16 + enemyRect.X - 32 + 8) * zoom, (j * 16 + enemyRect.Y - (data - 1) * 64 - 40) * zoom, enemyRect.Width * zoom, enemyRect.Height * zoom);							
							if(destRect.IntersectsWith(e.ClipRectangle))
							{																												
								e.Graphics.DrawImage(Level.tilesEnemies.Bitmap, destRect, enemyRect, GraphicsUnit.Pixel);
							}
						}			
					}														
				}
			}
		}
		
		void DrawScrollInfo(int x, int y, Graphics g)
		{
			int drawSector = x + y * 16;
			
			byte scroll = Level.scrollData[drawSector];
			if ((scroll & 2) == 2)
				g.FillRectangle(Brushes.Yellow, x * 256 * zoom, y * 256 * zoom, 6 * zoom, 256 * zoom);

			if ((scroll & 1) == 1)
				g.FillRectangle(Brushes.Yellow, ((x+1) * 256 - 6) * zoom, y * 256 * zoom, 6 * zoom, 256 * zoom);
		}
		
		void DrawSectorInfo(int x, int y, Graphics g, Font font, StringFormat format)
		{
			int drawSector = x + y * 16;
			
			g.FillRectangle(Brushes.Blue, x * 256 * zoom, y * 256 * zoom, 16 * zoom, 16 * zoom);
			g.DrawString(drawSector.ToString("D2"), font, Brushes.White, (x * 256 + 8) * zoom, (y * 256 + 8) * zoom, format);
											
			int sectorTarget = Level.warps[drawSector];
			if(sectorTarget != 255)
			{
				string text = GetWarpName(sectorTarget);
				var result = TextRenderer.MeasureText(text, font);
				
				g.FillRectangle(Brushes.Blue,  (x * 256 + 20) * zoom, y * 256 * zoom, result.Width, 16 * zoom);
				g.DrawString(text, font, Brushes.White, (x * 256) * zoom + result.Width / 2 + 20 * zoom, (y * 256 + 8) * zoom, format);
			}
			
		}
		
		void DrawSectorBorders(Graphics g, Rectangle clipRectangle)
		{	
			//draw sector borders
			using (Pen penBlue = new Pen(Color.Blue, 2.0f * zoom))
			{						
				penBlue.DashPattern = new [] { 5.0f, 1.0f };
				
				for(int i = 1 ; i < 16 ; i++)
				{
					int x = i * 256 * zoom;
					Rectangle lineRect = new Rectangle(x - zoom, 0, 2 * zoom, 512 * zoom);					
					if(lineRect.IntersectsWith(clipRectangle))
					{
						g.DrawLine(penBlue, x, 0, x, 512 * zoom);
					}
				}
										
				g.DrawLine(penBlue, 0, 256 * zoom, 4096 * zoom, 256 * zoom);					
			}			
		}
		
		List<Point> GetVisibleSectors(Rectangle clipRectangle)
		{					
			clipRectangle = GetClipRectangle(clipRectangle, 256 * zoom);
			List<Point> sectors = new List<Point>(clipRectangle.Width * clipRectangle.Height);
			for(int y = clipRectangle.Top ; y < clipRectangle.Bottom ; y++)
			{
				for(int x = clipRectangle.Left ; x < clipRectangle.Right ; x++)
				{
					sectors.Add(new Point(x, y));
				}
			}
			
			return sectors;
		}
		
		Rectangle GetClipRectangle(Rectangle clipRectangle, int size)
		{
			Point start = new Point(clipRectangle.Left / size, clipRectangle.Top / size);
			Point end = new Point((clipRectangle.Right - 1) / size + 1, (clipRectangle.Bottom - 1) / size + 1);			
			return new Rectangle(start.X, start.Y, end.X - start.X, end.Y - start.Y);
		}

		string GetWarpName(int sectorTarget)
		{
			string warpText;
			switch(sectorTarget)
			{
				case 32:
					warpText = "EXIT MAP";
					break;
				case 33:
					warpText = "EXIT A";
					break;
				case 34:
					warpText = "EXIT B";
					break;
				default:
					warpText = "W" + sectorTarget.ToString("D2");
					break;
			}

			return warpText;
		}
		
		public void InvalidateTile(int tileIndex)
		{
			Region r = new Region(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom));						
			invalidTiles[tileIndex] = false;                
			Invalidate(r);						
		}
		
		public void InvalidateObject(int tileIndex, int currentObject, int previousObject)
		{
			Region r = new Region(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom));									
			Action<int> addEnemyRegion = enemyIndex =>
			{
				if(enemyIndex >= 1 && enemyIndex <= 6)
				{
					Rectangle enemyRect = Level.loadedSprites[enemyIndex - 1];
					if(enemyRect != Rectangle.Empty)
					{						
						r.Union(new Rectangle(((tileIndex % 256) * 16 + enemyRect.X - 32 + 8) * zoom, ((tileIndex / 256) * 16 + enemyRect.Y - (enemyIndex - 1) * 64 - 40) * zoom, enemyRect.Width * zoom, enemyRect.Height * zoom));
					}
				}
			};
			
			addEnemyRegion(previousObject);
			addEnemyRegion(currentObject);
			
			Invalidate(r);
		}
		
		void OnMouseEvent(MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				int tileIndex = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 256;
				if(tileIndex != lastTileIndex)
				{
					TileMouseDown(this, tileIndex);
					lastTileIndex = tileIndex;
				}
			}
			else if(e.Button == MouseButtons.Right)
			{
				Point coordinates = e.Location;
				int sector = e.Location.X / 256 / zoom + (e.Location.Y / 256 / zoom) * 16;
				if(sector != CurrentSector)
				{
					CurrentSector = sector;
					SectorChanged(this, sector);
				}
			}
		}
				
		protected override void OnMouseDown(MouseEventArgs e)
		{
			lastTileIndex = -1;
			OnMouseEvent(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (ClientRectangle.Contains(e.Location))
			{
				OnMouseEvent(e);
			}
		}
	
		public void SetZoom(int zoomLevel)
		{
			Height = 512 * zoomLevel;
			Width = 4096 * zoomLevel;	
			zoom = zoomLevel;
			Invalidate();
		}
		
		public void ClearTileCache()
		{
			Array.Clear(invalidTiles, 0, invalidTiles.Length);
		}
	}
}
