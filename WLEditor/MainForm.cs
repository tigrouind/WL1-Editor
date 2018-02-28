using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class MainForm : Form
	{
		public Rom rom = new Rom();		 		
		public ToolboxForm toolboxForm = new ToolboxForm();
		
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
			InitializeComponent();						
			
			toolboxForm.FormClosing += (send, ev) => toolboxToolStripMenuItem.Checked = false;
			toolboxForm.MainForm = this;
			toolboxForm.InitializeControls();
			
			levelPictureBox.MainForm = this;			
		}

		public void LoadLevel(bool reloadAll)
		{
			if(rom.IsLoaded && levelComboBox.SelectedItem != null)
			{
				levelPictureBox.ClearTileCache(); 
				Level.DumpLevel(rom, currentCourseId, currentWarp, reloadAll, aToolStripMenuItem.Checked, bToolStripMenuItem.Checked, SelectedPaletteToolStripIndex());
				while(currentObject >= 1 && currentObject <= 6 && !Level.enemiesAvailable[currentObject - 1]) currentObject--;
				
				levelPictureBox.Refresh();				
				toolboxForm.RefreshPictureBoxes();
				toolboxToolStripMenuItem.Enabled = true;
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
					Level.DumpBonusSprites(rom);
					Level.DumpPlayerSprite(rom);
					romFilePath = openFileDialog1.FileName;

					if(levelComboBox.SelectedIndex == 0)
						LevelComboBoxSelectedIndexChanged(sender, e);
					else
						levelComboBox.SelectedIndex = 0;
					
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
			levelPictureBox.Refresh();
		}

		void ObjectsToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.Refresh();
		}

		void ScrollRegionToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.Refresh();
		}

		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Application.Exit();
		}

		void CollidersToolStripMenuItemClick(object sender, EventArgs e)
		{
			levelPictureBox.ClearTileCache();
			levelPictureBox.Refresh();
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
		
		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			MessageBox.Show(Text+" v0.66\r\nDate : 27.02.2018.\r\nContact me : tigrou.ind@gmail.com");
		}

		public bool ViewRegions
		{
			get
			{
				return regionsToolStripMenuItem.Checked;
			}			
		}
		
		public bool ViewScroll
		{
			get			
			{
				return scrollRegionToolStripMenuItem.Checked;	
			}			
		}
		
		public bool ViewObjects
		{
			get
			{
				return objectsToolStripMenuItem.Checked; 
			}
			
		}
		
		public bool ViewSwitchA
		{
			get
			{			
				return aToolStripMenuItem.Checked;
			}
		}
		
		public bool ViewSwitchB
		{
			get
			{
				return bToolStripMenuItem.Checked;
			}
		}
		
		public bool ViewColliders
		{
			get
			{
				return collidersToolStripMenuItem.Checked;			
			}
		}	
	}
}