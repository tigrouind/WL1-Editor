using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public Rom rom = new Rom();
		public DirectBitmap levelTiles = new DirectBitmap(4096, 512);
		public DirectBitmap tiles8x8 = new DirectBitmap(16 * 8, 16 * 8); 
		public DirectBitmap tiles16x16 = new DirectBitmap(16 * 8, 16 * 16); 
		public DirectBitmap tilesObjects = new DirectBitmap(16 * 9, 16);
		public DirectBitmap tilesEnemies = new DirectBitmap(64, 64 * 6); 		
		public DirectBitmap playerSprite = new DirectBitmap(64, 64 * 2); 
		public bool[] invalidTiles = new bool[256 * 32];
		public int currentSector = -1;
		public int currentTile = -1;
		public int currentObject = -1;
		public int currentCourseId = -1;
		public int currentWarp = -1;
		public string romFilePath;
		public bool hasChanges;
		public int zoom = 1;

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		void LoadLevel(bool reloadAll)
		{
			if(rom.IsLoaded && levelComboBox.SelectedItem != null)
			{
				Array.Clear(invalidTiles, 0, invalidTiles.Length);
				Level.DumpLevel(rom, currentCourseId, currentWarp, tiles8x8, tiles16x16, tilesEnemies, reloadAll, aToolStripMenuItem.Checked, bToolStripMenuItem.Checked, SelectedPaletteToolStripIndex());
				while(currentObject >= 1 && currentObject <= 6 && !Level.enemiesAvailable[currentObject - 1]) currentObject--;
				
				levelPictureBox.Refresh();
				objectPictureBox.Refresh();
				tilesPictureBox.Refresh();
			}
		}


		void LevelComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if(levelComboBox.SelectedItem != null)
			{
				if(AskForSavingChanges())
				{
					currentWarp = -1;
					currentSector = -1;
					ComboboxItem item = (ComboboxItem)levelComboBox.SelectedItem;
					currentCourseId = (int)item.Value;
					LoadLevel(true);
				}
				else
				{
					//restore previous item
					levelComboBox.SelectedIndexChanged -= LevelComboBoxSelectedIndexChanged;
					levelComboBox.SelectedItem = levelComboBox.Items.Cast<ComboboxItem>().First(x => (int)x.Value == currentCourseId);
					levelComboBox.SelectedIndexChanged += LevelComboBoxSelectedIndexChanged;
				}
			}
		}

		void LoadToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "GB ROM Image (*.gb)|*.gb";

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				Rom newRom = new Rom();
				newRom.Load(openFileDialog1.FileName);
				if (newRom.Title == "SUPERMARIOLAND3")
				{
					rom = newRom;
					LoadLevelCombobox();
					Level.DumpBonusSprites(rom, tiles8x8, tilesObjects);
					Level.DumpPlayerSprite(rom, tiles8x8, playerSprite);
					romFilePath = openFileDialog1.FileName;

					if(levelComboBox.SelectedIndex == 0)
						LevelComboBoxSelectedIndexChanged(sender, e);
					else
						levelComboBox.SelectedIndex = 0;
					
					saveAsToolStripMenuItem.Enabled = true;
					levelComboBox.Visible = true;
					LevelPanel.Visible = true;
					tilesPictureBox.Visible = true;
					objectPictureBox.Visible = true;
				}
				else
				{
					MessageBox.Show("Please select a valid WarioLand 1 rom", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

			}
		}			

		void LoadLevelCombobox()
		{
			//init combobox
			levelComboBox.SelectedItem = null;
			levelComboBox.Items.Clear();
			
			List<ComboboxItem> items = new List<ComboboxItem>();			
			foreach(var levelInfo in Level.GetCourseIds(rom).OrderBy(x => x.Value))
			{
				ComboboxItem item = new ComboboxItem(string.Format("{0:D2} {1}", levelInfo.Value, Level.levelNames[levelInfo.Key]), levelInfo.Key);
				items.Add(item);				
			}

			levelComboBox.Items.AddRange(items.ToArray());
		}

		void RegionsToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.Refresh();
		}

		void ObjectsToolStripMenuItemClick(object sender, EventArgs e)
		{
			objectPictureBox.Refresh();
			levelPictureBox.Refresh();
			currentObject = -1;
		}

		void ScrollRegionToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.Refresh();
		}

		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Application.Exit();
		}

		void LevelPictureBoxPaint(object sender, PaintEventArgs e)
		{
			if(Level.levelData != null)
			{
				bool viewSectors = regionsToolStripMenuItem.Checked;
				bool viewScroll = scrollRegionToolStripMenuItem.Checked;
				bool viewObjects = objectsToolStripMenuItem.Checked;
				bool switchA = aToolStripMenuItem.Checked;
				bool switchB = bToolStripMenuItem.Checked;
				bool viewColliders = collidersToolStripMenuItem.Checked;				
				
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
							e.Graphics.DrawImage(playerSprite.Bitmap, destRect, playerRectangle, GraphicsUnit.Pixel);
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
						
						if (currentSector != -1)
						{
							Rectangle sectorRect = new Rectangle((currentSector % 16) * 256 * zoom, (currentSector / 16) * 256 * zoom, 256 * zoom, 256 * zoom);
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
				Array.Copy(tiles16x16.Bits, src.X + (src.Y + i) * tiles16x16.Width, levelTiles.Bits, dest.X + (dest.Y + i) * levelTiles.Width, 16);
			}
		}

		void DrawSectorObjects(Font font, StringFormat format, PaintEventArgs e, Brush brush)
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
								e.Graphics.DrawImage(tilesObjects.Bitmap, destRect, new Rectangle((data - 7) * 16, 0, 16, 16), GraphicsUnit.Pixel);								
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
								e.Graphics.DrawImage(tilesEnemies.Bitmap, destRect, enemyRect, GraphicsUnit.Pixel);
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

		void CollidersToolStripMenuItemClick(object sender, EventArgs e)
		{
			Array.Clear(invalidTiles, 0, invalidTiles.Length);
			levelPictureBox.Refresh();
		}

		void ObjectsPictureBoxPaint(object sender, PaintEventArgs e)
		{
			if(Level.levelData != null)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(tiles16x16.Bitmap, 0, 0, 128 * zoom, 256 * zoom);
	
				if(currentTile != -1)
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
					{
						e.Graphics.FillRectangle(brush, (currentTile % 8) * 16 * zoom, (currentTile / 8) * 16 * zoom, 16 * zoom, 16 * zoom);
					}
				}
			}
		}

		void TilesPictureBoxPaint(object sender, PaintEventArgs e)
		{
			if(Level.levelData != null && objectsToolStripMenuItem.Checked)
			{
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

				using (Brush enemyBrush = new SolidBrush(Level.enemyPalette[2]))
				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Pen pen = new Pen(Color.White, 1.0f * zoom))
				using (Font font = new Font("Arial", 8 * zoom))
				{
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					
					for(int j = 0 ; j < 16 ; j++)
					{
						int x = (j % 4) * 16;
						int y = (j / 4) * 16;
						
						e.Graphics.FillRectangle(enemyBrush, x * zoom, y * zoom, 16 * zoom, 16 * zoom);
						
						if(j == 0)
						{							
							e.Graphics.DrawLine(pen, (x + 4) * zoom, (y + 4) * zoom, (x + 12) * zoom, (y + 12) * zoom);
							e.Graphics.DrawLine(pen, (x + 12) * zoom, (y + 4) * zoom, (x + 4) * zoom, (y + 12) * zoom);
						}
						else if(j <= 6)
						{																	
							e.Graphics.DrawString(j.ToString(), font, Level.enemiesAvailable[j - 1] ? Brushes.White : Brushes.DimGray, (x + 8) * zoom, (y + 8) * zoom, format);
						}
						else
						{
							e.Graphics.DrawImage(tilesObjects.Bitmap, new Rectangle(x * zoom, y * zoom, 16 * zoom, 16 * zoom), new Rectangle((j - 7) * 16, 0, 16, 16),  GraphicsUnit.Pixel);
						}
					}
					
					if(currentObject != -1)
					{
						e.Graphics.FillRectangle(brush, (currentObject % 4) * 16 * zoom, (currentObject / 4) * 16 * zoom, 16 * zoom, 16 * zoom);
					}
				}
			}
		}

		void NoneToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetSwitchMode(noneToolStripMenuItem);
			LoadLevel(false);
		}

		void AToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetSwitchMode(aToolStripMenuItem);
			LoadLevel(false);
		}

		void BToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetSwitchMode(bToolStripMenuItem);
			LoadLevel(false);
		}

		void SetSwitchMode(ToolStripItem toolStrip)
		{
			noneToolStripMenuItem.Checked = toolStrip == noneToolStripMenuItem;
			aToolStripMenuItem.Checked = toolStrip == aToolStripMenuItem;
			bToolStripMenuItem.Checked = toolStrip == bToolStripMenuItem;
		}

		bool SaveChanges()
		{
			if(rom.IsLoaded)
			{
				string message;
				if(!Level.SaveChanges(rom, currentCourseId, romFilePath, out message))
				{
					MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}

			SetChanges(false);
			return true;
		}

		bool AskForSavingChanges()
		{
			if(hasChanges)
			{
				DialogResult result = MessageBox.Show("Save pending changes ?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch(result)
				{
					case DialogResult.Yes:
						return SaveChanges();

					case DialogResult.No:
						SetChanges(false);
						return true;

					case DialogResult.Cancel:
						return false;

					default:
						throw new NotImplementedException();
				}
			}
			else
			{
				return true;
			}
		}
		
		void SetChanges(bool hasChanges)
		{			
			this.hasChanges = hasChanges;
			saveToolStripMenuItem.Enabled = hasChanges;
		}

		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveChanges();
		}

		void ObjectsPictureBoxMouseDown(object sender, MouseEventArgs e)
		{
			currentTile = e.Location.X / 16 / zoom+ (e.Location.Y / 16 / zoom) * 8;
			tilesPictureBox.Refresh();

			if(currentObject != -1)
			{
				currentObject = -1;
				objectPictureBox.Refresh();
			}
		}

		void TilesPictureBoxMouseDown(object sender, MouseEventArgs e)
		{
			int index = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 4;
			if(!(index >= 1 && index <= 6) || Level.enemiesAvailable[index - 1])
			{
				currentObject = index;
				objectPictureBox.Refresh();
	
				if(currentTile != -1)
				{
					currentTile = -1;
					tilesPictureBox.Refresh();
				}			
			}					
		}

		void LevelPictureBoxMouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				int tileIndex = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 256;
				Region r = new Region(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom));			

				if(currentTile != -1)
				{
					int previousTile = Level.levelData[tileIndex + 0x1000];
					if(previousTile != currentTile)
					{
						Level.levelData[tileIndex + 0x1000] = (byte)currentTile;
						invalidTiles[tileIndex] = false;
						SetChanges(true);
						levelPictureBox.Invalidate(r);
					}
				}

				if(currentObject != -1)
				{																		
					int previousObject = Level.objectsData[tileIndex];
					if(previousObject != currentObject)
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
						addEnemyRegion(currentObject);
						
						Level.objectsData[tileIndex] = (byte)currentObject;
						SetChanges(true);
						levelPictureBox.Invalidate(r);
					}
				}											
			}
			else if(e.Button == MouseButtons.Right)
			{
				Point coordinates = e.Location;
				int sector = e.Location.X / 256 / zoom + (e.Location.Y / 256 / zoom) * 16;
				if(sector != currentSector)
				{
					currentSector = sector;
					int warpTarget = Level.SearchWarp(rom, currentCourseId, currentSector);
					if(warpTarget != currentWarp)
					{
						currentWarp = warpTarget;
						LoadLevel(false);
					}
					else
					{
						levelPictureBox.Refresh();
					}
				}
			}
		}

		void LevelPictureBoxMouseMove(object sender, MouseEventArgs e)
		{
			if (levelPictureBox.ClientRectangle.Contains(e.Location))
			{
				LevelPictureBoxMouseDown(sender, e);
			}
		}

		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !AskForSavingChanges();
		}

		void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "GB ROM Image (*.gb)|*.gb";

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				romFilePath = saveFileDialog.FileName;
				SaveChanges();
			}
		}

		void ClassicToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetPalette(classicToolStripMenuItem);
			LoadLevel(false);
		}

		void BlackWhiteToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetPalette(blackWhiteToolStripMenuItem);
			LoadLevel(false);
		}

		void AutumnToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetPalette(autumnToolStripMenuItem);
			LoadLevel(false);
		}

		int SelectedPaletteToolStripIndex()
		{
			if(classicToolStripMenuItem.Checked) return 0;
			if(blackWhiteToolStripMenuItem.Checked) return 1;
			return 2;
		}

		void SetPalette(ToolStripMenuItem toolStrip)
		{
			classicToolStripMenuItem.Checked = toolStrip == classicToolStripMenuItem;
			blackWhiteToolStripMenuItem.Checked = toolStrip == blackWhiteToolStripMenuItem;
			autumnToolStripMenuItem.Checked = toolStrip == autumnToolStripMenuItem;
		}
		
		void SetZoomLevel(int zoomLevel)
		{
			zoom = zoomLevel;
			
			zoom100ToolStripMenuItem.Checked = zoomLevel == 1;
			zoom200ToolStripMenuItem.Checked = zoomLevel == 2;	
			zoom300ToolStripMenuItem.Checked = zoomLevel == 3;	
			zoom400ToolStripMenuItem.Checked = zoomLevel == 4;	
			
			levelPictureBox.Height = 512 * zoom;
			levelPictureBox.Width = 4096 * zoom;		
			levelPictureBox.Refresh();
			
			tilesPictureBox.Height = 256 * zoom;
			tilesPictureBox.Width = 128 * zoom;							
			tilesPictureBox.Refresh();
			
			objectPictureBox.Height = 64 * zoom;
			objectPictureBox.Width = 64 * zoom;
			objectPictureBox.Refresh();
		}
						
		void Zoom100ToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetZoomLevel(1);			
		}
		
		void Zoom200ToolStripMenuItemClick(object sender, EventArgs e)
		{			
			SetZoomLevel(2);
		}
		
		void Zoom300ToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetZoomLevel(3);
		}
		
		void Zoom400ToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetZoomLevel(4);
		}
	}
}
