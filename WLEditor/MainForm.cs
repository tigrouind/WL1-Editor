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
			
		int currentCourseId = -1;
		int currentWarp = -1;
		int paletteIndex;
		string romFilePath;
		bool hasChanges;
		int zoom = 1;
				
		public MainForm()
		{
			InitializeComponent();									
			toolboxForm.FormClosing += ToolBoxFormClosing;			
		}

		public void LoadLevel(bool reloadAll)
		{
			if(rom.IsLoaded && levelComboBox.SelectedItem != null)
			{				
				Level.DumpLevel(rom, currentCourseId, currentWarp, reloadAll, levelPictureBox.SwitchMode, paletteIndex);
				
				levelPictureBox.ClearTileCache();
				levelPictureBox.Invalidate();	
				
				//make sure current object can still be selected, otherwise select first available previous object
				int currentObject = toolboxForm.CurrentObject;
				while(currentObject >= 1 && currentObject <= 6 && !Level.enemiesAvailable[currentObject - 1])
				{
					currentObject--;
				}
				toolboxForm.CurrentObject = currentObject;									
				toolboxForm.Invalidate(true);				
			}
		}			

		void LevelComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if(levelComboBox.SelectedItem != null)
			{
				if(AskForSavingChanges())
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
					Level.DumpBonusSprites(rom);
					Level.DumpPlayerSprite(rom);
					romFilePath = openFileDialog1.FileName;

					if(levelComboBox.SelectedIndex == 0)
						LevelComboBoxSelectedIndexChanged(sender, e);
					else
						levelComboBox.SelectedIndex = 0;
					
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
				ComboboxItem item = new ComboboxItem(string.Format("{0:D2} {1}", levelInfo.Value, Level.levelNames[levelInfo.Key]), levelInfo.Key);
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
			levelPictureBox.ShowColliders = collidersToolStripMenuItem.Checked;
			levelPictureBox.ClearTileCache();
			levelPictureBox.Invalidate();
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

		void SetSwitchMode(int switchMode)
		{
			noneToolStripMenuItem.Checked = switchMode == 0;
			aToolStripMenuItem.Checked = switchMode == 1;
			bToolStripMenuItem.Checked = switchMode == 2;			
			levelPictureBox.SwitchMode = switchMode;
			
			LoadLevel(false);
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
		
		public void SetChanges(bool hasChanges)
		{			
			this.hasChanges = hasChanges;
			saveToolStripMenuItem.Enabled = hasChanges;
		}

		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveChanges();
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
			SetPalette(0);			
		}

		void BlackWhiteToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetPalette(1);
		}

		void AutumnToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetPalette(2);
		}
		
		void SetPalette(int index)
		{
			paletteIndex = index;
			classicToolStripMenuItem.Checked = index == 0;
			blackWhiteToolStripMenuItem.Checked = index == 1;
			autumnToolStripMenuItem.Checked = index == 2;
			
			LoadLevel(false);
		}
		
		void SetZoomLevel(int zoomLevel)
		{
			zoom = zoomLevel;
			zoom100ToolStripMenuItem.Checked = zoomLevel == 1;
			zoom200ToolStripMenuItem.Checked = zoomLevel == 2;	
			zoom300ToolStripMenuItem.Checked = zoomLevel == 3;	
			zoom400ToolStripMenuItem.Checked = zoomLevel == 4;	
						
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
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
		    if (keyData == (Keys.Control | Keys.Add))
		    {
		    	ZoomInToolStripMenuItemClick(this, EventArgs.Empty);
		        return true;
		    }
		    if (keyData == (Keys.Control | Keys.Subtract))
		    {
		        ZoomOutToolStripMenuItemClick(this, EventArgs.Empty);
		        return true;
		    }
		    return base.ProcessCmdKey(ref msg, keyData);
		}
		
		void ZoomOutToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(zoom > 1)
			{
				SetZoomLevel(zoom - 1);
			}
		}
		
		void ZoomInToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(zoom < 4)
			{
				SetZoomLevel(zoom + 1);
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
		
		void LevelPictureBoxTileMouseDown(object sender, int tileIndex)
		{
			if(toolboxForm.Visible)
			{
				int selectedPanelIndex = toolboxForm.SelectedPanelIndex;
				int currentTile = toolboxForm.CurrentTile;
				int currentObject = toolboxForm.CurrentObject;
				
				if(currentTile != -1 &&  selectedPanelIndex == 0)
				{
					int previousTile = Level.levelData[tileIndex + 0x1000];
			        if(previousTile != currentTile)
			        {
			        	Level.levelData[tileIndex + 0x1000] = (byte)currentTile;
			        	levelPictureBox.InvalidateTile(tileIndex);
			            SetChanges(true);
			        }						
				}
				
				if(currentObject != -1 && levelPictureBox.ShowObjects && selectedPanelIndex == 2)
				{	
					int previousObject = Level.objectsData[tileIndex];
					if(previousObject != currentObject)
					{
						Level.objectsData[tileIndex] = (byte)currentObject;
						levelPictureBox.InvalidateObject(tileIndex, currentObject, previousObject);
						SetChanges(true);
					}				
				}	
			}			
		}
		
		void LevelPictureBoxSectorChanged(object sender, int currentSector)
		{
			int warpTarget = Level.SearchWarp(rom, currentCourseId, currentSector);
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

		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			MessageBox.Show(Text+" v0.66\r\nDate : 27.02.2018.\r\nContact me : tigrou.ind@gmail.com");
		}		
	}
}