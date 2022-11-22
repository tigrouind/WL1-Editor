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

		static string[] levelNames =
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
			"Mt. Teapot 7",
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
			levelPictureBox.MouseWheel += MainFormMouseWheel;
			toolboxForm.MouseWheel += MainFormMouseWheel;
			toolboxForm.FormClosing += ToolBoxFormClosing;	
			toolboxForm.ProcessCommandKey += ToolBoxProcessCommandKey;
			SetZoomLevel(2);
		}

		void LoadLevel(bool reloadAll)
		{
			if(rom.IsLoaded && levelComboBox.SelectedItem != null)
			{				
				Level.DumpLevel(rom, currentCourseId, currentWarp, reloadAll, switchMode, animatedTileIndex, false);
				
				levelPictureBox.ClearTileCache();
				levelPictureBox.Invalidate();	
				
				//make sure current object can still be selected, otherwise select first available previous object
				int currentObject = toolboxForm.CurrentObject;
				while(currentObject >= 1 && currentObject <= 6 && !Level.EnemiesAvailable[currentObject - 1])
				{
					currentObject--;
				}
				toolboxForm.CurrentObject = currentObject;									
				toolboxForm.Invalidate(true);		
			}
		}			

		void LevelComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if(!ignoreEvents && levelComboBox.SelectedItem != null)
			{
				if(SaveChanges(false))
				{
					currentWarp = -1;
					levelPictureBox.CurrentSector = -1;
					ComboboxItem item = (ComboboxItem)levelComboBox.SelectedItem;
					currentCourseId = (int)item.Value;
					LoadLevel(true);
				}
				else
				{
					//restore previous item
					ignoreEvents = true;
					levelComboBox.SelectedItem = levelComboBox.Items.Cast<ComboboxItem>().First(x => (int)x.Value == currentCourseId);
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
					Level.DumpBonusSprites(rom);
					Level.DumpPlayerSprite(rom);
					romFilePath = openFileDialog1.FileName;

					SetChanges(false);
					ignoreEvents = true;
					levelComboBox.SelectedIndex = 0;
					ignoreEvents = false;
					LevelComboBoxSelectedIndexChanged(sender, e);						
					
					toolboxToolStripMenuItem.Enabled = true;
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
			
			List<ComboboxItem> items = new List<ComboboxItem>();			
			foreach(var levelInfo in Level.GetCourseIds(rom).OrderBy(x => x.Value))
			{
				ComboboxItem item = new ComboboxItem(string.Format("{0:D2} {1}", levelInfo.Value, levelNames[levelInfo.Key]), levelInfo.Key);
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
			levelPictureBox.Invalidate();
		}

		void ScrollRegionToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.ShowScrollInfo = scrollRegionToolStripMenuItem.Checked;
			levelPictureBox.Invalidate();
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

		void NoneToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetSwitchMode(0);
		}

		void AToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetSwitchMode(1);			
		}

		void BToolStripMenuItemClick(object sender, EventArgs e)
		{			
			SetSwitchMode(2);
		}

		void SetSwitchMode(int value)
		{
			switchMode = value;
			noneToolStripMenuItem.Checked = value == 0;
			aToolStripMenuItem.Checked = value == 1;
			bToolStripMenuItem.Checked = value == 2;			
			levelPictureBox.SwitchMode = value;
			toolboxForm.SwitchMode = value;
			toolboxForm.Invalidate(true);		
			
			LoadLevel(false);
		}

		bool SaveChanges(bool saveFile)
		{
			if (rom.IsLoaded)
			{
				if (hasChanges)
				{
					string message;
					if(!Level.SaveChanges(rom, currentCourseId, out message))
					{
						MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
				
				if(saveFile)
				{
					rom.Save(romFilePath);
					SetChanges(false);
				}
			}
			
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
			if(DispatchCommandKey(e.KeyData))
			{
				e.Handled = true;
			}	
		}
			
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(DispatchCommandKey(keyData))
			{
				return true;
			}
			
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		bool DispatchCommandKey(Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.Add) || keyData == (Keys.Control | Keys.Oemplus))
			{
				zoomInToolStripMenuItem.PerformClick();
				return true;
			}
			if (keyData == (Keys.Control | Keys.Subtract) || keyData == (Keys.Control | Keys.OemMinus))
			{
				zoomOutToolStripMenuItem.PerformClick();
				return true;
			}
						
			if (keyData == Keys.B)
			{
			 	SetSwitchMode((switchMode + 1) % 3);
				return true;
			}
			
			ToolStripMenuItem toolStrip = GetAllMenuItems(menuStrip1.Items)
				.FirstOrDefault(x => x.ShortcutKeys == keyData);
			
			if(toolStrip != null)
			{
				toolStrip.PerformClick();
				return true;
			}

			return false;
		}
				
		IEnumerable<ToolStripMenuItem> GetAllMenuItems(ToolStripItemCollection items)
		{
			foreach(ToolStripItem item in items)
			{
				ToolStripMenuItem toolStrip = item as ToolStripMenuItem;		
				if(toolStrip != null)
				{
					yield return toolStrip;
				}
				
				ToolStripDropDownItem toolStripDropDown = item as ToolStripDropDownItem;	
				if(toolStripDropDown != null)
				{
					foreach(var child in GetAllMenuItems(toolStripDropDown.DropDownItems))
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
		
		void MainFormMouseWheel(object sender, MouseEventArgs e)
		{
			Control control = (Control)sender;
			if (Control.ModifierKeys == Keys.Control &&
				LevelPanel.RectangleToScreen(LevelPanel.ClientRectangle).Contains(control.PointToScreen(e.Location)))
			{
				if(e.Delta > 0 && zoom < 4)
				{
					SetZoomLevel(zoom + 1);
				}
				else if(e.Delta < 0 && zoom > 1)
				{
					SetZoomLevel(zoom - 1);
				}
			}
		}
		
		void ToolboxToolStripMenuItemCheckedChanged(object sender, EventArgs e)
		{
			if(toolboxToolStripMenuItem.Checked)
			{
				if(!toolboxForm.Visible)
				{
					toolboxForm.Show(this);	
				}	
			}
			else
			{
				if(toolboxForm.Visible)
				{
					toolboxForm.Hide();	
				}
			}		
		}		
		
		void ToolBoxFormClosing(object sender, EventArgs e)
		{
			toolboxToolStripMenuItem.Checked = false;
		}
		
		void LevelPictureBoxTileMouseDown(object sender, MouseEventArgs e)
		{				
			if(toolboxForm.Visible)
			{
				int tileIndex = levelPictureBox.CurrentTileIndex;
				int selectedPanelIndex = toolboxForm.SelectedPanelIndex;
				int currentTile = toolboxForm.CurrentTile;
				int currentObject = toolboxForm.CurrentObject;
				
				if (e.Button == MouseButtons.Left)
				{
				
					if(currentTile != -1 && selectedPanelIndex == 0)
					{
						int previousTile = Level.LevelData[tileIndex + 0x1000];
						if(previousTile != currentTile)
						{
							Level.LevelData[tileIndex + 0x1000] = (byte)currentTile;
							levelPictureBox.InvalidateTile(tileIndex);
							SetChanges(true);
						}
					}
					
					if(currentObject != -1 && levelPictureBox.ShowObjects && selectedPanelIndex == 2)
					{	
						int previousObject = Level.ObjectsData[tileIndex];
						if(previousObject != currentObject)
						{
							Level.ObjectsData[tileIndex] = (byte)currentObject;
							levelPictureBox.InvalidateObject(tileIndex, currentObject, previousObject);
							SetChanges(true);
						}				
					}						
				}
				else if (e.Button == MouseButtons.Right)
				{
					if (levelPictureBox.ShowObjects && selectedPanelIndex == 2)
					{	
						int previousObject = Level.ObjectsData[tileIndex];
						if(previousObject != 0)
						{
							Level.ObjectsData[tileIndex] = 0;
							levelPictureBox.InvalidateObject(tileIndex, 0, previousObject);
							SetChanges(true);
						}			
					}
				}
			}			
		}
		
		void LevelPictureBoxSectorChanged(object sender, EventArgs e)
		{
			int warpTarget = Level.SearchWarp(rom, currentCourseId, levelPictureBox.CurrentSector);
			if(warpTarget != currentWarp)
			{
				currentWarp = warpTarget;
				LoadLevel(false);
			}
			else
			{
				levelPictureBox.Invalidate();
			}
		}
				
		void TimerTick(object sender, EventArgs e)
		{
			if(rom.IsLoaded && levelComboBox.SelectedItem != null && Level.AnimatedTilesMask != 0)
			{				
				timerTicks++;				
				if((timerTicks & Level.AnimatedTilesMask) == 0)
				{
					animatedTileIndex = (animatedTileIndex + 1) % 4;
					RefreshAnimatedTiles();
				}
			}
		}
		
		void RefreshAnimatedTiles()
		{
			Level.DumpLevel(rom, currentCourseId, currentWarp, false, switchMode, animatedTileIndex, true);
				
			//redraw
			levelPictureBox.InvalidateAnimatedTiles();
			if(toolboxForm.Visible)
			{
				switch(toolboxForm.SelectedPanelIndex)
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
			MessageBox.Show(Text+" v0.70\r\nDate : 04.12.2021\r\nContact me : tigrou.ind@gmail.com");
		}
		
		void AnimationToolStripMenuItemClick(object sender, EventArgs e)
		{
			timer.Enabled = animationToolStripMenuItem.Checked;
		}
		
		void TileNumbersToolStripMenuItemClick(object sender, EventArgs e)
		{
			bool showTileNumbers = tileNumbersToolStripMenuItem.Checked;
			levelPictureBox.ShowTileNumbers = showTileNumbers;
			levelPictureBox.ClearTileCache();
			levelPictureBox.Invalidate();
			toolboxForm.ShowTileNumbers = showTileNumbers;
			toolboxForm.Invalidate(true);
		}		
	}
}