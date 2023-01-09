using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;

namespace WLEditor
{
	public partial class Overworld : Form
	{		
		public event EventHandler<KeyEventArgs> ProcessCommandKey;	
		public event EventHandler WorldMapChanged;
		
		Rom rom;
		int zoom;		
		DirectBitmap tilesWorld8x8 = new DirectBitmap(16 * 8, 16 * 8);
		DirectBitmap tilesWorld = new DirectBitmap(32 * 8, 32 * 8);
		int currentWorld;
		
		bool eventMode;
		bool pathMode;
		bool ignoreEvents;
		int timerTicks;
		bool formLoaded;
		
		int lastTilePos = -1;
		int hasChanges;		
		
		Selection selection = new Selection(8);
		bool selectionMode;
		
		byte[] worldTiles = new byte[32 * 32];
		
		readonly PathForm pathForm;
		readonly EventForm eventForm;
						
		public Overworld()
		{
			InitializeComponent();
			eventForm = new EventForm(pictureBox1, tilesWorld8x8);
			eventForm.EventIndexChanged += (s, e) => UpdateTitle();
			eventForm.EventChanged += (s, e) => SetChanges(2);
			
			pathForm = new PathForm(pictureBox1);
			pathForm.PathChanged += (s, e) => SetChanges(4);			
			
			selection.InvalidatePictureBox += InvalidatePictureBox;
		}
		
		readonly ComboboxItem<int[]>[] worldData = new ComboboxItem<int[]>[]
		{
			//bank - 8x8 tiles / bank - map tiles / max tiles size 
			new ComboboxItem<int[]>(new int[] { 0x09, 0x407A, 0x09, 0x6DBE, 373  }, "1 Rice Beach"), 
			new ComboboxItem<int[]>(new int[] { 0x09, 0x407A, 0x09, 0x74E1, 346  }, "1 Rice Beach - FLOODED"),
			new ComboboxItem<int[]>(new int[] { 0x09, 0x407A, 0x09, 0x6F33, 368  }, "2 Mt. Teapot"), 
			new ComboboxItem<int[]>(new int[] { 0x09, 0x4E13, 0x09, 0x70A3, 371  }, "3 Stove Canyon"),
			new ComboboxItem<int[]>(new int[] { 0x09, 0x4E13, 0x09, 0x7216, 393  }, "4 SS Tea Cup"),
			new ComboboxItem<int[]>(new int[] { 0x14, 0x6909, 0x14, 0x7813, 388  }, "5 Parsley Woods"),
			new ComboboxItem<int[]>(new int[] { 0x14, 0x6909, 0x14, 0x76D2, 321  }, "6 Sherbet Land"), 
			new ComboboxItem<int[]>(new int[] { 0x09, 0x5C6C, 0x09, 0x739F, 322  }, "7 Syrup Castle" ),			
			new ComboboxItem<int[]>(new int[] { 0x14, 0x5AA0, 0x09, 0x6AA5, 787 }, "Overworld"),	
		};
				
		public void LoadRom(Rom rom)
		{	
			this.rom = rom;					
			formLoaded = false;
			hasChanges = 0;
			
			if (Visible)
			{
				InitForm();
			}
		}
		
		void OverworldVisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				InitForm();
			}
		}
		
		void InitForm()
		{
			if (!formLoaded)
			{
				formLoaded = true;
				LoadWorldCombobox();
				
				ignoreEvents = true;
				WorldComboBox.SelectedIndex = 0;
				ignoreEvents = false;
				
				currentWorld = 0;
				pathForm.LoadPaths(rom);
				LoadWorld();
				SetZoom(zoom);			
			}
		}
				
		void LoadWorld()
		{
			var data = worldData[currentWorld].Value;
			Map.Dump8x8Tiles(rom, data[0], data[1], tilesWorld8x8);
			if (timerTicks != 0)
			{
				DumpAnimatedTiles();
			}
			
			Map.LoadTiles(rom, data[2], data[3], worldTiles);			
			
			eventForm.LoadWorld(rom, currentWorld);
			pathForm.LoadWorld(currentWorld);
			
			selection.ClearUndo();
			
			UpdateTitle();			
			RenderMap();
		}
			
		void LoadWorldCombobox()
		{
			WorldComboBox.Items.Clear();
			for(int i = 0 ; i < worldData.Length ;i++)
			{
				WorldComboBox.Items.Add(worldData[i]);
			}
		}
		
		void WorldComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{				
				if (!SaveChanges())
				{
					ignoreEvents = true;
					WorldComboBox.SelectedIndex = currentWorld;
					ignoreEvents = false;
					return;
				}
				
				currentWorld = WorldComboBox.SelectedIndex;				
				LoadWorld();
				selection.ClearSelection();
				SetZoom(zoom);
			}
		}
		
		public bool SaveChanges()
		{
			if (hasChanges != 0)
			{
				string message;				
				if ((hasChanges & 1) != 0)
				{
					//improve tile compression
					if (currentWorld != 8)
					{
						for (int y =  0; y < 17; y++)	
						{
							var data = worldTiles[19 + y * 32];
							for (int x = 20; x < 32; x++)
							{
								worldTiles[x + y * 32] = data;
							}
						}
					}
					
					var worldInfo = worldData[currentWorld].Value;					
					if (!Map.SaveTiles(rom, worldInfo[2], worldInfo[3],
					                   currentWorld == 8 ? worldTiles : worldTiles.Take(564).ToArray(), worldInfo[4], out message))
					{									
						MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}					
				}
				
				hasChanges &= ~1;
				
				if ((hasChanges & 2) != 0 && !eventForm.SaveEvents(rom, out message))
				{
					MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);										
					return false;
				}
				
				hasChanges &= ~2;
				
				if ((hasChanges & 4) != 0 && !pathForm.SavePaths(rom, out message))
				{
					MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);										
					return false;
				}					

				hasChanges &= ~4;				
			}
			return true;
		}
		
		void PictureBox1Paint(object sender, PaintEventArgs e)
		{
			if(!DesignMode)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(tilesWorld.Bitmap, 
				                     new Rectangle(0, 0, currentMapX * 8 * zoom, currentMapY * 8 * zoom),
				                     new Rectangle(0, 0, currentMapX * 8, currentMapY * 8), 
				                     GraphicsUnit.Pixel);
				
				if (eventMode)
				{
					eventForm.DrawEvents(e.Graphics);
				}	
				if (pathMode)
				{					
					pathForm.DrawPaths(e.Graphics);
					pathForm.DrawProgress(e.Graphics);
					pathForm.DrawLevels(e.Graphics);
					pathForm.DrawExits(e.Graphics);
				}
				if (selectionMode) 
				{
					selection.DrawSelection(e.Graphics);
				}				
			}
		}		
		
		void PictureBox2Paint(object sender, PaintEventArgs e)
		{
			if(!DesignMode)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(tilesWorld8x8.Bitmap, 0, 0, 128 * zoom, 128 * zoom);	
				
				if (!selectionMode)
				{
					selection.DrawSelection(e.Graphics);
				}
			}
		}	
		
		public void SetZoom(int zoomlevel)
		{			
			pictureBox1.Width = currentMapX * 8 * zoomlevel;
			pictureBox1.Height = currentMapY * 8 * zoomlevel;
			
			pictureBox2.Width = 128 * zoomlevel;
			pictureBox2.Height = 128 * zoomlevel;				
			
			pictureBox1.Invalidate();
			pictureBox2.Invalidate();
			
			eventForm.SetZoom(zoomlevel);
			pathForm.SetZoom(zoomlevel);			
			selection.SetZoom(zoomlevel);
			
			zoom = zoomlevel;			
		}
		
		void OverworldFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing) 
			{
				e.Cancel = true;
				Hide();
			}
		}	

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			KeyEventArgs args = new KeyEventArgs(keyData);
			
			if (DispatchCommandKey(keyData))
		    {
				return true;    	
		    }
			
			ProcessCommandKey(this, args);
			if(args.Handled)
			{
				return true;
			}
						
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		bool DispatchCommandKey(Keys keyData)
		{
			if (keyData == Keys.E)
			{
				pathMode = false;						
				
				eventMode = !eventMode;
				UpdateTitle();
				pictureBox1.Invalidate();
				pictureBox2.Visible = true;
				UpdateBounds();
				return true;
			}
			
			if (keyData == Keys.P)
			{				
				eventMode = false;
				pathMode = !pathMode;
				UpdateTitle();
				pictureBox1.Invalidate();
				pictureBox2.Visible = !pathMode;
				UpdateBounds();
				selection.ClearSelection();
				return true;
			}
			
			if (keyData == (Keys.Control | Keys.C))
			{
				CopySelection();
				return true;
			}
			
			if (keyData == (Keys.Control | Keys.V))
			{					
				PasteSelection();
				return true;
			}
			
			if (keyData == (Keys.Control | Keys.Z))
			{
				Undo();
				return true;
			}
			
			if (keyData == (Keys.Control | Keys.Y))
			{				
				Redo();
				return true;
			}
			
			if (eventMode && eventForm.ProcessEventKey(keyData))
			{
				return true;
			}
			
			if (pathMode && pathForm.ProcessPathKey(keyData))
			{
				return true;
			}
			
			if (keyData >= Keys.A && keyData <= Keys.Z)
			{
				return true;
			}
			
			return false;
		}
				
		void SetChanges(int mode)
		{
			hasChanges |= mode;
			WorldMapChanged(this, EventArgs.Empty);
		}
		
		void RenderTiles()
		{			
			for(int y = 0 ; y < currentMapY ; y++)
			{
				for(int x = 0 ; x < currentMapX ; x++)
				{
					int tileIndex = worldTiles[x + y * 32];					
					Dump8x8Tile(new Point(x * 8, y * 8), tileIndex ^ 0x80, tilesWorld);
				}			
			}
		}	
		
		void RenderMap()
		{
			Array.Clear(tilesWorld.Bits, 0, tilesWorld.Bits.Length);
			RenderTiles();	
			ScrollLines();
			
			pictureBox1.Invalidate();
			pictureBox2.Invalidate();
		}
				
		void Dump8x8Tile(Point dest, int tileIndex, DirectBitmap bitmap)
		{
			Point source = new Point((tileIndex % 16) * 8, (tileIndex / 16) * 8);										
			for(int y = 0 ; y < 8 ; y++)
			{
				Array.Copy(tilesWorld8x8.Bits, source.X + (source.Y + y) * tilesWorld8x8.Width, 
				           bitmap.Bits, dest.X + (dest.Y + y) * bitmap.Width, 8);
			}
		}
				
		void UpdateTitle()
		{
			if (eventMode)
			{
				Text = "Overworld - " + eventForm.GetTitle();
			}
			else
			{
				Text = "Overworld";
			}
		}	
		
		int currentMapX
		{
			get
			{
				return currentWorld == 8 ? 32 : 20;
			}
		}
		
		int currentMapY
		{
			get
			{
				return currentWorld == 8 ? 32 : 18;
			}			
		}
			
		#region Mouse
		
		void MouseEvent(MouseEventArgs e, bool down)
		{
			int tilePosX = e.Location.X / 8 / zoom;
			int tilePosY = e.Location.Y / 8 / zoom;
			
			int tilePos = tilePosX + tilePosY * 32;
			if (tilePos != lastTilePos)
			{
				lastTilePos = tilePos;	
				if (e.Button == MouseButtons.Left)
				{
					if (!pathMode)
					{
						if (down)
						{
							selection.StartSelection(tilePosX, tilePosY);					
						}
						else
						{
							selection.SetSelection(tilePosX, tilePosY);					
						}
					}
				}	
				else if (e.Button == MouseButtons.Right)
				{
					if (!pathMode)
					{
						if (eventMode)
						{
							eventForm.RemoveEvent(tilePos);
						}
						
						selection.ClearSelection();
					}
				}				
			}
		}
		
		void PictureBox1MouseMove(object sender, MouseEventArgs e)
		{
			if (pictureBox1.ClientRectangle.Contains(e.Location))
			{
				MouseEvent(e, false);
			}
		}
		
		void PictureBox1MouseDown(object sender, MouseEventArgs e)
		{
			lastTilePos = -1;
			if (!selectionMode)
			{				
				selection.ClearSelection();
				selectionMode = true;
			}
			
			MouseEvent(e, true);
		}	

		void PictureBox2MouseMove(object sender, MouseEventArgs e)
		{
			if (pictureBox2.ClientRectangle.Contains(e.Location))
			{
				MouseEvent(e, false);
			}
		}			
		
		void PictureBox2MouseDown(object sender, MouseEventArgs e)
		{
			lastTilePos = -1;
			if (selectionMode) 
			{				
				selection.ClearSelection();
				selectionMode = false;
			}
			
			MouseEvent(e, true);
		}
		
		#endregion		
		
		#region Selection
		
		void CopySelection()
		{
			if (!pathMode)
			{
				selection.CopySelection((x, y) =>
				{
	            	if (selectionMode)
					{
	            		return GetTileAt(x, y);
					}
	            	
					return (byte)((x + y * 16) ^ 0x80);
	        	});
			}
		}
		
		void PasteSelection()
		{
			if (!pathMode && selectionMode)
			{
				selection.PasteSelection((x, y, data) => 
				{   	
	             	if (x < currentMapX && y < currentMapY)
	             	{	                     	
						if (eventMode)
						{
							if (data != 0xFF) //not allowed because used as a marker
							{
								eventForm.AddEvent((byte)data, x + y * 32);
							}
						}
						else
						{							
							int previous = GetTileAt(x, y);
							worldTiles[x + y * 32] = (byte)data;							
							return previous;
						}					
	             	}
	             	
	             	return -1;
				});
				
				RenderMap();
				SetChanges(1);
			}
		}
		
		int GetTileAt(int x, int y)
		{
			return worldTiles[x + y * 32];
		}
		
		void SetTileAt(int x, int y, int data)
		{
			worldTiles[x + y * 32] = (byte)data;
		}
		
		void InvalidatePictureBox(object sender, SelectionEventArgs e)
		{	
			if (selectionMode)
			{
				pictureBox1.Invalidate(e.ClipRectangle);
			}
			else
			{
				pictureBox2.Invalidate(e.ClipRectangle);
			}
		}
		
		#endregion
		
		#region Undo
				
		void Undo()
		{
			if (!eventMode && !pathMode)
			{
				selection.Undo(SetTileAt, GetTileAt);
				RenderMap();	
				SetChanges(1);
			}
		}
		
		void Redo()
		{
			if (!eventMode && !pathMode)
			{
				selection.Redo(SetTileAt, GetTileAt);
				RenderMap();	
				SetChanges(1);
			}
		}
				
		#endregion
		
		#region Animation
				
		readonly int[] animationSea = 
		{
			0x5B18, 208,
			0x5B49, 209,
			0x5B7A, 224,
			0x5BAB, 225,
			0x5BDC, 226,
			0x5C0D, 240,
			0x5C3E, 241,
			0x5C6F, 242,
			0x5CA0, 243,
			0x5CD1, 244
		};
		
		readonly int[] animationLava =
		{
			0x5222, 218,
			0x5242, 202,
			0x5252, 203,
			0x5232, 219,
			0x5232, 218,
			0x5252, 202,			
			0x5242, 203,
			0x5222, 219,			
		};
		
		readonly int[] animationWater =
		{
			0x518D, 53,
			0x519D, 54,
			0x519D, 53,
			0x518D, 54,
		};
				
		public void TimerTick()
		{	
			timerTicks++;
			if (Visible)
			{				
				switch (currentWorld)
				{
					case 0:
					case 1:
					case 3:
					case 4:
					case 7:		
					case 8:
						DumpAnimatedTiles();
						RenderMap();					
						break;
				}				
			}
		}
		
		public void ResetTimer()
		{
			timerTicks = 0;
			if (Visible)
			{				
				RenderMap();		
			}
		}
		
		void DumpAnimatedTiles()
		{
			int animationIndex = timerTicks / 3;
			switch (currentWorld)
			{
				case 0:
				case 1:
					for (int i = 0; i < 10; i++)
					{
						Map.DumpAnimatedTilesA(rom, animationSea[i * 2], animationSea[i * 2 + 1], tilesWorld8x8, animationIndex % 6, 6);					
					}
					break;
					
				case 3:
					for (int i = 0; i < 4; i++)
					{
						int index = i + (animationIndex % 2) * 4;
						Map.DumpAnimatedTilesB(rom, animationLava[index * 2], animationLava[index * 2 + 1], tilesWorld8x8);
					}
					break;
					
				case 4:
					for (int i = 0; i < 2; i++)
					{
						int index = i + (animationIndex % 2) * 2;
						Map.DumpAnimatedTilesB(rom, animationWater[index * 2], animationWater[index * 2 + 1], tilesWorld8x8);
					}
					break;
										
				case 8:
					Map.DumpAnimatedTilesA(rom, 0x46F6, 42, tilesWorld8x8, animationIndex % 8, 8);
					break;					
			}						
		}
		
		void ScrollLines()
		{
			if (currentWorld == 7 && timerTicks != 0)
			{
				int animationIndex = timerTicks / 2;
				uint[] line = new uint[160];
				
				for(int y = 0 ; y < 54 ; y++)
				{					
					int scroll = Map.GetScroll(rom, animationIndex + y);
					for(int x = 0 ; x < line.Length ; x++)
					{
						line[(x + scroll + line.Length) % line.Length] = tilesWorld.Bits[x + y * 256];
					}
					Array.Copy(line, 0, tilesWorld.Bits, y * 256, line.Length);
				}
			}
		}
		
		#endregion		
	}
}
