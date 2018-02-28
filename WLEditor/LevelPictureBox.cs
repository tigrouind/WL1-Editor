using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class LevelPictureBox : PictureBox
	{		
		public MainForm MainForm;	
		public bool[] invalidTiles = new bool[256 * 32];	
		public DirectBitmap levelTiles = new DirectBitmap(4096, 512);
		
		protected override void OnPaint(PaintEventArgs e)
		{
			int zoom = MainForm.zoom;
			
			if(Level.levelData != null)
			{
				bool viewSectors = MainForm.ViewRegions;
				bool viewScroll = MainForm.ViewScroll;
				bool viewObjects = MainForm.ViewObjects;
				bool switchA = MainForm.ViewSwitchA;
				bool switchB = MainForm.ViewSwitchB;
				bool viewColliders = MainForm.ViewColliders;				
				
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
					foreach (Point point in sectorsToDraw)
					{
						DrawTiles(point.X, point.Y, redBrush, g, e.ClipRectangle, viewColliders, switchA, switchB);
					}

					//draw tiles from cache
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;					
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.DrawImage(levelTiles.Bitmap, 0, 0, 4096 * zoom, 512 * zoom);					
					
					//sector objects (enemies, powerups)
					if(viewObjects)
					{
						DrawSectorObjects(font, format, e, enemyBrush);			
						
						//wario position
						int index = Level.warioRightFacing ? 0 : 1;
						Rectangle playerRectangle = Level.playerRectangles[index];
						Rectangle destRect = new Rectangle((Level.warioPosition % 8192 + playerRectangle.X - 32) * zoom, (Level.warioPosition / 8192 + playerRectangle.Y - 56 - index * 64) * zoom, playerRectangle.Width * zoom, playerRectangle.Height * zoom);
						if (destRect.IntersectsWith(e.ClipRectangle))
						{
							e.Graphics.DrawImage(MainForm.playerSprite.Bitmap, destRect, playerRectangle, GraphicsUnit.Pixel);
						}
					}
					
					//scroll
					if(viewScroll)
					{
						foreach (Point point in sectorsToDraw)
						{
							DrawScrollInfo(point.X, point.Y, e.Graphics);
						}
					}

					//sectors
					if(viewSectors)
					{			
						foreach (Point point in sectorsToDraw)
						{						
							DrawSectorInfo(point.X, point.Y, e.Graphics, font, format);
						}
						
						DrawSectorBorders(e.Graphics, e.ClipRectangle);
						
						if (MainForm.currentSector != -1)
						{
							Rectangle sectorRect = new Rectangle((MainForm.currentSector % 16) * 256 * zoom, (MainForm.currentSector / 16) * 256 * zoom, 256 * zoom, 256 * zoom);
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
		
		void DrawTiles(int x, int y, Brush brush, Graphics g, Rectangle clipRectangle, bool viewColliders, bool switchA, bool switchB)
		{
			int zoom = MainForm.zoom;
			
			for(int j = y * 16 ; j < (y + 1) * 16 ; j++)
			{
				for(int i = x * 16 ; i < (x + 1) * 16 ; i++)
				{
					Rectangle destRect = new Rectangle(i * 16 * zoom, j * 16 * zoom, 16 * zoom, 16 * zoom);

					if(destRect.IntersectsWith(clipRectangle))
					{
						if(!invalidTiles[i + j * 256])
						{
							invalidTiles[i + j * 256] = true;
							DrawTileToBitmap(i, j);

							if(viewColliders)
							{
								byte tileIndex = Level.levelData[i + j * 256 + 0x1000];
								if(Level.IsCollidable(Level.Switch(tileIndex, switchA, switchB)))
								{
									g.FillRectangle(brush, new Rectangle(destRect.X / zoom, destRect.Y / zoom, destRect.Width / zoom, destRect.Height / zoom));
								}
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
				Array.Copy(MainForm.tiles16x16.Bits, src.X + (src.Y + i) * MainForm.tiles16x16.Width, levelTiles.Bits, dest.X + (dest.Y + i) * levelTiles.Width, 16);
			}
		}

		void DrawSectorObjects(Font font, StringFormat format, PaintEventArgs e, Brush brush)
		{
			int zoom = MainForm.zoom;
			
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
								e.Graphics.DrawImage(MainForm.tilesObjects.Bitmap, destRect, new Rectangle((data - 7) * 16, 0, 16, 16), GraphicsUnit.Pixel);								
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
								e.Graphics.DrawImage(MainForm.tilesEnemies.Bitmap, destRect, enemyRect, GraphicsUnit.Pixel);
							}
						}			
					}														
				}
			}
		}
		
		void DrawScrollInfo(int x, int y, Graphics g)
		{
			int zoom = MainForm.zoom;
			
			int drawSector = x + y * 16;
			
			byte scroll = Level.scrollData[drawSector];
			if ((scroll & 2) == 2)
				g.FillRectangle(Brushes.Yellow, x * 256 * zoom, y * 256 * zoom, 6 * zoom, 256 * zoom);

			if ((scroll & 1) == 1)
				g.FillRectangle(Brushes.Yellow, ((x+1) * 256 - 6) * zoom, y * 256 * zoom, 6 * zoom, 256 * zoom);
		}
		
		void DrawSectorInfo(int x, int y, Graphics g, Font font, StringFormat format)
		{
			int zoom = MainForm.zoom;
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
			int zoom = MainForm.zoom;
			
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
			int zoom = MainForm.zoom;
			
			List<Point> sectors = new List<Point>();
			
			for(int y = 0 ; y < 2 ; y++)
			{
				for(int x = 0 ; x < 16 ; x++)
				{
					Rectangle destRect = new Rectangle(x * 256 * zoom, y * 256 * zoom, 256 * zoom, 256 * zoom);
					if(destRect.IntersectsWith(clipRectangle))
					{
						sectors.Add(new Point(x, y));
					}
				}
			}
			
			return sectors;
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
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			int zoom = MainForm.zoom;
			bool viewObjects = MainForm.ViewObjects;
			ToolboxForm toolboxForm = MainForm.toolboxForm;
			
			if(e.Button == MouseButtons.Left)
			{
				int tileIndex = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 256;
				Region r = new Region(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom));			
				int selectedPanelIndex = toolboxForm.GetSelectedPanelIndex();
				
				if(MainForm.currentTile != -1 && toolboxForm.Visible && selectedPanelIndex == 0)
				{
					int previousTile = Level.levelData[tileIndex + 0x1000];
					if(previousTile != MainForm.currentTile)
					{
						Level.levelData[tileIndex + 0x1000] = (byte)MainForm.currentTile;
						invalidTiles[tileIndex] = false;
						MainForm.SetChanges(true);
						Invalidate(r);
					}
				}

				if(MainForm.currentObject != -1 && viewObjects && toolboxForm.Visible && selectedPanelIndex == 2)
				{																		
					int previousObject = Level.objectsData[tileIndex];
					if(previousObject != MainForm.currentObject)
					{
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
						addEnemyRegion(MainForm.currentObject);
						
						Level.objectsData[tileIndex] = (byte)MainForm.currentObject;
						MainForm.SetChanges(true);
						Invalidate(r);
					}
				}											
			}
			else if(e.Button == MouseButtons.Right)
			{
				Point coordinates = e.Location;
				int sector = e.Location.X / 256 / zoom + (e.Location.Y / 256 / zoom) * 16;
				if(sector != MainForm.currentSector)
				{
					MainForm.currentSector = sector;
					int warpTarget = Level.SearchWarp(MainForm.rom, MainForm.currentCourseId, MainForm.currentSector);
					if(warpTarget != MainForm.currentWarp)
					{
						MainForm.currentWarp = warpTarget;
						MainForm.LoadLevel(false);
					}
					else
					{
						Refresh();
					}
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (ClientRectangle.Contains(e.Location))
			{
				OnMouseDown(e);
			}
		}
	
		public void SetZoom(int zoom)
		{
			Height = 512 * zoom;
			Width = 4096 * zoom;		
			Refresh();
		}
		
		public void ClearTileCache()
		{
			Array.Clear(invalidTiles, 0, invalidTiles.Length);
		}
	}
}
