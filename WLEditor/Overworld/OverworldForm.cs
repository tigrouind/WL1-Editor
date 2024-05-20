using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class OverworldForm : Form
	{
		public event EventHandler<KeyEventArgs> ProcessCommandKey;
		public event EventHandler WorldMapChanged;

		Rom rom;
		int zoom;
		readonly DirectBitmap tilesWorld8x8 = new DirectBitmap(16 * 8, 16 * 8);
		readonly DirectBitmap tilesWorld = new DirectBitmap(32 * 8, 32 * 8);
		readonly DirectBitmap tilesWorldScroll = new DirectBitmap(32 * 8, 32 * 8);
		int currentWorld;

		List<SelectionChange> changes = new List<SelectionChange>();
		int SelectedTile => selection2.GetCurrentTile((x, y) => x + y * 16);

		bool eventMode;
		bool pathMode;
		bool ignoreEvents;
		bool formLoaded;

		int timerTicks;
		int animationIndex;

		int currentTile = -1;
		int lastTile = -1;
		bool lastTileSide;
		ChangeEnum changesFlag;

		readonly Selection selection1 = new Selection(8);
		readonly Selection selection2 = new Selection(8);
		readonly History history = new History();
		bool selectionMode;

		readonly byte[] worldTiles = new byte[32 * 32];
		readonly int[] previousWorldTiles = new int[32 * 32];
		readonly bool[] invalidTiles = new bool[16 * 16];

		readonly PathForm pathForm;
		readonly EventForm eventForm;

		public OverworldForm()
		{
			InitializeComponent();
			eventForm = new EventForm(pictureBox1, tilesWorld8x8, selection1, history);
			eventForm.UpdateTitle = UpdateTitle;
			eventForm.EventChanged += (s, e) => SetChanges();

			pathForm = new PathForm(pictureBox1);
			pathForm.UpdateTitle = UpdateTitle;
			pathForm.PathChanged += (s, e) => SetChanges();
			pathForm.GetAnimationIndex = () => animationIndex;

			selection1.InvalidateSelection += (s, e) => pictureBox1.Invalidate(e.ClipRectangle);
			selection2.InvalidateSelection += (s, e) => pictureBox2.Invalidate(e.ClipRectangle);
		}

		readonly ComboboxItemCollection<(int BankA, int TileAddressA, int MaxLengthA, byte Palette, int BankB, int TileAddressB, int MaxLengthB, int UncompressedSize)> worldData = new ComboboxItemCollection<(int, int, int, byte, int, int, int, int)>
		{
			{ ( 0x09, 0x407A, 3477, 0xE1, 0x09, 0x6DBE, 373,  564 ), "1 Rice Beach" },
			{ ( 0x09, 0x407A, 3477, 0xE1, 0x09, 0x74E1, 346,  564 ), "1 Rice Beach - FLOODED" },
			{ ( 0x09, 0x407A, 3477, 0xE1, 0x09, 0x6F33, 368,  564 ), "2 Mt. Teapot" },
			{ ( 0x14, 0x6909, 3524, 0xE1, 0x14, 0x76D2, 321,  564 ), "3 Sherbet Land" },
			{ ( 0x09, 0x4E13, 3667, 0xE1, 0x09, 0x70A3, 371,  564 ), "4 Stove Canyon" },
			{ ( 0x09, 0x4E13, 3667, 0xE1, 0x09, 0x7216, 393,  564 ), "5 SS Tea Cup" },
			{ ( 0x14, 0x6909, 3524, 0xE1, 0x14, 0x7813, 388,  564 ), "6 Parsley Woods" },
			{ ( 0x09, 0x5C6C, 3639, 0xE1, 0x09, 0x739F, 322,  564 ), "7 Syrup Castle" },
			{ ( 0x09, 0x5C6C, 3639, 0xE1, 0x09, 0x763B, 247,  564 ), "7 Syrup Castle - DEFEATED" },
			{ ( 0x14, 0x5AA0, 3682, 0xE1, 0x09, 0x6AA5, 787, 1024 ), "8 Overworld" },

			{ ( 0x12, 0x54C2, 4761, 0xE4, 0x12, 0x6763, 199, 308 ), "Title screen" },
			{ ( 0x06, 0x5706, 4141, 0xD0, 0x06, 0x6737, 306, 564 ), "Save Selection" },
			{ ( 0x06, 0x7469, 1085, 0xE4, 0x06, 0x78AA, 284, 564 ), "Mini Game Selection" },
			{ ( 0x1E, 0x7236, 2859, 0xE1, 0x1E, 0x7D67, 346, 564 ), "Mini Game 1" },
			{ ( 0x1E, 0x57CF, 4731, 0xE4, 0x1E, 0x6A50, 337, 564 ), "Mini Game 2" },
			{ ( 0x1E, 0x57CF, 4731, 0xE4, 0x1E, 0x6BA1, 258, 564 ), "Mini Game 2" },
			{ ( 0x1E, 0x57CF, 4731, 0xE4, 0x1E, 0x6CA3, 352, 564 ), "Mini Game 2" },
			{ ( 0x06, 0x41E3, 5078, 0xE1, 0x06, 0x55BC, 330, 564 ), "Treasure" },
			{ ( 0x05, 0x4FF1, 2289, 0xE1, 0x05, 0x58EA,  79, 564 ), "Time up" },
			{ ( 0x05, 0x4FF1, 2289, 0xE1, 0x05, 0x593A,  77, 564 ), "Time up" },
			{ ( 0x05, 0x4FF1, 2289, 0xE1, 0x05, 0x5987, 162, 564 ), "Game over" },
			{ ( 0x1F, 0x53C8, 5430, 0xE1, 0x1F, 0x754D,  88, 436 ), "End 1" },
			{ ( 0x1F, 0x53C8, 5430, 0xE1, 0x1F, 0x75A6, 124, 436 ), "End 2" },
			{ ( 0x1F, 0x53C8, 5430, 0xE1, 0x1F, 0x74B4, 148, 436 ), "End 3" },
			{ ( 0x1F, 0x53C8, 5430, 0xE1, 0x1F, 0x7627, 120, 436 ), "End 4" },
			{ ( 0x1F, 0x53C8, 5430, 0xE1, 0x1F, 0x73D3, 225, 436 ), "End 5" },
			{ ( 0x1F, 0x53C8, 5430, 0xE1, 0x1F, 0x727B, 344, 948 ), "End 6" }
		};

		public void LoadRom(Rom rom)
		{
			this.rom = rom;
			formLoaded = false;
			changesFlag = ChangeEnum.None;
			selection1.ClearSelection();
			selection2.ClearSelection();

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
			if (!formLoaded && rom != null)
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

			void LoadWorldCombobox()
			{
				WorldComboBox.Items.Clear();
				WorldComboBox.Items.AddRange(worldData.ToArray());
			}
		}

		void LoadWorld()
		{
			var data = worldData[currentWorld].Value;
			Overworld.Dump8x8Tiles(rom, data.BankA, data.TileAddressA, tilesWorld8x8, data.Palette);

			if (data.TileAddressB == 0x7627)
			{
				Overworld.Dump8x8TilesUncompressed(rom, data.BankA, 0x6F01, 35, 144, tilesWorld8x8, data.Palette);
			}
			else if (data.TileAddressB == 0x727B)
			{
				Overworld.Dump8x8TilesUncompressed(rom, data.BankA, 0x6B01, 64, 64, tilesWorld8x8, data.Palette);
			}

			if (timerTicks != 0)
			{
				DumpAnimatedTiles();
			}

			Array.Clear(worldTiles, 0, worldTiles.Length);
			Overworld.LoadTiles(rom, data.BankB, data.TileAddressB, worldTiles);

			if (Overworld.HasEvents(currentWorld))
			{
				eventForm.LoadWorld(rom, currentWorld);
			}

			if (Overworld.HasPaths(currentWorld))
			{
				pathForm.LoadWorld(currentWorld);
			}

			history.ClearUndo();

			UpdateTitle();
			ClearAllTiles();
			pictureBox1.Invalidate();
			pictureBox2.Invalidate();
		}

		void ClearAllTiles()
		{
			for (int y = 0; y < CurrentMapY; y++)
			{
				for (int x = 0; x < CurrentMapX; x++)
				{
					previousWorldTiles[x + y * 32] = -1;
				}
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
				if ((!Overworld.HasPaths(currentWorld) && pathMode) || (!Overworld.HasEvents(currentWorld) && eventMode))
				{
					ChangeEditMode(0);
				}

				eventModeToolStripMenuItem.Enabled = Overworld.HasEvents(currentWorld);
				pathModeToolStripMenuItem.Enabled = Overworld.HasPaths(currentWorld);
				musicToolStripMenuItem.Enabled = Overworld.HasMusic(currentWorld);

				LoadWorld();
				selection1.ClearSelection();
				selection2.ClearSelection();
				SetZoom(zoom);
			}
		}

		public bool SaveChanges()
		{
			string message;
			if ((changesFlag & ChangeEnum.WorldTile) != 0)
			{
				//improve tile compression
				if (!Overworld.IsOverworld(currentWorld))
				{
					CopyTilesOnTheRightSide(worldTiles);
				}

				var worldInfo = worldData[currentWorld].Value;
				if (!Overworld.SaveTiles(rom, worldInfo.BankB, worldInfo.TileAddressB,
								worldTiles.Take(worldInfo.UncompressedSize).ToArray(), worldInfo.MaxLengthB, out message))
				{
					MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				changesFlag &= ~ChangeEnum.WorldTile;
			}

			if ((changesFlag & ChangeEnum.WorldEvent) != 0)
			{
				if (!eventForm.SaveEvents(rom, out message))
				{
					MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				changesFlag &= ~ChangeEnum.WorldEvent;
			}

			if ((changesFlag & ChangeEnum.WorldPath) != 0)
			{
				if (!pathForm.SavePaths(rom, out message))
				{
					MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				changesFlag &= ~ChangeEnum.WorldPath;
			}

			return true;
		}

		void CopyTilesOnTheRightSide(byte[] tileData)
		{
			for (int y = 0; y < tileData.Length / 32; y++)
			{
				var data = tileData[19 + y * 32];
				for (int x = 20; x < 32; x++)
				{
					tileData[x + y * 32] = data;
				}
			}
		}

		void PictureBox1Paint(object sender, PaintEventArgs e)
		{
			if (!DesignMode)
			{
				RenderTiles();

				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(GetTilesWorldBitmap(),
									new Rectangle(0, 0, CurrentMapX * 8 * zoom, CurrentMapY * 8 * zoom),
									new Rectangle(0, 0, CurrentMapX * 8, CurrentMapY * 8),
									GraphicsUnit.Pixel);

				if (eventMode)
				{
					eventForm.DrawEvents(e.Graphics);
				}
				if (pathMode)
				{
					eventForm.DrawEvents(e.Graphics, false);
					pathForm.Draw(e.Graphics);
				}

				selection1.DrawSelection(e.Graphics);
			}

			void RenderTiles()
			{
				for (int y = 0; y < CurrentMapY; y++)
				{
					for (int x = 0; x < CurrentMapX; x++)
					{
						byte tileIndex = worldTiles[x + y * 32];
						int previousTileIndex = previousWorldTiles[x + y * 32];
						if (tileIndex != previousTileIndex)
						{
							previousWorldTiles[x + y * 32] = tileIndex;
							Dump8x8Tile(new Point(x * 8, y * 8), tileIndex, tilesWorld);
						}
					}
				}

				void Dump8x8Tile(Point dest, int tileIndex, DirectBitmap bitmap)
				{
					Point source = new Point((tileIndex % 16) * 8, (tileIndex / 16) * 8);
					for (int y = 0; y < 8; y++)
					{
						Array.Copy(tilesWorld8x8.Bits, source.X + (source.Y + y) * tilesWorld8x8.Width,
								bitmap.Bits, dest.X + (dest.Y + y) * bitmap.Width, 8);
					}
				}
			}

			Bitmap GetTilesWorldBitmap()
			{
				if (currentWorld == 7 && timerTicks != 0)
				{
					for (int y = 0; y < 144; y++)
					{
						int offset = y * 256;
						int scroll = y < 54 ? Overworld.GetScroll(rom, animationIndex + y) : 0;
						ScrollLine(offset, scroll);
					}

					return tilesWorldScroll.Bitmap;
				}

				return tilesWorld.Bitmap;

				void ScrollLine(int offset, int scroll)
				{
					if (scroll > 0)
					{
						Array.Copy(tilesWorld.Bits, offset, tilesWorldScroll.Bits, offset + scroll, 160 - scroll);
						Array.Copy(tilesWorld.Bits, offset + 160 - scroll, tilesWorldScroll.Bits, offset, scroll);
					}
					else if (scroll < 0)
					{
						Array.Copy(tilesWorld.Bits, offset - scroll, tilesWorldScroll.Bits, offset, 160 + scroll);
						Array.Copy(tilesWorld.Bits, offset, tilesWorldScroll.Bits, offset + 160 + scroll, -scroll);
					}
					else
					{
						Array.Copy(tilesWorld.Bits, offset, tilesWorldScroll.Bits, offset, 160);
					}
				}
			}
		}

		void PictureBox2Paint(object sender, PaintEventArgs e)
		{
			if (!DesignMode)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(tilesWorld8x8.Bitmap, 0, 0, 128 * zoom, 128 * zoom);

				selection2.DrawSelection(e.Graphics);
			}
		}

		public void SetZoom(int zoomlevel)
		{
			pictureBox1.Width = CurrentMapX * 8 * zoomlevel;
			pictureBox1.Height = CurrentMapY * 8 * zoomlevel;

			pictureBox2.Width = 128 * zoomlevel;
			pictureBox2.Height = 128 * zoomlevel;

			eventForm.SetZoom(zoomlevel);
			pathForm.SetZoom(zoomlevel);
			selection1.SetZoom(zoomlevel);
			selection2.SetZoom(zoomlevel);

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

			if (DispatchCommandKey())
			{
				return true;
			}

			ProcessCommandKey(this, args);
			if (args.Handled)
			{
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);

			bool DispatchCommandKey()
			{
				if (eventMode && eventForm.ProcessEventKey(keyData))
				{
					return true;
				}

				if (pathMode && pathForm.ProcessPathKey(keyData))
				{
					return true;
				}

				switch (keyData)
				{
					case Keys.Control | Keys.C:
						CopySelection();
						return true;

					case Keys.Control | Keys.V:
						PasteSelection();
						return true;

					case Keys.Control | Keys.X:
						CutSelection();
						return true;

					case Keys.Delete:
						DeleteSelection();
						return true;

					case Keys.Control | Keys.Z:
						Undo();
						return true;

					case Keys.Control | Keys.Y:
						Redo();
						return true;
				}

				if (eventMode || pathMode)
				{
					switch (keyData & Keys.KeyCode)
					{
						case Keys.Home:
						case Keys.End:
						case Keys.PageDown:
						case Keys.PageUp:
						case Keys.Left:
						case Keys.Right:
						case Keys.Up:
						case Keys.Down:
							return true; //prevent dropdown change
					}
				}

				if (DispatchShortcut())
				{
					return true;
				}

				return false;

				#region Selection

				void CopySelection()
				{
					if (!pathMode)
					{
						var selection = selectionMode ? selection1 : selection2;
						selection.CopySelection(CopyTileAt);
						selection.ClearSelection();
					}
				}

				void PasteSelection()
				{
					if (!pathMode && selectionMode)
					{
						if (selection1.HasSelection)
						{
							history.AddChanges(selection1.PasteSelection(PasteTileAt));
							UpdateTitle();
							pictureBox1.Invalidate();
							SetChanges();
						}

						selection1.ClearSelection();
					}

					int PasteTileAt(int x, int y, int data)
					{
						if (x < CurrentMapX && y < CurrentMapY && data != -1)
						{
							return SetTileAt(x, y, data);
						}

						return data; //no changes
					}
				}

				void CutSelection()
				{
					if (!pathMode && selectionMode)
					{
						int tile = GetEmptyTile();
						if (selection1.HasSelection)
						{
							selection1.CopySelection(CopyTileAt);
							history.AddChanges(selection1.DeleteSelection(SetTileAt, GetEmptyTile()));

							UpdateTitle();
							pictureBox1.Invalidate();
							SetChanges();
						}

						selection1.ClearSelection();
					}
				}

				void DeleteSelection()
				{
					if (!pathMode && selectionMode)
					{
						int tile = GetEmptyTile();
						if (selection1.HasSelection)
						{
							history.AddChanges(selection1.DeleteSelection(SetTileAt, GetEmptyTile()));

							UpdateTitle();
							pictureBox1.Invalidate();
							SetChanges();
						}

						selection1.ClearSelection();
					}
				}

				ClipboardData CopyTileAt(int x, int y)
				{
					if (selectionMode)
					{
						if (eventMode)
						{
							int index = eventForm.FindEvent(x, y);
							return new ClipboardData { Index = index, Tile = eventForm.GetEvent(index) };
						}

						return new ClipboardData { Tile = GetTileAt(x, y) };
					}

					return new ClipboardData { Tile = (x + y * 16) };
				}

				int GetEmptyTile()
				{
					if (eventMode)
					{
						return -1;
					}

					return Level.GetEmptyTile(tilesWorld8x8.Bits, 8, 16);
				}

				#endregion

				#region Undo

				void Undo()
				{
					if (!pathMode)
					{
						if (history.Undo(SetTileAt, GetTileAt))
						{
							UpdateTitle();
							pictureBox1.Invalidate();
							SetChanges();
						}
					}
				}

				void Redo()
				{
					if (!pathMode)
					{
						if (history.Redo(SetTileAt, GetTileAt))
						{
							UpdateTitle();
							pictureBox1.Invalidate();
							SetChanges();
						}
					}
				}

				#endregion

				bool DispatchShortcut()
				{
					ToolStripMenuItem toolStrip = menuStrip1.Items.GetAllMenuItems()
						.FirstOrDefault(x => x.ShortcutKeys == keyData || x.ShortcutKeyDisplayString == keyData.ToString());

					if (toolStrip != null)
					{
						toolStrip.PerformClick();
						return true;
					}

					return false;
				}
			}
		}

		void SetChanges()
		{
			if (pathMode)
			{
				changesFlag |= ChangeEnum.WorldPath;
			}
			else if (eventMode)
			{
				changesFlag |= ChangeEnum.WorldEvent;
			}
			else
			{
				changesFlag |= ChangeEnum.WorldTile;
			}

			WorldMapChanged(this, EventArgs.Empty);
		}

		void UpdateTitle()
		{
			string title;
			if (eventMode)
			{
				title = eventForm.GetTitle();
			}
			else if (pathMode)
			{
				title = pathForm.GetTitle();
			}
			else
			{
				title = GetTitle();
			}

			toolStripStatusLabel1.Text = title;
			statusStrip1.Visible = !string.IsNullOrEmpty(title);

			string GetTitle()
			{
				if (lastTile != -1)
				{
					if (lastTileSide)
					{
						int tileIndex = worldTiles[lastTile];
						return $"{lastTile % 32}:{lastTile / 32} - {tileIndex:X2}";
					}
					else
					{
						return $"{lastTile:X2}";
					}
				}

				return string.Empty;
			}
		}

		int CurrentMapX => Overworld.IsOverworld(currentWorld) ? 32 : 20;

		int CurrentMapY => Overworld.IsOverworld(currentWorld) ? 32 : (worldData[currentWorld].Value.UncompressedSize + 31) / 32;

		#region Mouse

		void UpdateSelection(TileEventArgs e, Selection selection)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (e.Status == TileEventStatus.MouseDown)
				{
					selection.StartSelection(e.TileX, e.TileY);
				}
				else if (e.Status == TileEventStatus.MouseMove)
				{
					selection.SetSelection(e.TileX, e.TileY);
				}
			}
		}

		#region PictureBox1

		void PictureBox1MouseEvent(MouseEventArgs e, TileEventStatus status)
		{
			int tilePosX = e.Location.X / 8 / zoom;
			int tilePosY = e.Location.Y / 8 / zoom;
			int tilePos = tilePosX + tilePosY * 32;

			if ((e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) && !pathMode)
			{
				if (tilePos != currentTile)
				{
					currentTile = tilePos;
					PictureBox1TileMouseDown(new TileEventArgs(e.Button, status, tilePosX, tilePosY));
				}
			}

			if (status == TileEventStatus.MouseDown || status == TileEventStatus.MouseMove && !pathMode && !eventMode)
			{
				if (tilePos != lastTile)
				{
					lastTileSide = true;
					lastTile = tilePos;
					UpdateTitle();
				}
			}
		}

		void PictureBox1TileMouseDown(TileEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (e.Status == TileEventStatus.MouseDown)
				{
					changes = new List<SelectionChange>();
				}

				if ((e.Status == TileEventStatus.MouseDown || e.Status == TileEventStatus.MouseMove) && !selection1.HasSelection && SelectedTile != -1)
				{
					UpdateTile(e.TileX, e.TileY, SelectedTile);
				}

				if (e.Status == TileEventStatus.MouseUp)
				{
					history.AddChanges(changes);
				}
			}

			selectionMode = true;
			UpdateSelection(e, selection1);

			if (e.Button == MouseButtons.Right && e.Status == TileEventStatus.MouseDown)
			{
				selection1.ClearSelection();
			}

			if (SelectedTile == -1)
			{
				selection2.ClearSelection();
			}

			void UpdateTile(int x, int y, int newTile)
			{
				int previousTile = SetTileAt(x, y, newTile);
				if (previousTile != newTile)
				{
					changes.Add(new SelectionChange { X = x, Y = y, Data = previousTile });
					pictureBox1.Invalidate(new Rectangle(x * 8 * zoom, y * 8 * zoom, 8 * zoom, 8 * zoom));
					SetChanges();
				}
			}
		}

		void PictureBox1MouseMove(object sender, MouseEventArgs e)
		{
			if (pictureBox1.ClientRectangle.Contains(e.Location))
			{
				PictureBox1MouseEvent(e, TileEventStatus.MouseMove);
			}
		}

		void PictureBox1MouseDown(object sender, MouseEventArgs e)
		{
			currentTile = -1;
			lastTile = -1;
			PictureBox1MouseEvent(e, TileEventStatus.MouseDown);
		}

		void PictureBox1MouseUp(object sender, MouseEventArgs e)
		{
			currentTile = -1;
			lastTile = -1;
			PictureBox1MouseEvent(e, TileEventStatus.MouseUp);
		}

		#endregion

		#region PictureBox2

		void PictureBox2MouseEvent(MouseEventArgs e, TileEventStatus status)
		{
			int tilePosX = e.Location.X / 8 / zoom;
			int tilePosY = e.Location.Y / 8 / zoom;
			int tilePos = tilePosX + tilePosY * 16;

			if ((e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) && !pathMode)
			{
				if (tilePos != currentTile)
				{
					currentTile = tilePos;
					PictureBox2TileMouseDown(new TileEventArgs(e.Button, status, tilePosX, tilePosY));
				}
			}

			if (status == TileEventStatus.MouseDown || status == TileEventStatus.MouseMove)
			{
				if (tilePos != lastTile)
				{
					lastTileSide = false;
					lastTile = tilePos;
					UpdateTitle();
				}
			}
		}

		void PictureBox2TileMouseDown(TileEventArgs e)
		{
			selectionMode = false;
			UpdateSelection(e, selection2);

			selection1.ClearSelection();

			if (e.Button == MouseButtons.Right && e.Status == TileEventStatus.MouseDown)
			{
				selection2.StartSelection(e.TileX, e.TileY);
			}
		}

		void PictureBox2MouseMove(object sender, MouseEventArgs e)
		{
			if (pictureBox2.ClientRectangle.Contains(e.Location))
			{
				PictureBox2MouseEvent(e, TileEventStatus.MouseMove);
			}
		}

		void PictureBox2MouseDown(object sender, MouseEventArgs e)
		{
			currentTile = -1;
			lastTile = -1;
			PictureBox2MouseEvent(e, TileEventStatus.MouseDown);
		}

		#endregion

		#endregion

		#region Selection

		int GetTileAt(int x, int y)
		{
			if (eventMode)
			{
				return eventForm.GetEvent(eventForm.FindEvent(x, y));
			}

			return worldTiles[x + y * 32];
		}

		int SetTileAt(int x, int y, int data)
		{
			int previous = GetTileAt(x, y);
			if (previous != data)
			{
				if (eventMode)
				{
					if (data == 0x7F) //not allowed because 0xFF used as a marker
					{
						return data; //no changes
					}

					if (data == -1)
					{
						eventForm.RemoveEvent(x, y);
					}
					else
					{
						eventForm.AddEvent((byte)data, x, y);
					}
				}
				else
				{
					worldTiles[x + y * 32] = (byte)data;
				}
			}

			return previous;
		}

		#endregion

		#region Animation

		readonly (int Address, int Position)[] animationSea =
		{
			( 0x5B18, 208 ),
			( 0x5B49, 209 ),
			( 0x5B7A, 224 ),
			( 0x5BAB, 225 ),
			( 0x5BDC, 226 ),
			( 0x5C0D, 240 ),
			( 0x5C3E, 241 ),
			( 0x5C6F, 242 ),
			( 0x5CA0, 243 ),
			( 0x5CD1, 244 )
		};

		readonly (int Address, int Position)[] animationOverworld =
		{
			( 0x46F6, 42 )
		};

		readonly (int Address, int Position)[,] animationLava =
		{
			{ ( 0x5222, 218 ), ( 0x5242, 202 ), ( 0x5252, 203 ), ( 0x5232, 219 ) },
			{ ( 0x5232, 218 ), ( 0x5252, 202 ), ( 0x5242, 203 ), ( 0x5222, 219 ) },
		};

		readonly (int Address, int Position)[,] animationWater =
		{
			{ ( 0x518D, 53 ), ( 0x519D, 54 ) },
			{ ( 0x519D, 53 ), ( 0x518D, 54 ) },
		};

		readonly (int Address, int Position)[,] animationEnding =
		{
			{ ( 0x6AC1, 98 ), ( 0x6AD1, 99 ) },
			{ ( 0x6AE1, 98 ), ( 0x6AF1, 99 ) },
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
					case 4:
					case 5:
					case 9:
					case 26:
						if ((timerTicks % 4) == 0)
						{
							animationIndex++;
							DumpAnimatedTiles();
							InvalidateAnimatedTiles();
							pictureBox1.Invalidate();
							pictureBox2.Invalidate();
						}
						break;

					case 7:
						if ((timerTicks % 2) == 0)
						{
							animationIndex++;
							pictureBox1.Invalidate();
						}
						break;
				}
			}

			void InvalidateAnimatedTiles()
			{
				for (int y = 0; y < CurrentMapY; y++)
				{
					for (int x = 0; x < CurrentMapX; x++)
					{
						var previousTile = previousWorldTiles[x + y * 32];
						if (invalidTiles[previousTile])
						{
							previousWorldTiles[x + y * 32] = -1;
						}
					}
				}
			}
		}

		public void ResetTimer()
		{
			timerTicks = 0;
			if (Visible)
			{
				pictureBox1.Invalidate();
				pictureBox2.Invalidate();
			}
		}

		void DumpAnimatedTiles()
		{
			Array.Clear(invalidTiles, 0, invalidTiles.Length);

			switch (currentWorld)
			{
				case 0:
				case 1:
					AnimatedTileA(6, animationSea);
					break;

				case 4:
					AnimateTilesB(8, animationLava);
					break;

				case 5:
					AnimateTilesB(8, animationWater);
					break;

				case 9:
					AnimatedTileA(8, animationOverworld);
					break;

				case 26:
					AnimateTilesB(0x1f, animationEnding);
					break;
			}

			void AnimatedTileA(int count, (int Address, int Position)[] animation)
			{
				for (int i = 0; i < animation.GetLength(0); i++)
				{
					Overworld.DumpAnimatedTilesA(rom, animation[i].Address, animation[i].Position, tilesWorld8x8, animationIndex % count, count);
					invalidTiles[animation[i].Position] = true;
				}
			}

			void AnimateTilesB(int bank, (int Address, int Position)[,] animation)
			{
				int index = animationIndex % animation.GetLength(0);
				for (int i = 0; i < animation.GetLength(1); i++)
				{
					Overworld.DumpAnimatedTilesB(rom, bank, animation[index, i].Address, animation[index, i].Position, tilesWorld8x8);
					invalidTiles[animation[index, i].Position] = true;
				}
			}
		}

		#endregion

		private void TileModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangeEditMode(0);
		}

		private void EventModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Overworld.HasEvents(currentWorld))
			{
				ChangeEditMode(1);
			}
		}

		private void PathModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Overworld.HasPaths(currentWorld))
			{
				ChangeEditMode(2);
			}
		}

		void ChangeEditMode(int mode)
		{
			if (!ignoreEvents && mode != (eventMode ? 1 : (pathMode ? 2 : 0)))
			{
				eventMode = mode == 1;
				pathMode = mode == 2;

				UpdateTitle();
				pictureBox1.Invalidate();

				pictureBox2.Visible = !pathMode;
				UpdateBounds();
				history.ClearUndo();
				selection1.ClearSelection();
				selection2.ClearSelection();

				ignoreEvents = true;
				tileModeToolStripMenuItem.Checked = !eventMode && !pathMode;
				eventModeToolStripMenuItem.Checked = eventMode;
				pathModeToolStripMenuItem.Checked = pathMode;
				ignoreEvents = false;

				transparentPathToolStripMenuItem.Enabled = pathMode;
			}
		}

		private void MusicToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int musicTrack = Overworld.GetMusic(rom, currentWorld);

			string musicTrackTxt = musicTrack == -1 ? string.Empty : (musicTrack + 1).ToString();
			do
			{
				musicTrackTxt = Interaction.InputBox(string.Empty, "Enter music track number (1-8)", musicTrackTxt, -1, -1);
			}
			while (!(musicTrackTxt == string.Empty || (int.TryParse(musicTrackTxt, out musicTrack) && musicTrack >= 1 && musicTrack <= 8)));

			if (musicTrackTxt != string.Empty)
			{
				Overworld.SetMusic(rom, currentWorld, musicTrack - 1);
				WorldMapChanged(this, EventArgs.Empty);
			}
		}

		private void TransparentPathToolStripMenuItem_Click(object sender, EventArgs e)
		{
			pathForm.TransparentPath = transparentPathToolStripMenuItem.Checked;
			pathForm.Invalidate();
		}

		private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog
			{
				Filter = "Tile/Map data (*.chr, *.prg)|*.chr;*.prg"
			};

			var result = openFile.ShowDialog();
			if (result == DialogResult.OK)
			{
				Import(openFile.FileName);
			}
		}

		void Import(string filePath)
		{
			if (Path.GetExtension(filePath) == ".chr")
			{
				ImportCHR();
			}
			else if (Path.GetExtension(filePath) == ".prg")
			{
				ImportPRG();
			}

			void ImportCHR()
			{
				var item = worldData[currentWorld].Value;
				var data = File.ReadAllBytes(filePath);
				var romData = Overworld.Dump8x8Tiles(rom, item.BankA, item.TileAddressA).ToArray();
				Array.Copy(data, 0, romData, 128 * 16, Math.Min(data.Length, 256 * 16));

				if (Overworld.Save8x8Tiles(rom, item.BankA, item.TileAddressA, romData, item.MaxLengthA, out string message))
				{
					Overworld.Dump8x8Tiles(rom, item.BankA, item.TileAddressA, tilesWorld8x8, item.Palette);
					ClearAllTiles();
					WorldMapChanged(this, EventArgs.Empty);

					pictureBox1.Invalidate();
					pictureBox2.Invalidate();
				}
				else
				{
					MessageBox.Show(message, "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			void ImportPRG()
			{
				var item = worldData[currentWorld].Value;
				var data = File.ReadAllBytes(filePath);
				Array.Copy(data, worldTiles, Math.Min(data.Length, item.UncompressedSize));

				SetChanges();
				pictureBox1.Invalidate();
			}
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
			base.OnDragEnter(e);
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			string filePath = files[0];
			if (File.Exists(filePath))
			{
				Import(filePath);
			}

			base.OnDragDrop(e);
		}

		private void ExportTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFile = new SaveFileDialog
			{
				Filter = "Tile data (*.chr)|*.chr"
			};

			var result = saveFile.ShowDialog();
			if (result == DialogResult.OK)
			{
				var item = worldData[currentWorld].Value;
				var data = Overworld.Dump8x8Tiles(rom, item.BankA, item.TileAddressA).Skip(128 * 16).ToArray();
				File.WriteAllBytes(saveFile.FileName, data);
			}
		}

		private void ExportMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var saveFile = new SaveFileDialog
			{
				Filter = "Map data (*.prg)|*.prg"
			};

			var result = saveFile.ShowDialog();
			if (result == DialogResult.OK)
			{
				var item = worldData[currentWorld].Value;
				File.WriteAllBytes(saveFile.FileName, worldTiles.Take(item.UncompressedSize).ToArray());
			}
		}
	}
}
