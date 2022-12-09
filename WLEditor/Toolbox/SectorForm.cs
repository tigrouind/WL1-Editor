using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace WLEditor
{	
	public partial class SectorForm : UserControl
	{						
		public event EventHandler SectorChanged;			
		
		Rom rom;
		int currentCourseId;
		Warp currentWarp;		
		int currentSector;
		int scroll;
		bool ignoreEvents, isWarp;
									
		public SectorForm()
		{
			InitializeComponent();
		}
		
		#region Dropdown data
		
		ComboboxItem<int[]>[] tileSets =
		{
			//bank, tileSetB, tileSetA, blockIndex, palette
			new ComboboxItem<int[]>(new[] { 0x06, 0x6E69, 0x5600, 0x640A, 0xE1 }, "Beach 1"),			
			new ComboboxItem<int[]>(new[] { 0x03, 0x5200, 0x5400, 0x460A, 0xE1 }, "Beach 2"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x4600, 0x5400, 0x4E0A, 0xE1 }, "Beach 3"),
			new ComboboxItem<int[]>(new[] { 0x05, 0x5A29, 0x5000, 0x5C0A, 0xE1 }, "Stone 1"),
			new ComboboxItem<int[]>(new[] { 0x05, 0x5A29, 0x5400, 0x5C0A, 0xE1 }, "Stone 2"),
			new ComboboxItem<int[]>(new[] { 0x05, 0x5A29, 0x5600, 0x5C0A, 0xE1 }, "Stone 3"),
			new ComboboxItem<int[]>(new[] { 0x06, 0x6869, 0x5000, 0x620A, 0xE1 }, "Ice"),
			new ComboboxItem<int[]>(new[] { 0x03, 0x4000, 0x5400, 0x420A, 0xE1 }, "Cave 1"),			
			new ComboboxItem<int[]>(new[] { 0x03, 0x4000, 0x5400, 0x4C0A, 0xE1 }, "Cave 2"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x4000, 0x5400, 0x4A0A, 0xE1 }, "Cave 3"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x4000, 0x5600, 0x4A0A, 0xE1 }, "Cave 4"),			
			new ComboboxItem<int[]>(new[] { 0x0E, 0x4C00, 0x5400, 0x500A, 0xE1 }, "Cave 5"),		
			new ComboboxItem<int[]>(new[] { 0x0E, 0x6A00, 0x5800, 0x5A0A, 0xE4 }, "Lava"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x5200, 0x5400, 0x520A, 0xE1 }, "Train"),
			new ComboboxItem<int[]>(new[] { 0x03, 0x4600, 0x5000, 0x400A, 0xE1 }, "Woods 1"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x5800, 0x5000, 0x540A, 0xE1 }, "Woods 2"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x5E00, 0x5E00, 0x560A, 0x63 }, "Treasure"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x6400, 0x5000, 0x580A, 0xE1 }, "Ship"),			
			new ComboboxItem<int[]>(new[] { 0x03, 0x4C00, 0x5A00, 0x440A, 0xE1 }, "Castle 1"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x7000, 0x5000, 0x660A, 0xE1 }, "Castle 2"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x7000, 0x5000, 0x680A, 0xE1 }, "Castle 3"),
			new ComboboxItem<int[]>(new[] { 0x0E, 0x7600, 0x5200, 0x6C0A, 0xE1 }, "Castle 4"),
			new ComboboxItem<int[]>(new[] { 0x1D, 0x5200, 0x6600, 0x5E0A, 0xE1 }, "Boss 1"),
			new ComboboxItem<int[]>(new[] { 0x1D, 0x4600, 0x6200, 0x5E0A, 0xE1 }, "Boss 2"),						
			new ComboboxItem<int[]>(new[] { 0x1D, 0x4C00, 0x6400, 0x5E0A, 0xE1 }, "Boss 3"),
			new ComboboxItem<int[]>(new[] { 0x1D, 0x5E00, 0x6A00, 0x600A, 0xD2 }, "Boss 4"),
			new ComboboxItem<int[]>(new[] { 0x1D, 0x6400, 0x6C00, 0x600A, 0xE4 }, "Boss 5"),
			new ComboboxItem<int[]>(new[] { 0x1D, 0x5800, 0x6800, 0x600A, 0xE1 }, "Boss 6"),
			new ComboboxItem<int[]>(new[] { 0x1D, 0x6A00, 0x6E00, 0x6A0A, 0xE1 }, "Boss 7")
		};	
		
		int[][] enemyData =
		{
			//same enemyid / tilebank / tileaddress / tilecount / code 
			new [] { 0x41EF },
			new [] { 0x4208 },
			new [] { 0x4219 },
			new [] { 0x422A },
			new [] { 0x423B },
			new [] { 0x4254 },
			new [] { 0x42A9, 0x42BA, 0x42FE, 0x4927, 0x4986, 0x49CA, 0x4BD9, 0x4D61, 0x5028, 0x5096, 0x5228 },
			new [] { 0x42CB, 0x4997, 0x49B9 },
			new [] { 0x42DC, 0x4464, 0x44A8, 0x45EB, 0x47B3, 0x4818, 0x4B3B, 0x4B6E, 0x4C32, 0x4E23, 0x4EEA, 0x4F60, 0x4F71, 0x4FAC, 0x506C, 0x5158, 0x51BA, 0x51F5, 0x5217, 0x52E5, 0x4D09 },
			new [] { 0x42ED },
			new [] { 0x430F, 0x49DB },
			new [] { 0x4325, 0x4E95 },
			new [] { 0x4336 },
			new [] { 0x4347 },
			new [] { 0x4358, 0x437A, 0x4402, 0x4413, 0x46A0, 0x486D, 0x4DE8 },
			new [] { 0x4369, 0x4520, 0x46B1, 0x4807, 0x4FF5, 0x5147 },
			new [] { 0x438B },
			new [] { 0x43A4 },
			new [] { 0x43C2 },
			new [] { 0x43D8 },
			new [] { 0x43F1 },
			new [] { 0x4424 },
			new [] { 0x443D },
			new [] { 0x4453 },
			new [] { 0x4475 },
			new [] { 0x4486 },
			new [] { 0x4497, 0x4B7F },
			new [] { 0x44B9, 0x44E0, 0x4BA1, 0x4CF3 },
			new [] { 0x44CF, 0x4B90, 0x4FD3, 0x4FE4 },
			new [] { 0x44F6 },
			new [] { 0x4507 },
			new [] { 0x4531 },
			new [] { 0x4542 },
			new [] { 0x455B },
			new [] { 0x456C },
			new [] { 0x457D },
			new [] { 0x4596, 0x4A1B },
			new [] { 0x45A7 },
			new [] { 0x45B8, 0x467E, 0x46D3, 0x4975, 0x4D9C, 0x50A7 },
			new [] { 0x45C9, 0x50EB },
			new [] { 0x45DA },
			new [] { 0x45FC },
			new [] { 0x460D },
			new [] { 0x464C },
			new [] { 0x4665 },
			new [] { 0x468F },
			new [] { 0x46C2, 0x4778, 0x4C87, 0x4C98, 0x4D72, 0x5039, 0x504A, 0x50DA },
			new [] { 0x46E4 },
			new [] { 0x46FD },
			new [] { 0x4716 },
			new [] { 0x4727 },
			new [] { 0x473D },
			new [] { 0x4756 },
			new [] { 0x4767 },
			new [] { 0x4789 },
			new [] { 0x47A2 },
			new [] { 0x47C4 },
			new [] { 0x47DD },
			new [] { 0x47F6 },
			new [] { 0x4829, 0x5169 },
			new [] { 0x483A },
			new [] { 0x484B },
			new [] { 0x485C },
			new [] { 0x487E },
			new [] { 0x488F },
			new [] { 0x48A0 },
			new [] { 0x48B1 },
			new [] { 0x48C2 },
			new [] { 0x48D3 },
			new [] { 0x48EC },
			new [] { 0x48FD },
			new [] { 0x4916 },
			new [] { 0x4938 },
			new [] { 0x494E },
			new [] { 0x495F },
			new [] { 0x49A8 },
			new [] { 0x49F1 },
			new [] { 0x4A0A },
			new [] { 0x4A2C },
			new [] { 0x4A3D },
			new [] { 0x4A56 },
			new [] { 0x4A6F },
			new [] { 0x4A88 },
			new [] { 0x4AA1 },
			new [] { 0x4AB2 },
			new [] { 0x4AC3 },
			new [] { 0x4ADC },
			new [] { 0x4AED },
			new [] { 0x4B06 },
			new [] { 0x4B2A },
			new [] { 0x4B4C },
			new [] { 0x4B5D },
			new [] { 0x4BB7 },
			new [] { 0x4BC8 },
			new [] { 0x4BEA },
			new [] { 0x4BFB, 0x4F25, 0x4F4F },
			new [] { 0x4C0C },
			new [] { 0x4C43 },
			new [] { 0x4C54 },
			new [] { 0x4C65 },
			new [] { 0x4C76 },
			new [] { 0x4CA9 },
			new [] { 0x4CE2 },
			new [] { 0x4D1A },
			new [] { 0x4D48 },
			new [] { 0x4D83, 0x4DAD },
			new [] { 0x4DC6 },
			new [] { 0x4DD7 },
			new [] { 0x4DF9 },
			new [] { 0x4E12 },
			new [] { 0x4E34 },
			new [] { 0x4E62 },
			new [] { 0x4E73 },
			new [] { 0x4E84 },
			new [] { 0x4EA6 },
			new [] { 0x4EB7 },
			new [] { 0x4EC8 },
			new [] { 0x4ED9 },
			new [] { 0x4EFB },
			new [] { 0x4F14 },
			new [] { 0x4F36 },
			new [] { 0x4F82 },
			new [] { 0x4F9B },
			new [] { 0x4FBD },
			new [] { 0x5006 },
			new [] { 0x5017 },
			new [] { 0x505B },
			new [] { 0x507D },
			new [] { 0x50B8, 0x526C },
			new [] { 0x50C9 },
			new [] { 0x50FC },
			new [] { 0x5115 },
			new [] { 0x512E },
			new [] { 0x517A },
			new [] { 0x5190 },
			new [] { 0x51A9 },
			new [] { 0x51CB },
			new [] { 0x51E4 },
			new [] { 0x5206 },
			new [] { 0x5239 },
			new [] { 0x524A },
			new [] { 0x525B },
			new [] { 0x527D },
			new [] { 0x52AA },
			new [] { 0x52BB },
			new [] { 0x52CC },
			new [] { 0x52F6 }
		};
		
		ComboboxItem<int>[] tilesAnimation =
		{
			new ComboboxItem<int>(0x4000, "Platform"),	
			new ComboboxItem<int>(0x4100, "Sand / plant"), 		
			new ComboboxItem<int>(0x4800, "Water"),			
			new ComboboxItem<int>(0x4A00, "Water / plant"),			
			new ComboboxItem<int>(0x4400, "Waterfall"),			
			new ComboboxItem<int>(0x4600, "Water flow"),	
			new ComboboxItem<int>(0x4E00, "Treasure room"),
			new ComboboxItem<int>(0x4500, "Lava 1"),	
			new ComboboxItem<int>(0x4900, "Lava 2"),			
			new ComboboxItem<int>(0x4200, "Lava 3"),			
			new ComboboxItem<int>(0x4C00, "Piranha"),				
			new ComboboxItem<int>(0x4700, "Train left"),
			new ComboboxItem<int>(0x4F00, "Train right"),			
			new ComboboxItem<int>(0x4B00, "Conveyor belt"),		
			new ComboboxItem<int>(0x4D00, "Castle")		
		};
					
		ComboboxItem<int>[] music = 
		{			
			new ComboboxItem<int>(0x7ED5, "Beach 1"),
			new ComboboxItem<int>(0x7F29, "Beach 2"),
			new ComboboxItem<int>(0x7EE1, "Cave 1"),
			new ComboboxItem<int>(0x7EED, "Cave 2"),
			new ComboboxItem<int>(0x7F11, "Cave 3"),
			new ComboboxItem<int>(0x7F1D, "Cave 4"),
			new ComboboxItem<int>(0x7F35, "Cave 5"),
			new ComboboxItem<int>(0x7F4D, "Cave 6"),
			new ComboboxItem<int>(0x7EC9, "Lake"),
			new ComboboxItem<int>(0x7EF9, "Train"),			
			new ComboboxItem<int>(0x7F05, "Boss Level"),			
			new ComboboxItem<int>(0x7F41, "Syrup Castle"),			
			new ComboboxItem<int>(0x7F59, "Sherbet Land")
		};
		
		ComboboxItem<int>[] animationSpeed = 
		{
			new ComboboxItem<int>(0x00, "None"),
			new ComboboxItem<int>(0x1F, "Slow"),			
			new ComboboxItem<int>(0x0F, "Normal"),
			new ComboboxItem<int>(0x07, "Fast"),		
			new ComboboxItem<int>(0x03, "Fastest")
		};
		
		ComboboxItem<int>[] cameraTypes = 
		{
			new ComboboxItem<int>(0x00, "X scroll"),
			new ComboboxItem<int>(0x10, "X/Y scroll"),
			new ComboboxItem<int>(0x01, "Train bumps"),
			new ComboboxItem<int>(0x30, "Train auto-scroll right"),
			new ComboboxItem<int>(0x31, "Train auto-scroll left"),
			new ComboboxItem<int>(0xFF, "No scroll (boss fight)")
		};
		
		ComboboxItem<int>[] warps = 
		{
			new ComboboxItem<int>(0x5B76, "None"),
			new ComboboxItem<int>(0x5B77, "Exit map"),
			new ComboboxItem<int>(0x5B78, "Exit A"),		
			new ComboboxItem<int>(0x5B79, "Exit B")
		};
		
		Dictionary<int, string> enemyNames = new Dictionary<int, string>
		{
			{ 1, "Wanderin' Goom" },
			{ 2, "Sparky" },
			{ 3, "Big" },
			{ 4, "Spiked ball" },
			{ 5, "Pirate Goom" },
			{ 6, "Helmut" },
			{ 8, "100 Coin" },
			{ 9, "Pouncer" },
			{ 10, "Pouncer" },
			{ 11, "Dropper" },
			{ 12, "White puff" },
			{ 13, "Bō" },
			{ 14, "Door" },
			{ 15, "Exit" },
			{ 16, "Cart" },
			{ 17, "Cart" },
			{ 18, "Gaugau" },
			{ 19, "Penkoon" },
			{ 20, "D.D." },
			{ 21, "Checkpoint" },
			{ 22, "Chest" },
			{ 23, "Flame" },
			{ 24, "Treasure" },
			{ 25, "Halo" },			
			{ 27, "Watch" },
			{ 28, "Chicken Duck" },			
			{ 38, "Floater" },
			{ 39, "Treasure door" },
			{ 40, "Bridge" },
			{ 41, "Bucket Head" },
			{ 42, "! Block" },
			{ 43, "Lava" },
			{ 44, "Elevator up" },
			{ 45, "Elevator down" },
			{ 46, "Yadorā" },
			{ 47, "Yarikuri Obake" },
			{ 48, "Pinwheel" },
			{ 49, "Giant block" },
			{ 50, "Mine" },
			{ 55, "Pecan" },
			{ 56, "Skewer" },
			{ 57, "Skewer" },
			{ 58, "Skewer" },
			{ 59, "Skewer" },
			{ 60, "Maizō" },
			{ 61, "Sinking platform" },
			{ 62, "Togemaru" },
			{ 63, "Pikkarikun" },			
			{ 64, "Guragura" },
			{ 65, "Spike ball" },
			{ 66, "Ukiwani" },
			{ 67, "Goboten" },
			{ 68, "3-Up heart" },
			{ 69, "Paidan" },
			{ 70, "Harisu" },
			{ 71, "Guragura" },
			{ 72, "Kōmori Missile" },
			{ 73, "Konotako" },
			{ 74, "Knight" },
			{ 75, "Door" },
			{ 76, "Bee Fly" },
			{ 77, "Skull" },
			{ 78, "Demon Bat" }
		};
		
		string EnemyToString(int enemyId, bool exitOpen)
		{			
			string enemyName;
			if (enemyNames.TryGetValue(enemyId, out enemyName))
			{
				if (enemyId == 15 && exitOpen)
				{
					enemyName += " open";
				}
				
				return enemyName;
			}
			
			return enemyId.ToString("D2");
		}
		
		string GetEnemyInfo(int enemyPointer)
		{
			//boss
			int[] boss = { 0x4CA9, 0x460D, 0x4C0C, 0x4E34, 0x4B06, 0x4D1A, 0x527D };
			int bossId = Array.IndexOf(boss, enemyPointer);
			if (bossId != -1)
			{
				return string.Format("[Boss {0}]", bossId + 1);
			}
			
			//treasure/exit open
			int enemiesIdsPointer, tilesPointer, treasureId;
			bool exitOpen, treasureCheck;
			
			Level.FindEnemiesData(rom, enemyPointer, out enemiesIdsPointer, out tilesPointer, out treasureId, out treasureCheck, out exitOpen);
			
			string prefix = string.Empty;
			char[] treasureNames = { 'C', 'I', 'F', 'O', 'A', 'N', 'H', 'M', 'L', 'K', 'B', 'D', 'G', 'J', 'E' };			
			if (treasureId >= 1 && treasureId <= 15)
			{
				prefix = string.Format("[Treasure {0}]{1}   ", treasureNames[treasureId - 1], treasureCheck ? " [X]" : string.Empty);
			}
					
			//enemy ids
			List<int> enemies = Level.GetEnemyIds(rom, enemiesIdsPointer);				
			return prefix + string.Join(" / ", enemies.Select(x => EnemyToString(x, exitOpen)).ToArray());
		}
		
		void InitForm()
		{					
			ddlAnimationSpeed.Items.Clear();
			ddlAnimationSpeed.Items.AddRange(animationSpeed);
			
			ddlCameraType.Items.Clear();
			ddlCameraType.Items.AddRange(cameraTypes);
			
			ddlMusic.Items.Clear();
			ddlMusic.Items.AddRange(music);
			
			ddlTileSet.Items.Clear();
			for(int i = 0 ; i < tileSets.Length; i++)
			{
				var item = tileSets[i];
				ddlTileSet.Items.Add(new ComboboxItem<int[]>(item.Value, string.Format("{0:D2}   {1}", i, item.Text)));
			}
						
			var sorted = enemyData.Select(x => new { Text = GetEnemyInfo(x[0]), Value = x })
				.OrderBy(x => x.Text[0] == '[')
				.ThenBy(x => x.Text)
				.ToArray();
			
			ddlEnemies.Items.Clear();
			for(int i = 0 ; i < sorted.Length; i++)
			{
				var item = sorted[i];
				ddlEnemies.Items.Add(new ComboboxItem<int[]>(item.Value, string.Format("{0:D3}   {1}", i, item.Text)));
			}
			
			ddlAnimation.Items.Clear();
			for (int i = 0 ; i < tilesAnimation.Length ; i++)
			{
				var item = tilesAnimation[i];
				ddlAnimation.Items.Add(new ComboboxItem<int>(item.Value, string.Format("{0:D2}   {1}", i, item.Text)));
			}
		}
		
		#endregion				
		
		public void SetSize()
		{
			Width = 339;
			Height = 320;
		}
				
		void SetControlsVisibility()
		{
			bool sectorLoaded = currentSector != -1;
			
			labScroll.Visible = sectorLoaded;
			
			checkBoxLeft.Visible = sectorLoaded;
			checkBoxRight.Visible = sectorLoaded;			
			
			labWarp.Visible = sectorLoaded;
			ddlWarp.Visible = sectorLoaded;
			
			groupBox1.Visible = sectorLoaded;			
			groupBox1.Text = isWarp ? "Warp" : "Level";
			
			labMusic.Visible = !isWarp;
			ddlMusic.Visible = !isWarp;
		}
		
		#region Load
		
		public void LoadRom(Rom rom)
		{
			this.rom = rom;
			InitForm();
		}
				
		public void LoadSector(Rom rom, int course, int sector)
		{		
			currentCourseId = course;
			currentSector = sector;	
			
			if (currentCourseId != -1 && currentSector != -1)
			{		
				int warp = Level.GetWarp(rom, currentCourseId, currentSector);				
				LoadWarp(warp);												
				LoadScroll();												
			}
			else
			{
				currentWarp = null;
			}				
						
			SetControlsVisibility();
		}
		
		void LoadWarp(int warp)
		{					
			LoadWarpsDropdown(warp);
			LoadDropdown(ddlWarp, warp);	
			
			if(warp >= 0x5B7A)
			{
				currentWarp = Level.GetWarp(rom, warp);									
				isWarp = true;
			}
			else
			{
				currentWarp = Level.GetLevelHeader(rom, currentCourseId);	
				isWarp = false;
			}
			
			LoadWarp();
		}
				
		void LoadWarpsDropdown(int selectedWarp)
		{
			var used = Level.GetWarpUsage(rom);
						
			ddlWarp.Items.Clear();
			ddlWarp.Items.AddRange(warps);
						
			for (int i = 0 ; i < 370 ; i++)
			{
				int warp = 0x5B7A + i * 24;
				if (!used.Contains(warp) || warp == selectedWarp)
				{
					ddlWarp.Items.Add(new ComboboxItem<int>(warp, string.Format("{0:D3}", i)));	
				}				
			}
		}
							
		void LoadScroll()
		{
			scroll = Level.GetScroll(rom, currentCourseId, currentSector);			
			LoadCheckBox(checkBoxLeft, (scroll & 2) == 2);
			LoadCheckBox(checkBoxRight, (scroll & 1) == 1);
		}
		
		void LoadWarp()
		{
			LoadDropdown(ddlTileSet, new []
			{
				currentWarp.Bank, 
				currentWarp.TileSetB, 
				currentWarp.TileSetA, 
				currentWarp.BlockIndex, 
				currentWarp.Palette 
			});				
			LoadDropdown(ddlAnimation, currentWarp.TileAnimation);
			LoadDropdown(ddlAnimationSpeed, currentWarp.AnimationSpeed);
			LoadDropdownAny(ddlEnemies, currentWarp.Enemy);
			
			LoadDropdown(ddlCameraType, currentWarp.CameraType);					
			LoadNumericUpDown(txbCameraX, currentWarp.CameraX);
			LoadNumericUpDown(txbCameraY, currentWarp.CameraY);	
			
			LoadNumericUpDown(txbWarioX, currentWarp.WarioX);
			LoadNumericUpDown(txbWarioY, currentWarp.WarioY);		
			
			if (!isWarp)
			{
				LoadDropdown(ddlMusic, currentWarp.Music);
			}
		}	
				
		void LoadCheckBox(CheckBox checkBox, bool value)
		{
			ignoreEvents = true;
			checkBox.Checked = value;
			ignoreEvents = false;
		}
		
		void LoadNumericUpDown(NumericUpDown numericUpDown, int value)
		{	
			ignoreEvents = true;
			numericUpDown.Value = Math.Min(Math.Max(value, (int)numericUpDown.Minimum), (int)numericUpDown.Maximum);
			ignoreEvents = false;
		}

		void LoadDropdown(ComboBox combo, int value)
		{				
			ignoreEvents = true;
			combo.SelectedIndex = combo.Items.Cast<ComboboxItem<int>>().FindIndex(x => x.Value == value);
			ignoreEvents = false;
		}
		
		void LoadDropdownAny(ComboBox combo, int value)
		{		
			ignoreEvents = true;			
			combo.SelectedIndex = combo.Items.Cast<ComboboxItem<int[]>>().FindIndex(x => x.Value.Contains(value));
			ignoreEvents = false;
		}
		
		void LoadDropdown(ComboBox combo, int[] value)
		{		
			ignoreEvents = true;			
			combo.SelectedIndex = combo.Items.Cast<ComboboxItem<int[]>>().FindIndex(x => x.Value.SequenceEqual(value));
			ignoreEvents = false;
		}
		
		#endregion
		
		#region Save
				
		void CheckBoxLeftCheckedChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				scroll = (scroll & ~0x2) | (checkBoxLeft.Checked ? 0x2 : 0);
				
				Level.SaveScroll(rom, currentCourseId, currentSector, scroll);
				OnSectorChanged();
			}
		}
		
		void CheckBoxRightCheckedChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				scroll = (scroll & ~0x1) | (checkBoxRight.Checked ? 0x1 : 0);
				
				Level.SaveScroll(rom, currentCourseId, currentSector, scroll);
				OnSectorChanged();
			}
		}
				
		void DdlWarpSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlWarp.SelectedItem;
				int warp = item.Value;
				
				Level.SaveWarp(rom, currentCourseId, currentSector, warp);				
				LoadWarp(warp);									
				
				OnSectorChanged();
				SetControlsVisibility();
			}
		}

		protected void DdlTileSetSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int[]>)ddlTileSet.SelectedItem;
				currentWarp.Bank = item.Value[0];
				currentWarp.TileSetB = item.Value[1];
				currentWarp.TileSetA = item.Value[2];
				currentWarp.BlockIndex = item.Value[3];
				currentWarp.Palette = item.Value[4];
				
				SaveWarp();
				OnSectorChanged();
			}
		}
		
		protected void DdlAnimationSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlAnimation.SelectedItem;
				currentWarp.TileAnimation = item.Value;
				
				SaveWarp();
				OnSectorChanged();
			}
		}
			
		protected void DdlAnimationSpeedSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlAnimationSpeed.SelectedItem;
				currentWarp.AnimationSpeed = item.Value;
				
				SaveWarp();
				OnSectorChanged();
			}
		}
		
		protected void DdlEnemiesSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int[]>)ddlEnemies.SelectedItem;
				currentWarp.Enemy = item.Value[0];
				
				SaveWarp();
				OnSectorChanged();
			}
		}
						
		protected void TxbWarioXValueChanged(object sender, EventArgs e)
		{			
			if (!ignoreEvents)
			{
				currentWarp.WarioX = (int)txbWarioX.Value;
				
				SaveWarp();
				OnSectorChanged();
			}
		}
		
		protected void TxbWarioYValueChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				currentWarp.WarioY = (int)txbWarioY.Value;
				
				SaveWarp();
				OnSectorChanged();
			}
		}
		
		void TxbCameraXValueChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				currentWarp.CameraX = (int)txbCameraX.Value;
				
				SaveWarp();
				OnSectorChanged();
			}
		}
		
		void TxbCameraYValueChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				currentWarp.CameraY = (int)txbCameraY.Value;
				
				SaveWarp();
				OnSectorChanged();
			}
		}
		
		void DdlCameraTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlCameraType.SelectedItem;
				currentWarp.CameraType = item.Value;
			
				SaveWarp();
				OnSectorChanged();
			}
		}		

		void DdlMusicSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlMusic.SelectedItem;
				currentWarp.Music = item.Value;
				
				SaveWarp();
				OnSectorChanged();
			}
		}		
		
		void OnSectorChanged()
		{
			if (SectorChanged != null)
			{
				SectorChanged(this, EventArgs.Empty);
			}
		}
		
		void SaveWarp()
		{
			if (isWarp)
			{
				Level.SaveWarp(rom, currentCourseId, currentSector, currentWarp);
			}
			else
			{
				Level.SaveLevelHeader(rom, currentCourseId, currentWarp);
			}
		}
		
		#endregion
	
		#region Calculate
		
		void CmdCalculatePosClick(object sender, EventArgs e)
		{
			int sector = currentWarp.WarioX / 32 + currentWarp.WarioY / 32 * 16;
			
			string sectorTxt = Interaction.InputBox(string.Empty, "Enter sector number (0-31)", sector.ToString(), -1, -1);
			if (!(int.TryParse(sectorTxt, out sector) && sector >= 0 && sector <= 31))
			{
				return;
			}
			
			currentWarp.WarioX = (sector % 16) * 32 + currentWarp.WarioX % 32;
			currentWarp.WarioY = (sector / 16) * 32 + currentWarp.WarioY % 32;
			
			int doorX, doorY;
			if (Level.FindDoorInSector(sector, out doorX, out doorY))
			{				
				//player position
				currentWarp.WarioX = (sector % 16) * 32 + Math.Min(doorX * 2 + 1, 31);
				currentWarp.WarioY = (sector / 16) * 32 + Math.Min(doorY * 2 + 2, 31);
			}			
							
			//camera position (scroll area: 10 x 9)				
			int cameraX = currentWarp.WarioX - 12;
			int cameraY = currentWarp.WarioY - 16;
						
			LimitScroll(sector, ref cameraX, ref cameraY, ref currentWarp.WarioY);
			
			currentWarp.CameraX = cameraX;
			currentWarp.CameraY = cameraY;
			
			SaveWarp();
			LoadWarp();
			SectorChanged(this, EventArgs.Empty);
		}		
							
		void LimitScroll(int sector, ref int cameraX, ref int cameraY, ref int warioY)
		{
			int scrollData = Level.GetScroll(rom, currentCourseId, sector);
			bool allowLeft = (scrollData & 2) != 2;
			bool allowRight = (scrollData & 1) != 1;
			int sectorX = sector % 16;			
			
			if (!allowLeft)
			{
				cameraX = Math.Max(cameraX, sectorX * 32);
			}
			else 
			{
				cameraX = Math.Max(cameraX, 0);
			}
			
			if (!allowRight)
			{
				cameraX = Math.Min(cameraX, sectorX * 32 + 10);
			}
			else 
			{
				cameraX = Math.Min(cameraX, 15 * 32 + 10);
			}			
							
			cameraY = Math.Max(cameraY, 0);  //top limit
			cameraY = Math.Min(cameraY, 32 + 12); //bottom limit

			if (currentWarp.CameraType == 0) //scroll X
			{
				if (cameraY < 16)
				{
					cameraY = 12;
					warioY = Math.Max(warioY, 16);
				}
				else if (cameraY >= 16 && cameraY < 32)
				{
					cameraY = 28;
				}
				else
				{
					cameraY = 44;
				}
			}
		}
		
		#endregion
	}
}
