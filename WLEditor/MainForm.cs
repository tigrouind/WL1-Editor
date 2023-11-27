using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class MainForm : Form
	{
		Rom rom = new Rom();
		readonly ToolboxForm toolboxForm = new ToolboxForm();
		readonly OverworldForm overworldForm = new OverworldForm();
		readonly SectorForm sectorForm = new SectorForm();

		public readonly static string[] LevelNames =
		{
			"SS Teacup 1",
			"Parsley Woods 3",
			"Sherbet Land 2",
			"Stove Canyon 1",
			"Sherbet Land 3",
			"Mt. Teapot 4",
			"Mt. Teapot 1",
			"Rice Beach 1",
			"Sherbet Land 4",
			"Mt. Teapot 6",
			"Mt. Teapot 4 - CRUSHED",
			"SS Teacup 4",
			"Rice Beach 4",
			"Mt. Teapot 3",
			"Rice Beach 3",
			"Rice Beach 2",
			"Mt. Teapot 2",
			"Mt. Teapot 5",
			"Parsley Woods 5",
			"Parsley Woods 4",
			"SS Teacup 5",
			"Stove Canyon 2",
			"Stove Canyon 3",
			"Rice Beach 1 - FLOODED",
			"Sherbet Land 6",
			"Rice Beach 5",
			"Parsley Woods 6",
			"Stove Canyon 5",
			"Stove Canyon 6",
			"Parsley Woods 2",
			"SS Teacup 2",
			"SS Teacup 3",
			"Sherbet Land 5",
			"Sherbet Land 1",
			"Syrup Castle 2",
			"Syrup Castle 3",
			"Rice Beach 3 - FLOODED",
			"Syrup Castle 1",
			"Parsley Woods 1 - FLOODED",
			"Stove Canyon 4",
			"Syrup Castle 4",
			"Rice Beach 6",
			"Parsley Woods 1 - DRAINED"
		};

		int currentCourseId = -1;
		int currentWarp = -1;
		int treasureId = -1;
		bool checkPoint;
		int animatedTileIndex;
		int timerTicks;
		string romFilePath;
		ChangeEnum changeMode;
		int zoom;
		bool ignoreEvents;

		public MainForm()
		{
			InitializeComponent();
			Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

			LevelPanel.MouseWheel += LevelPanelMouseWheel;
			levelPictureBox.TileMouseMove += LevelPictureBoxTileMouseMove;
			levelPictureBox.TileMouseDown += LevelPictureBoxTileMouseDown;
			levelPictureBox.SectorChanged += LevelPictureBoxSectorChanged;
			levelPictureBox.GetSourceSector = x => Sector.GetSourceSector(rom, currentCourseId, x);

			toolboxForm.MouseWheel += LevelPanelMouseWheel;
			toolboxForm.FormClosing += ToolBoxFormClosing;
			toolboxForm.ProcessCommandKey += ProcessSubFormCommand;

			overworldForm.FormClosing += OverworldFormClosing;
			overworldForm.ProcessCommandKey += ProcessSubFormCommand;
			overworldForm.MouseWheel += LevelPanelMouseWheel;
			overworldForm.WorldMapChanged += WorldMapChanged;

			sectorForm.FormClosing += SectorFormClosing;
			sectorForm.ProcessCommandKey += ProcessSubFormCommand;
			sectorForm.SectorChanged += SectorFormSectorChanged;
			sectorForm.SectorLoad += SectorFormSectorLoad;

			SetZoomLevel(2);

			void ProcessSubFormCommand(object sender, KeyEventArgs e)
			{
				if ((sender != overworldForm || e.Modifiers == Keys.Shift || e.Modifiers == Keys.Control) && DispatchShortcut(e.KeyData))
				{
					e.Handled = true;
				}
				else if (sender != overworldForm)
				{
					DispatchCommandKey(e.KeyData);
				}
			}

			#region Subforms

			void SectorFormSectorChanged(object sender, EventArgs e)
			{
				currentWarp = Sector.SearchWarp(rom, currentCourseId, levelPictureBox.CurrentSector, treasureId);
				LoadLevel();
				SetChanges(ChangeEnum.Sectors);
			}

			void SectorFormSectorLoad(object sender, (int Sector, bool Checkpoint, int TreasureId) e)
			{
				treasureId = e.TreasureId;
				levelPictureBox.CurrentSector = e.Sector;
				checkPoint = e.Checkpoint;

				currentWarp = Sector.SearchWarp(rom, currentCourseId, levelPictureBox.CurrentSector, treasureId);
				LoadLevel();
				sectorForm.LoadSector(currentCourseId, levelPictureBox.CurrentSector, treasureId, checkPoint);
				levelPictureBox.ClearSelection();
			}

			void WorldMapChanged(object sender, EventArgs e)
			{
				SetChanges(ChangeEnum.WorldMap);
			}

			void ToolBoxFormClosing(object sender, EventArgs e)
			{
				toolboxToolStripMenuItem.Checked = false;
				Focus(); //prevent main window disapearing
			}

			void OverworldFormClosing(object sender, EventArgs e)
			{
				overworldToolStripMenuItem.Checked = false;
				Focus(); //prevent main window disapearing
			}

			void SectorFormClosing(object sender, EventArgs e)
			{
				sectorsToolStripMenuItem.Checked = false;
				Focus(); //prevent main window disapearing
			}

			#endregion

			#region Zoom

			void LevelPanelMouseWheel(object sender, MouseEventArgs e)
			{
				if (ModifierKeys == Keys.Control)
				{
					if (e.Delta > 0 && zoom < 4)
					{
						SetZoomLevel(zoom + 1);
					}
					else if (e.Delta < 0 && zoom > 1)
					{
						SetZoomLevel(zoom - 1);
					}
				}
			}

			#endregion

			#region LevelPictureBox

			void LevelPictureBoxSectorChanged(object sender, EventArgs e)
			{
				treasureId = -1;
				checkPoint = false;

				currentWarp = Sector.SearchWarp(rom, currentCourseId, levelPictureBox.CurrentSector, treasureId);
				LoadLevel();
				sectorForm.LoadSector(currentCourseId, levelPictureBox.CurrentSector, treasureId, checkPoint);
				levelPictureBox.ClearSelection();
			}

			void LevelPictureBoxTileMouseMove(object sender, TileEventArgs e)
			{
				byte tileIndex = Level.LevelData[e.TileX + e.TileY * 256 + 0x1000];
				var tileInfo = Level.GetTileInfo(tileIndex);
				toolStripStatusLabel1.Text = $"{e.TileX}:{e.TileY} - {tileIndex:X2} {tileInfo.Text}";
			}

			void LevelPictureBoxTileMouseDown(object sender, TileEventArgs e)
			{
				if (toolboxForm.Visible)
				{
					if (e.Status == TileEventStatus.MouseDown)
					{
						levelPictureBox.StartChanges();
					}

					if ((e.Status == TileEventStatus.MouseMove || e.Status == TileEventStatus.MouseDown) && !levelPictureBox.HasSelection)
					{
						int tileIndex = e.TileX + e.TileY * 256;
						var control = toolboxForm.SelectedPanel;
						int currentTile = toolboxForm.Tiles16x16.CurrentTile;
						int currentObject = toolboxForm.Objects.CurrentObject;

						if (e.Button == MouseButtons.Right)
						{
							if (control == toolboxForm.Tiles16x16 && currentTile != -1)
							{
								UpdateTile(tileIndex, currentTile);
							}
							else if (levelPictureBox.ShowObjects && control == toolboxForm.Objects)
							{
								UpdateObject(tileIndex, currentObject);
							}
						}
					}

					if (e.Status == TileEventStatus.MouseUp)
					{
						levelPictureBox.CommitChanges();
					}
				}

				if (e.Button == MouseButtons.Left)
				{
					if (e.Status == TileEventStatus.MouseDown)
					{
						levelPictureBox.StartSelection(e.TileX, e.TileY);
					}
					else if (e.Status == TileEventStatus.MouseMove)
					{
						levelPictureBox.SetSelection(e.TileX, e.TileY);
					}
				}
				else
				{
					levelPictureBox.ClearSelection();
				}

				void UpdateTile(int tileIndex, int currentTile)
				{
					int previousTile = Level.LevelData[tileIndex + 0x1000];
					if (previousTile != currentTile)
					{
						levelPictureBox.AddChange(tileIndex % 256, tileIndex / 256);
						Level.LevelData[tileIndex + 0x1000] = (byte)currentTile;
						levelPictureBox.InvalidateTile(tileIndex);
						SetChanges(ChangeEnum.Blocks);
					}
				}

				void UpdateObject(int tileIndex, int currentObject)
				{
					int previousObject = Level.ObjectsData[tileIndex];
					if (previousObject != currentObject)
					{
						levelPictureBox.AddChange(tileIndex % 256, tileIndex / 256);
						Level.ObjectsData[tileIndex] = (byte)currentObject;
						levelPictureBox.InvalidateObject(tileIndex, currentObject, previousObject);
						SetChanges(ChangeEnum.Blocks);
					}
				}
			}

			#endregion
		}

		#region Load

		void LoadLevel()
		{
			if (rom.IsLoaded && levelComboBox.SelectedItem != null)
			{
				Level.DumpLevel(rom, currentCourseId, currentWarp, checkPoint, animatedTileIndex, false);

				levelPictureBox.ClearTileCache();
				levelPictureBox.Invalidate();

				toolboxForm.Invalidate(true);
			}
		}

		void LevelComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents && levelComboBox.SelectedItem != null)
			{
				if (SaveChanges(false))
				{
					treasureId = -1;
					currentWarp = -1;
					checkPoint = false;
					levelPictureBox.CurrentSector = -1;
					var item = (ComboboxItem<int>)levelComboBox.SelectedItem;
					currentCourseId = item.Value;
					Level.DumpBlocks(rom, currentCourseId);
					Level.SwitchType = Level.GetSwitchType();
					Level.SwitchMode = 0;
					ignoreEvents = true;
					switchBlockToolStripMenuItem.Checked = false;
					ignoreEvents = false;
					LoadLevel();
					sectorForm.LoadSector(currentCourseId, levelPictureBox.CurrentSector, treasureId, checkPoint);
					levelPictureBox.ClearSelection();
					levelPictureBox.ClearUndo();
				}
				else
				{
					//restore previous item
					ignoreEvents = true;
					levelComboBox.SelectedIndex = levelComboBox.Items.Cast<ComboboxItem<int>>().FindIndex(x => x.Value == currentCourseId);
					ignoreEvents = false;
				}
			}
		}

		void LoadToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog
			{
				Filter = "GB ROM Image (*.gb)|*.gb"
			};

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				Rom newRom = new Rom();
				newRom.Load(openFileDialog1.FileName);
				if (newRom.Title == "SUPERMARIOLAND3")
				{
					if (!newRom.CheckCRC())
					{
						MessageBox.Show("ROM checksum failed", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}

					rom = newRom;
					LoadLevelCombobox();
					Sprite.DumpBonusSprites(rom);
					Sprite.DumpPlayerSprite(rom);
					sectorForm.LoadRom(rom);
					overworldForm.LoadRom(rom);
					romFilePath = openFileDialog1.FileName;

					SetChanges(ChangeEnum.None);
					ignoreEvents = true;
					levelComboBox.SelectedIndex = 0;
					ignoreEvents = false;
					LevelComboBoxSelectedIndexChanged(sender, e);

					toolboxToolStripMenuItem.Enabled = true;
					overworldToolStripMenuItem.Enabled = true;
					sectorsToolStripMenuItem.Enabled = true;
					saveAsToolStripMenuItem.Enabled = true;
					levelComboBox.Visible = true;
					LevelPanel.Visible = true;
				}
				else
				{
					MessageBox.Show("Please select a valid WarioLand 1 rom", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			void LoadLevelCombobox()
			{
				//init combobox
				levelComboBox.SelectedItem = null;
				levelComboBox.Items.Clear();

				var items = Level.GetCourseIds(rom)
					.OrderBy(x => x.CourseNo)
					.Select(x => new ComboboxItem<int>(x.CourseId, $"{x.CourseNo:D2} {LevelNames[x.CourseId]}"))
					.ToList();

				levelComboBox.Items.AddRange(items.ToArray());
			}
		}

		#endregion

		void RegionsToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.ShowSectors = regionsToolStripMenuItem.Checked;
			levelPictureBox.Invalidate();
		}

		void ObjectsToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.ShowObjects = objectsToolStripMenuItem.Checked;
			levelPictureBox.Invalidate();
		}

		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			var version = Assembly.GetEntryAssembly().GetName().Version;
			var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(TimeSpan.TicksPerDay * version.Build));

			MessageBox.Show($"{Text} v{version.Major}.{version.Minor}\r\nDate : {buildDateTime:d}\r\nContact me : tigrou.ind@gmail.com");
		}

		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Application.Exit();
		}

		void CollidersToolStripMenuItemClick(object sender, EventArgs e)
		{
			bool value = collidersToolStripMenuItem.Checked;
			Level.ShowColliders = value;
			LoadLevel();
		}

		void CollectiblesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool value = collectiblesToolStripMenuItem.Checked;
			Level.ShowCollectibles = value;
			LoadLevel();
		}

		private void SwitchBlockToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				int switchType = Level.GetSwitchType();
				if (switchType == 0 && !switchBlockToolStripMenuItem.Checked)
				{
					SystemSounds.Beep.Play();
				}

				switchBlockToolStripMenuItem.Checked = switchType != 0 && !switchBlockToolStripMenuItem.Checked;
				int switchMode = switchBlockToolStripMenuItem.Checked ? switchType : 0;

				if (Level.SwitchMode != switchMode || Level.SwitchType != switchType)
				{
					Level.SwitchMode = switchMode;
					Level.SwitchType = switchType;
					LoadLevel();
				}
			}
		}

		#region Save

		bool SaveChanges(bool saveFile)
		{
			if (rom.IsLoaded)
			{
				if ((changeMode & ChangeEnum.Blocks) != 0)
				{
					if (!Level.SaveChanges(rom, currentCourseId, out string message))
					{
						MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}

				if (saveFile && !overworldForm.SaveChanges())
				{
					return false;
				}

				if (saveFile)
				{
					rom.FixCRC();
					try
					{
						rom.Save(romFilePath);
					}
					catch (UnauthorizedAccessException)
					{
						MessageBox.Show("The ROM file is marked as read-only and cannot be overwritten.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}

					SetChanges(ChangeEnum.None);
				}
			}

			return true;
		}

		void SetChanges(ChangeEnum mode)
		{
			if (mode == ChangeEnum.None)
			{
				changeMode = ChangeEnum.None;
				saveToolStripMenuItem.Enabled = false;
			}
			else
			{
				changeMode |= mode;
				saveToolStripMenuItem.Enabled = true;
			}
		}

		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveChanges(true);
		}

		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !AskForSavingChanges();

			bool AskForSavingChanges()
			{
				if (changeMode != ChangeEnum.None)
				{
					DialogResult result = MessageBox.Show("Save pending changes ?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					switch (result)
					{
						case DialogResult.Yes:
							return SaveChanges(true);

						case DialogResult.No:
							return true;

						case DialogResult.Cancel:
							return false;

						default:
							throw new NotImplementedException();
					}
				}

				return true;
			}

		}

		void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "GB ROM Image (*.gb)|*.gb"
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string previousRomFilePath = romFilePath;
				romFilePath = saveFileDialog.FileName;
				if (!SaveChanges(true))
				{
					romFilePath = previousRomFilePath;
				}
			}
		}

		#endregion

		#region Zoom

		void SetZoomLevel(int zoomLevel)
		{
			zoom = zoomLevel;
			zoom100ToolStripMenuItem.Checked = zoomLevel == 1;
			zoom200ToolStripMenuItem.Checked = zoomLevel == 2;
			zoom300ToolStripMenuItem.Checked = zoomLevel == 3;
			zoom400ToolStripMenuItem.Checked = zoomLevel == 4;
			zoomOutToolStripMenuItem.Enabled = zoomLevel > 1;
			zoomInToolStripMenuItem.Enabled = zoomLevel < 4;

			levelPictureBox.SetZoom(zoomLevel);
			toolboxForm.SetZoom(zoomLevel);
			overworldForm.SetZoom(zoomLevel);
			sectorForm.SetZoom(zoomLevel);
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

		void ZoomOutToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetZoomLevel(zoom - 1);
		}

		void ZoomInToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetZoomLevel(zoom + 1);
		}

		#endregion

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (DispatchCommandKey(keyData))
			{
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		bool DispatchCommandKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Control | Keys.Add:
				case Keys.Control | Keys.Oemplus:
					zoomInToolStripMenuItem.PerformClick();
					return true;

				case Keys.Control | Keys.Subtract:
				case Keys.Control | Keys.OemMinus:
					zoomOutToolStripMenuItem.PerformClick();
					return true;

				case Keys.Control | Keys.C:
					levelPictureBox.CopySelection();
					levelPictureBox.ClearSelection();
					return true;

				case Keys.Control | Keys.V:
					if (levelPictureBox.PasteSelection())
					{
						SetChanges(ChangeEnum.Blocks);
					}

					levelPictureBox.ClearSelection();
					return true;

				case Keys.Control | Keys.X:
					if (levelPictureBox.CutSelection())
					{
						SetChanges(ChangeEnum.Blocks);
					}

					levelPictureBox.ClearSelection();
					return true;

				case Keys.Delete:
					if (levelPictureBox.DeleteSelection())
					{
						SetChanges(ChangeEnum.Blocks);
					}

					levelPictureBox.ClearSelection();
					return true;

				case Keys.Control | Keys.Z:
					if (levelPictureBox.Undo())
					{
						SetChanges(ChangeEnum.Blocks);
					}
					return true;

				case Keys.Control | Keys.Y:
					if (levelPictureBox.Redo())
					{
						SetChanges(ChangeEnum.Blocks);
					}
					return true;
			}

			return DispatchShortcut(keyData);
		}

		bool DispatchShortcut(Keys keyData)
		{
			ToolStripMenuItem toolStrip = menuStrip1.Items.GetAllMenuItems()
				.FirstOrDefault(x => x.ShortcutKeys == keyData);

			if (toolStrip != null)
			{
				toolStrip.PerformClick();
				return true;
			}

			return false;
		}

		private void ScrollBoundaryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			levelPictureBox.ScrollLines = (levelPictureBox.ScrollLines + 1) % 5;
			levelPictureBox.Invalidate();
		}

		#region Subforms

		void ToolboxToolStripMenuItemClick(object sender, EventArgs e)
		{
			ToggleForm(toolboxForm, toolboxToolStripMenuItem.Checked);
		}

		void OverworldToolStripMenuItemClick(object sender, EventArgs e)
		{
			ToggleForm(overworldForm, overworldToolStripMenuItem.Checked);
		}

		void SectorsToolStripMenuItemClick(object sender, EventArgs e)
		{
			ToggleForm(sectorForm, sectorsToolStripMenuItem.Checked);
		}

		void ToggleForm(Form form, bool visible)
		{
			if (visible)
			{
				if (!form.Visible)
				{
					form.Show(this);
				}
			}
			else
			{
				if (form.Visible)
				{
					form.Close();
				}
			}
		}

		#endregion

		#region Animation

		void TimerTick(object sender, EventArgs e)
		{
			if (rom.IsLoaded)
			{
				timerTicks++;
				if (levelComboBox.SelectedItem != null && Level.AnimatedTilesMask != 0)
				{
					if ((timerTicks & (Level.AnimatedTilesMask >> 2)) == 0)
					{
						animatedTileIndex = (animatedTileIndex + 1) % 4;
						RefreshAnimatedTiles();
					}
				}

				overworldForm.TimerTick();
			}

			void RefreshAnimatedTiles()
			{
				Level.DumpLevel(rom, currentCourseId, currentWarp, checkPoint, animatedTileIndex, true);

				//redraw
				levelPictureBox.InvalidateAnimatedTiles();
				if (toolboxForm.Visible)
				{
					var control = toolboxForm.SelectedPanel;
					if (control == toolboxForm.Tiles16x16)
					{
						toolboxForm.Tiles16x16.Invalidate();
					}
					else if (control == toolboxForm.Tiles8x8)
					{
						toolboxForm.Tiles8x8.Invalidate();
					}
				}
			}
		}

		void AnimationToolStripMenuItemClick(object sender, EventArgs e)
		{
			timer.Enabled = animationToolStripMenuItem.Checked;
			overworldForm.ResetTimer();
		}

		#endregion
	}
}