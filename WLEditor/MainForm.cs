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

			//convert course id => course no using data in ROM
			rom.SetBank(0);
			var courseIdToNo = new Dictionary<int, int>();
			List<ComboboxItem> items = new List<ComboboxItem>();
			for(int i = 0 ; i <= 0x2A ; i++)
			{
				int levelpointer = rom.ReadWord(0x0534 + i * 2);
				int courseNo = (levelpointer - 0x0587) / 3;
				ComboboxItem item = new ComboboxItem(string.Format("{0:D2} {1}", courseNo, Level.levelNames[i]), i);
				items.Add(item);
				courseIdToNo.Add(i, courseNo);
			}

			items = items.OrderBy(x => courseIdToNo[(int)x.Value]).ToList();
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
			bool viewSectors = regionsToolStripMenuItem.Checked;
			bool viewScroll = scrollRegionToolStripMenuItem.Checked;

			if(Level.levelData != null)
			{
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;	
			
				var sectorsToDraw = GetVisibleSectors(e.ClipRectangle);

				using (Brush redBrush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Font font = new Font("Arial", 8 * zoom))				
				using (Graphics g = Graphics.FromImage(levelTiles.Bitmap))
				{
					//draw tiles to cache
					foreach (Point point in sectorsToDraw)
					{
						DrawTiles(point.X, point.Y, redBrush, g, e);
					}

					//draw tiles from cache
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;					
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.DrawImage(levelTiles.Bitmap, 0, 0, 4096 * zoom, 512 * zoom);					
					
					//sector objects (enemies, powerups)
					foreach (Point point in sectorsToDraw)
					{
						DrawSectorObjects(point.X, point.Y, font, format, e);			
					}					
					
					if(viewSectors)
					{
						//wario position
						Rectangle destRect = new Rectangle((Level.warioPosition % 8192 - 8) * zoom, (Level.warioPosition / 8192 - 16) * zoom, 16 * zoom, 16 * zoom);
						if (destRect.IntersectsWith(e.ClipRectangle))
						{
							e.Graphics.FillRectangle(Brushes.Green, destRect);
							e.Graphics.DrawString("W", font, Brushes.White, destRect, format);
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

		void DrawTiles(int x, int y, Brush brush, Graphics g, PaintEventArgs e)
		{
			bool switchA = aToolStripMenuItem.Checked;
			bool switchB = bToolStripMenuItem.Checked;
			bool viewColliders = collidersToolStripMenuItem.Checked;

			for(int j = y * 16 ; j < (y + 1) * 16 ; j++)
			{
				for(int i = x * 16 ; i < (x + 1) * 16 ; i++)
				{
					Rectangle destRect = new Rectangle(i * 16 * zoom, j * 16 * zoom, 16 * zoom, 16 * zoom);

					if(destRect.IntersectsWith(e.ClipRectangle))
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

		void DrawTileToBitmap(int sectorX, int sectorY)
		{
			byte tileIndex = Level.levelData[sectorX + sectorY * 256 + 0x1000];
			Point dest = new Point(sectorX * 16, sectorY * 16);
			Point src = new Point((tileIndex % 8) * 16, (tileIndex / 8) * 16);

			for(int y = 0 ; y < 16 ; y++)
			{
				Array.Copy(tiles16x16.Bits, src.X + (src.Y + y) * tiles16x16.Width, levelTiles.Bits, dest.X + (dest.Y + y) * levelTiles.Width, 16);
			}
		}

		void DrawSectorObjects(int x, int y, Font font, StringFormat format, PaintEventArgs e)
		{
			bool viewObjects = objectsToolStripMenuItem.Checked;
			bool viewSectors = regionsToolStripMenuItem.Checked;

			for(int j = y * 16 ; j < (y + 1) * 16 ; j++)
			{
				for(int i = x * 16 ; i < (x + 1) * 16 ; i++)
				{
					Rectangle destRect = new Rectangle(i * 16 * zoom, j * 16 * zoom, 16 * zoom, 16 * zoom);

					if(destRect.IntersectsWith(e.ClipRectangle))
					{
						//tile blocks
						byte tileIndex = Level.levelData[i + j * 256 + 0x1000];
						
						if(viewSectors)
						{
							int sectorTarget = Level.warps[x + y * 16];
							if(Level.IsDoor(tileIndex) && sectorTarget != 255)
							{
								e.Graphics.FillRectangle(Brushes.Green, destRect);
								e.Graphics.DrawString("D", font, Brushes.White, (i * 16 + 8) * zoom, (j * 16 + 8) * zoom, format);
							}
						}

						//objects
						if(viewObjects)
						{
							byte data = Level.objectsData[i + j * 256];
							if(data != 0)
							{
								e.Graphics.FillRectangle(Brushes.DarkViolet, destRect);
								
								if(data <= 6)
								{
									if(!Level.loadedSprites.Contains(data))
									{
										e.Graphics.DrawString(data.ToString(), font, Brushes.White, (i * 16 + 8) * zoom, (j * 16 + 8) * zoom, format);
									}
								}
								else
								{
									e.Graphics.DrawImage(tilesObjects.Bitmap, new Rectangle(i * 16 * zoom, j * 16 * zoom, 16 * zoom, 16 * zoom), new Rectangle((data - 7) * 16, 0, 16, 16), GraphicsUnit.Pixel);
								}
							}
						}
					}
					
					//objects sprites							
					if(viewObjects)
					{											
						byte data = Level.objectsData[i + j * 256];
						if(data > 0 && data <= 6 && Level.loadedSprites.Contains(data))
						{
							destRect = new Rectangle((i * 16 + 8 - 32) * zoom, (j * 16 + 8 - 48) * zoom, 64 * zoom, 64 * zoom);
							if(destRect.IntersectsWith(e.ClipRectangle))
							{
								e.Graphics.DrawImage(tilesEnemies.Bitmap, new Rectangle((i * 16 - 16 - 8)*zoom, (j * 16 - 40)*zoom, 64 * zoom, 64 * zoom), new Rectangle(0, (data - 1) * 64, 64, 64), GraphicsUnit.Pixel);	
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
					Rectangle lineRect = new Rectangle(x - 2, 0, 4, 512 * zoom);
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
					warpText = "START";
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

		void TilesPictureBoxPaint(object sender, PaintEventArgs e)
		{
			if(Level.levelData != null && objectsToolStripMenuItem.Checked)
			{
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

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
						
						e.Graphics.FillRectangle(Brushes.DarkViolet, (j % 4) * 16 * zoom, (j / 4) * 16 * zoom, 16 * zoom, 16 * zoom);
						
						if(j == 0)
						{							
							e.Graphics.DrawLine(pen, (x + 4) * zoom, (y + 4) * zoom, (x + 12) * zoom, (y + 12) * zoom);
							e.Graphics.DrawLine(pen, (x + 12) * zoom, (y + 4) * zoom, (x + 4) * zoom, (y + 12) * zoom);
						}
						else if(j <= 6)
						{						
							e.Graphics.DrawString(j.ToString(), font, Brushes.White, (x + 8) * zoom, (y + 8) * zoom, format);
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
			currentObject = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 4;
			objectPictureBox.Refresh();

			if(currentTile != -1)
			{
				currentTile = -1;
				tilesPictureBox.Refresh();
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
					Level.levelData[tileIndex + 0x1000] = (byte)currentTile;
					invalidTiles[tileIndex] = false;
					SetChanges(true);
				}

				if(currentObject != -1)
				{
					Level.objectsData[tileIndex] = (byte)currentObject;
					int objectIndex = Level.objectsData[tileIndex];
					if(objectIndex == 0 || Level.loadedSprites.Contains(objectIndex))
					{
						r.Union(new Rectangle(((tileIndex % 256) * 16 + 8 - 32) * zoom, ((tileIndex / 256) * 16 + 8 - 48) * zoom, 64 * zoom, 64 * zoom));
					}
					SetChanges(true);
				}				
				
				levelPictureBox.Invalidate(r);
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
	}
}
