using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
		int switchMode, switchType;
		int animatedTileIndex;
		int timerTicks;
		string romFilePath;
		bool hasChanges;
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

			toolboxForm.MouseWheel += LevelPanelMouseWheel;
			toolboxForm.FormClosing += ToolBoxFormClosing;
			toolboxForm.ProcessCommandKey += ProcessSubFormCommand;

			overworldForm.FormClosing += OverworldFormClosing;
			overworldForm.ProcessCommandKey += ProcessSubFormCommand;
			overworldForm.MouseWheel += LevelPanelMouseWheel;
			overworldForm.WorldMapChanged += WorldMapChanged;

			sectorForm.FormClosing += SectorFormClosing;
			sectorForm.ProcessCommandKey += ProcessSubFormCommand;
			sectorForm.SectorChanged += SectorChanged;

			SetZoomLevel(2);

			void ProcessSubFormCommand(object sender, KeyEventArgs e)
			{
				if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12 && DispatchCommandKey(e.KeyCode))
				{
					e.Handled = true;
				}
			}

			#region Subforms

			void SectorChanged(object sender, EventArgs e)
			{
				currentWarp = Sector.SearchWarp(rom, currentCourseId, levelPictureBox.CurrentSector);
				LoadLevel(false);
				SetChanges(true);
			}

			void WorldMapChanged(object sender, EventArgs e)
			{
				SetChanges(true);
			}

			void ToolBoxFormClosing(object sender, EventArgs e)
			{
				toolboxToolStripMenuItem.Checked = false;
			}

			void OverworldFormClosing(object sender, EventArgs e)
			{
				overworldToolStripMenuItem.Checked = false;
			}

			void SectorFormClosing(object sender, EventArgs e)
			{
				sectorsToolStripMenuItem.Checked = false;
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
				int warpTarget = Sector.SearchWarp(rom, currentCourseId, levelPictureBox.CurrentSector);
				if (warpTarget != currentWarp)
				{
					currentWarp = warpTarget;
					LoadLevel(false);
				}
				else
				{
					levelPictureBox.Invalidate();
				}
				sectorForm.LoadSector(currentCourseId, levelPictureBox.CurrentSector);
				levelPictureBox.ClearSelection();
			}

			void LevelPictureBoxTileMouseMove(object sender, TileEventArgs e)
			{
				int data = 0;
				if (levelPictureBox.ShowObjects)
				{
					data = Level.ObjectsData[e.TileX + e.TileY * 256];
				}

				if (data == 0)
				{
					data = Level.LevelData[e.TileX + e.TileY * 256 + 0x1000];
				}

				int tileIndex = Level.LevelData[e.TileX + e.TileY * 256 + 0x1000];
				var tileInfo = Level.GetTileInfo(tileIndex, switchType);
				toolStripStatusLabel1.Text = string.Format($"{e.TileX}:{e.TileY} - {data:X2} {tileInfo.Text}");
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
						int selectedPanelIndex = toolboxForm.SelectedPanelIndex;
						int currentTile = toolboxForm.Tiles16x16.CurrentTile;
						int currentObject = toolboxForm.Objects.CurrentObject;

						if (e.Button == MouseButtons.Right)
						{
							if (selectedPanelIndex == 0)
							{
								UpdateTile(tileIndex, currentTile);
							}
							else if (levelPictureBox.ShowObjects && selectedPanelIndex == 2)
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
						SetChanges(true);
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
						SetChanges(true);
					}
				}
			}

			#endregion
		}

		#region Load

		void LoadLevel(bool reloadAll)
		{
			if (rom.IsLoaded && levelComboBox.SelectedItem != null)
			{
				Level.DumpLevel(rom, currentCourseId, currentWarp, reloadAll, switchMode, animatedTileIndex, false);

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
					currentWarp = -1;
					levelPictureBox.CurrentSector = -1;
					var item = (ComboboxItem<int>)levelComboBox.SelectedItem;
					currentCourseId = item.Value;
					SetSwitchMode(0);
					LoadLevel(true);
					SetSwitchType(Level.GetSwitchType());
					sectorForm.LoadSector(currentCourseId, -1);
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

					SetChanges(false);
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
					.Select(x => new ComboboxItem<int>(x.CourseId, string.Format($"{x.CourseNo:D2} {LevelNames[x.CourseId]}")))
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
			levelPictureBox.RaiseTileMoveEvent(); //refresh status bar
			levelPictureBox.Invalidate();
		}

		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			MessageBox.Show(Text + " v0.82\r\nDate : 17.09.2023\r\nContact me : tigrou.ind@gmail.com");
		}

		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Application.Exit();
		}

		void CollidersToolStripMenuItemClick(object sender, EventArgs e)
		{
			bool showColliders = collidersToolStripMenuItem.Checked;
			levelPictureBox.ShowColliders = showColliders;
			levelPictureBox.ClearTileCache();
			levelPictureBox.Invalidate();
			toolboxForm.Tiles16x16.ShowColliders = showColliders;
			toolboxForm.Invalidate(true);
		}

		bool SetSwitchType(int value)
		{
			if (switchType != value)
			{
				switchType = value;
				levelPictureBox.SwitchType = value;
				toolboxForm.Tiles16x16.SwitchType = value;
				return true;
			}

			return false;
		}

		bool SetSwitchMode(int value)
		{
			if (switchMode != value)
			{
				switchMode = value;
				levelPictureBox.SwitchMode = value;
				toolboxForm.Tiles16x16.SwitchMode = value;
				return true;
			}

			return false;
		}

		#region Save

		bool SaveChanges(bool saveFile)
		{
			if (rom.IsLoaded)
			{
				if (hasChanges)
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
					rom.Save(romFilePath);
					SetChanges(false);
				}
			}

			return true;
		}

		void SetChanges(bool hasChanges)
		{
			this.hasChanges = hasChanges;
			saveToolStripMenuItem.Enabled = hasChanges;
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
				if (hasChanges)
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

				case Keys.S:
					levelPictureBox.ScrollLines = (levelPictureBox.ScrollLines + 1) % 5;
					levelPictureBox.Invalidate();
					break;

				case Keys.B:
					int typeOfSwitch = Level.GetSwitchType();
					if (SetSwitchMode(GetNextSwitchMode(switchMode, typeOfSwitch)) | SetSwitchType(typeOfSwitch))
					{
						toolboxForm.Invalidate(true);
						LoadLevel(false);
					}
					return true;

				case Keys.Control | Keys.C:
					levelPictureBox.CopySelection();
					levelPictureBox.ClearSelection();
					return true;

				case Keys.Control | Keys.V:
					if (levelPictureBox.PasteSelection())
					{
						SetChanges(true);
					}

					levelPictureBox.ClearSelection();
					return true;

				case Keys.Control | Keys.X:
					if (levelPictureBox.CutSelection())
					{
						SetChanges(true);
					}

					levelPictureBox.ClearSelection();
					return true;

				case Keys.Delete:
					if (levelPictureBox.DeleteSelection())
					{
						SetChanges(true);
					}

					levelPictureBox.ClearSelection();
					return true;

				case Keys.Control | Keys.Z:
					if (levelPictureBox.Undo())
					{
						SetChanges(true);
					}
					return true;

				case Keys.Control | Keys.Y:
					if (levelPictureBox.Redo())
					{
						SetChanges(true);
					}
					return true;
			}

			ToolStripMenuItem toolStrip = menuStrip1.Items.GetAllMenuItems()
				.FirstOrDefault(x => x.ShortcutKeys == keyData);

			if (toolStrip != null)
			{
				toolStrip.PerformClick();
				return true;
			}

			return false;

			int GetNextSwitchMode(int value, int typeOfSwitch)
			{
				int[] flags = { 0, 1, 2, 4 };
				int nextMode = value;

				do
				{
					nextMode = (nextMode + 1) % 4;
				}
				while (nextMode != 0 && (flags[nextMode] & typeOfSwitch) == 0);
				return nextMode;
			}
		}

		#region Subforms

		void ToolboxToolStripMenuItemCheckedChanged(object sender, EventArgs e)
		{
			ToggleForm(toolboxForm, toolboxToolStripMenuItem.Checked);
		}

		void OverworldToolStripMenuItemCheckedChanged(object sender, EventArgs e)
		{
			ToggleForm(overworldForm, overworldToolStripMenuItem.Checked);
		}

		void SectorsToolStripMenuItemCheckedChanged(object sender, EventArgs e)
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
					form.Hide();
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
				Level.DumpLevel(rom, currentCourseId, currentWarp, false, switchMode, animatedTileIndex, true);

				//redraw
				levelPictureBox.InvalidateAnimatedTiles();
				if (toolboxForm.Visible)
				{
					switch (toolboxForm.SelectedPanelIndex)
					{
						case 0: //16x16 tiles
						case 1: //8x8 tiles
							toolboxForm.Invalidate(true);
							break;
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