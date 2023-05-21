using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class MainForm : Form
	{
		Rom rom = new Rom();
		ToolboxForm toolboxForm = new ToolboxForm();
		OverworldForm overworldForm = new OverworldForm();
		SectorForm sectorForm = new SectorForm();

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
		int switchMode;
		int animatedTileIndex;
		int timerTicks;
		string romFilePath;
		bool hasChanges;
		int zoom;
		bool ignoreEvents;

		public MainForm()
		{
			InitializeComponent();
			LevelPanel.MouseWheel += LevelPanelMouseWheel;
			levelPictureBox.TileMouseMove += LevelPictureBoxTileMouseMove;
			levelPictureBox.TileMouseDown += LevelPictureBoxTileMouseDown;
			levelPictureBox.SectorChanged += LevelPictureBoxSectorChanged;

			toolboxForm.MouseWheel += LevelPanelMouseWheel;
			toolboxForm.FormClosing += ToolBoxFormClosing;
			toolboxForm.Tile16x16MouseMove += ToolBoxTile16x16MouseMove;
			toolboxForm.ObjectTileMouseMove += ToolBoxObjectMouseMove;
			toolboxForm.ProcessCommandKey += ToolBoxProcessCommandKey;

			overworldForm.FormClosing += OverworldFormClosing;
			overworldForm.ProcessCommandKey += ToolBoxProcessCommandKey;
			overworldForm.MouseWheel += LevelPanelMouseWheel;
			overworldForm.WorldMapChanged += WorldMapChanged;

			sectorForm.FormClosing += SectorFormClosing;
			sectorForm.ProcessCommandKey += ToolBoxProcessCommandKey;
			sectorForm.SectorChanged += SectorChanged;

			SetZoomLevel(2);
		}

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
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "GB ROM Image (*.gb)|*.gb";

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
		}

		void LoadLevelCombobox()
		{
			//init combobox
			levelComboBox.SelectedItem = null;
			levelComboBox.Items.Clear();

			var items = new List<ComboboxItem<int>>();
			foreach (var levelInfo in Level.GetCourseIds(rom).OrderBy(x => x.Value))
			{
				var item = new ComboboxItem<int>(levelInfo.Key, string.Format("{0:D2} {1}", levelInfo.Value, LevelNames[levelInfo.Key]));
				items.Add(item);
			}

			levelComboBox.Items.AddRange(items.ToArray());
		}

		void RegionsToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.ShowSectors = regionsToolStripMenuItem.Checked;
			levelPictureBox.Invalidate();
		}

		void ObjectsToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.ShowObjects = objectsToolStripMenuItem.Checked;
			levelPictureBox.RaiseTileMoveEvent();
			levelPictureBox.Invalidate();
		}

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
			toolboxForm.ShowColliders = showColliders;
			toolboxForm.Invalidate(true);
		}

		void SetSwitchMode(int value)
		{
			if (switchMode != value)
			{
				switchMode = value;
				levelPictureBox.SwitchMode = value;
				toolboxForm.SwitchMode = value;
				toolboxForm.Invalidate(true);

				LoadLevel(false);
			}
		}

		int GetNextSwitchMode(int value)
		{
			int typeOfSwitch = Level.GetTypeOfSwitch();
			int[] flags = { 0, 1, 2, 4 };
			int nextMode = value;

			do
			{
				nextMode = (nextMode + 1) % 4;
			}
			while (nextMode != 0 && (flags[nextMode] & typeOfSwitch) == 0);
			return nextMode;
		}

		bool SaveChanges(bool saveFile)
		{
			if (rom.IsLoaded)
			{
				if (hasChanges)
				{
					string message;
					if (!Level.SaveChanges(rom, currentCourseId, out message))
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
		}

		void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "GB ROM Image (*.gb)|*.gb";

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

		void ToolBoxProcessCommandKey(object sender, KeyEventArgs e)
		{
			if (DispatchCommandKey(e.KeyData))
			{
				e.Handled = true;
			}
		}

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
					levelPictureBox.ScrollLines = (levelPictureBox.ScrollLines + 1) % 4;
					levelPictureBox.Invalidate();
					break;

				case Keys.B:
					SetSwitchMode(GetNextSwitchMode(switchMode));
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

			ToolStripMenuItem toolStrip = GetAllMenuItems(menuStrip1.Items)
				.FirstOrDefault(x => x.ShortcutKeys == keyData);

			if (toolStrip != null)
			{
				toolStrip.PerformClick();
				return true;
			}

			return false;
		}

		IEnumerable<ToolStripMenuItem> GetAllMenuItems(ToolStripItemCollection items)
		{
			foreach (ToolStripItem item in items)
			{
				ToolStripMenuItem toolStrip = item as ToolStripMenuItem;
				if (toolStrip != null)
				{
					yield return toolStrip;
				}

				ToolStripDropDownItem toolStripDropDown = item as ToolStripDropDownItem;
				if (toolStripDropDown != null)
				{
					foreach (var child in GetAllMenuItems(toolStripDropDown.DropDownItems))
					{
						yield return child;
					}
				}
			}
		}

		void ZoomOutToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetZoomLevel(zoom - 1);
		}

		void ZoomInToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetZoomLevel(zoom + 1);
		}

		void LevelPanelMouseWheel(object sender, MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Control)
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

		void ToolboxToolStripMenuItemCheckedChanged(object sender, EventArgs e)
		{
			ToggleForm(toolboxForm, toolboxToolStripMenuItem.Checked);
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

		#region MouseDown

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
					int currentTile = toolboxForm.CurrentTile;
					int currentObject = toolboxForm.CurrentObject;

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

		#endregion

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

		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			MessageBox.Show(Text + " v0.80\r\nDate : 24.02.2023\r\nContact me : tigrou.ind@gmail.com");
		}

		void AnimationToolStripMenuItemClick(object sender, EventArgs e)
		{
			timer.Enabled = animationToolStripMenuItem.Checked;
			overworldForm.ResetTimer();
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

		void OverworldToolStripMenuItemCheckedChanged(object sender, EventArgs e)
		{
			ToggleForm(overworldForm, overworldToolStripMenuItem.Checked);
		}

		void SectorsToolStripMenuItemCheckedChanged(object sender, EventArgs e)
		{
			ToggleForm(sectorForm, sectorsToolStripMenuItem.Checked);
		}

		#region MouseMove

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

			toolStripStatusLabel1.Text = string.Format("{0:X2} {1}:{2}", data, e.TileX, e.TileY);
		}

		void ToolBoxTile16x16MouseMove(object sender, TileEventArgs e)
		{
			toolStripStatusLabel1.Text = string.Format("{0:X2}", e.TileX + e.TileY * 8);
		}

		void ToolBoxObjectMouseMove(object sender, TileEventArgs e)
		{
			toolStripStatusLabel1.Text = string.Format("{0:X2}", e.TileX + e.TileY * 4);
		}

		#endregion
	}
}