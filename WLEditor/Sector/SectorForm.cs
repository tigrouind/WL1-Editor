using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class SectorForm : Form
	{
		public event EventHandler<KeyEventArgs> ProcessCommandKey;
		public event EventHandler SectorChanged;
		public event EventHandler<(int Sector, bool Checkpoint, int TreasureId)> SectorLoad;

		public readonly DirectBitmap TilesEnemies = new DirectBitmap(32 * 6, 32 * 147);
		public readonly (Rectangle Rectangle, Point Point)[] enemiesRects = new (Rectangle, Point)[6 * 147];
		readonly char[] treasureNames = { 'C', 'I', 'F', 'O', 'A', 'N', 'H', 'M', 'L', 'K', 'B', 'D', 'G', 'J', 'E' };
		readonly int[] boss = { 0x4CA9, 0x460D, 0x4C0C, 0x4E34, 0x4B06, 0x4D1A, 0x527D };

		Rom rom;
		int currentCourseId;
		Warp currentWarp;
		int currentSector;
		int currentTreasureId;
		bool currentCheckPoint;
		int scroll;
		int zoom;
		bool ignoreEvents;
		bool formLoaded;

		public SectorForm()
		{
			InitializeComponent();
		}

		#region Dropdown data

		readonly ComboboxItemCollection<(int Bank, int TileSetB, int TileSetA, int BlockIndex, int Palette, int GUI)> tileSets = new ComboboxItemCollection<(int, int, int, int, int, int)>
		{
			{ ( 0x03, 0x5200, 0x5400, 0x460A, 0xE1, 0x7200 ), "00   Beach 1" },
			{ ( 0x0E, 0x4600, 0x5400, 0x4E0A, 0xE1, 0x7200 ), "01   Beach 2" },
			{ ( 0x06, 0x6E69, 0x5600, 0x640A, 0xE1, 0x7200 ), "02   Beach 3" },
			{ ( 0x05, 0x5A29, 0x5000, 0x5C0A, 0xE1, 0x7200 ), "03   Stone 1" },
			{ ( 0x05, 0x5A29, 0x5400, 0x5C0A, 0xE1, 0x7200 ), "04   Stone 2" },
			{ ( 0x05, 0x5A29, 0x5600, 0x5C0A, 0xE1, 0x7200 ), "05   Stone 3" },
			{ ( 0x06, 0x6869, 0x5000, 0x620A, 0xE1, 0x7200 ), "06   Ice" },
			{ ( 0x03, 0x4000, 0x5400, 0x420A, 0xE1, 0x7200 ), "07   Cave 1" },
			{ ( 0x03, 0x4000, 0x5400, 0x4C0A, 0xE1, 0x7200 ), "08   Cave 2" },
			{ ( 0x0E, 0x4C00, 0x5400, 0x500A, 0xE1, 0x7200 ), "09   Cave 3" },
			{ ( 0x0E, 0x4000, 0x5400, 0x4A0A, 0xE1, 0x7200 ), "10   Cave 4" },
			{ ( 0x0E, 0x4000, 0x5600, 0x4A0A, 0xE1, 0x7200 ), "11   Cave 5" },
			{ ( 0x0E, 0x6A00, 0x5800, 0x5A0A, 0xE4, 0x7000 ), "12   Lava" },
			{ ( 0x0E, 0x5200, 0x5400, 0x520A, 0xE1, 0x7200 ), "13   Train" },
			{ ( 0x03, 0x4600, 0x5000, 0x400A, 0xE1, 0x7200 ), "14   Woods 1" },
			{ ( 0x0E, 0x5800, 0x5000, 0x540A, 0xE1, 0x7200 ), "15   Woods 2" },
			{ ( 0x0E, 0x5E00, 0x5E00, 0x560A, 0x63, 0x7600 ), "16   Treasure" },
			{ ( 0x0E, 0x6400, 0x5000, 0x580A, 0xE1, 0x7200 ), "17   Ship" },
			{ ( 0x03, 0x4C00, 0x5A00, 0x440A, 0xE1, 0x7200 ), "18   Castle 1" },
			{ ( 0x0E, 0x7000, 0x5000, 0x660A, 0xE1, 0x7200 ), "19   Castle 2" },
			{ ( 0x0E, 0x7000, 0x5000, 0x680A, 0xE1, 0x7200 ), "20   Castle 3" },
			{ ( 0x0E, 0x7600, 0x5200, 0x6C0A, 0xE1, 0x7200 ), "21   Castle 4" },
			{ ( 0x1D, 0x5200, 0x6600, 0x5E0A, 0xE1, 0x7200 ), "22   Boss 1" },
			{ ( 0x1D, 0x4600, 0x6200, 0x5E0A, 0xE1, 0x7200 ), "23   Boss 2" },
			{ ( 0x1D, 0x4C00, 0x6400, 0x5E0A, 0xE1, 0x7200 ), "24   Boss 3" },
			{ ( 0x1D, 0x5E00, 0x6A00, 0x600A, 0xD2, 0x7400 ), "25   Boss 4" },
			{ ( 0x1D, 0x6400, 0x6C00, 0x600A, 0xE4, 0x7000 ), "26   Boss 5" },
			{ ( 0x1D, 0x5800, 0x6800, 0x600A, 0xE1, 0x7200 ), "27   Boss 6" },
			{ ( 0x1D, 0x6A00, 0x6E00, 0x6A0A, 0xE1, 0x7200 ), "28   Boss 7" }
		};

		readonly int[][] enemyPointer =
		{
			//pointers referencing similar code are grouped together
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

		readonly ComboboxItemCollection<int> tilesAnimation = new ComboboxItemCollection<int>
		{
			{ 0x4000, "00   Platform" },
			{ 0x4100, "01   Sand / plant" },
			{ 0x4800, "02   Water" },
			{ 0x4A00, "03   Water / plant" },
			{ 0x4400, "04   Waterfall" },
			{ 0x4600, "05   Water flow" },
			{ 0x4E00, "06   Treasure room" },
			{ 0x4500, "07   Lava 1" },
			{ 0x4900, "08   Lava 2" },
			{ 0x4200, "09   Lava 3" },
			{ 0x4C00, "10   Piranha" },
			{ 0x4700, "11   Train left" },
			{ 0x4F00, "13   Train right" },
			{ 0x4B00, "14   Conveyor belt" },
			{ 0x4D00, "15   Castle" }
		};

		readonly ComboboxItemCollection<int> music = new ComboboxItemCollection<int>
		{
			{ 0x7ED5, "00   Beach 1" },
			{ 0x7F29, "01   Beach 2" },
			{ 0x7EE1, "02   Cave 1" },
			{ 0x7EED, "03   Cave 2" },
			{ 0x7F11, "04   Cave 3" },
			{ 0x7F1D, "05   Cave 4" },
			{ 0x7F35, "06   Cave 5" },
			{ 0x7F4D, "07   Cave 6" },
			{ 0x7EC9, "08   Lake" },
			{ 0x7EF9, "09   Train" },
			{ 0x7F05, "10   Boss Level" },
			{ 0x7F41, "11   Syrup Castle" },
			{ 0x7F59, "12   Sherbet Land" }
		};

		readonly ComboboxItemCollection<int> animationSpeed = new ComboboxItemCollection<int>
		{
			{ 0x00, "None" },
			{ 0x1F, "Slow" },
			{ 0x0F, "Normal" },
			{ 0x07, "Fast" },
			{ 0x03, "Fastest" }
		};

		readonly ComboboxItemCollection<int> cameraTypes = new ComboboxItemCollection<int>
		{
			{ 0x00, "X scroll" },
			{ 0x10, "X/Y scroll" },
			{ 0x01, "Train bumps" },
			{ 0x30, "Train auto-scroll right" },
			{ 0x31, "Train auto-scroll left" },
			{ 0xFF, "No scroll (boss fight)" }
		};

		readonly ComboboxItemCollection<int> warioStatus = new ComboboxItemCollection<int>
		{
			{ 0x00, "Standing" },
			{ 0x01, "Swimming" },
		};

		readonly ComboboxItemCollection<int> warioAttributes = new ComboboxItemCollection<int>
		{
			{ 0x00, "Left facing" },
			{ 0x20, "Right facing" },
			{ 0xA0, "Behind background" },
		};

		readonly ComboboxItemCollection<int> warps = new ComboboxItemCollection<int>
		{
			{ 0x5B76, "None" },
			{ 0x5B77, "Exit map" },
			{ 0x5B78, "Exit A" },
			{ 0x5B79, "Exit B" },
			{ 0x5B7A, "Sector" }
		};

		void DdlEnemiesDrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index >= 0 && !DesignMode)
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

				var item = ((ComboboxItem<EnemyInfo>)ddlEnemies.Items[e.Index]).Value;

				DrawBackground();
				DrawSprites();
				DrawText();

				void DrawBackground()
				{
					e.DrawBackground();

					Rectangle rect = new Rectangle(e.Bounds.Left, e.Bounds.Top, 20, e.Bounds.Height);
					if (item.BossId >= 0)
					{
						e.Graphics.FillRectangle(Brushes.MistyRose, rect);
					}
					else if (item.TreasureId >= 1 && item.TreasureId <= 15)
					{
						e.Graphics.FillRectangle(Brushes.Wheat, rect);
					}
					else
					{
						e.Graphics.FillRectangle(Brushes.White, rect);
					}
				}

				void DrawSprites()
				{
					for (int index = 0; index < 6; index++)
					{
						var enemyRect = enemiesRects[item.Index * 6 + index];
						if (enemyRect != default)
						{
							var destRect = new Rectangle(e.Bounds.Left + 20 + index * 32 * zoom, e.Bounds.Top, 32 * zoom, 32 * zoom);

							if ((item.ExitOpen && item.EnemyIds[index] == 15) //skull open
								|| item.EnemyIds[index] == 10 //moving pouncer
								|| item.EnemyIds[index] == 71) //guragura with coin
							{
								using (var darkBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 0)))
								{
									e.Graphics.FillRectangle(darkBrush, destRect);
								}
							}

							var imgRect = new Rectangle(index * 32, item.Index * 32, 32, 32);

							e.Graphics.DrawImage(TilesEnemies.Bitmap, destRect, imgRect, GraphicsUnit.Pixel);
						}
					}
				}

				void DrawText()
				{
					using (var format = new StringFormat())
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Center;
						var dest = new Rectangle(10, e.Bounds.Top + 16 * zoom, 0, 0);

						if (item.BossId >= 0)
						{
							e.Graphics.DrawString(string.Format($"B{item.BossId + 1}"), e.Font, Brushes.Black, dest, format);
						}
						else if (item.TreasureId >= 1 && item.TreasureId <= 15)
						{
							e.Graphics.DrawString(string.Format($"{treasureNames[item.TreasureId - 1]}"), e.Font, Brushes.Black, dest, format);
						}
						else
						{
							e.Graphics.DrawString(string.Format($"{e.Index:D2}"), e.Font, Brushes.Black, dest, format);
						}
					}
				}
			}
		}

		void InitForm()
		{
			if (!formLoaded)
			{
				formLoaded = true;
				ddlAnimationSpeed.Items.Clear();
				ddlAnimationSpeed.Items.AddRange(animationSpeed.ToArray());

				ddlCameraType.Items.Clear();
				ddlCameraType.Items.AddRange(cameraTypes.ToArray());

				ddlWarioStatus.Items.Clear();
				ddlWarioStatus.Items.AddRange(warioStatus.ToArray());

				ddlWarioAttributes.Items.Clear();
				ddlWarioAttributes.Items.AddRange(warioAttributes.ToArray());

				ddlMusic.Items.Clear();
				ddlMusic.Items.AddRange(music.ToArray());

				ddlWarp.Items.Clear();
				ddlWarp.Items.AddRange(warps.ToArray());

				ddlTileSet.Items.Clear();
				ddlTileSet.Items.AddRange(tileSets.ToArray());

				ddlAnimation.Items.Clear();
				ddlAnimation.Items.AddRange(tilesAnimation.ToArray());

				Array.Clear(TilesEnemies.Bits, 0, TilesEnemies.Bits.Length);
				ddlEnemies.Items.Clear();
				ddlEnemies.Items.AddRange(enemyPointer.Select((x, i) => new ComboboxItem<EnemyInfo>(GetEnemyInfo(i), string.Empty))
					.OrderBy(x => x.Value.BossId)
					.ThenBy(x => (x.Value.TreasureId >= 1 && x.Value.TreasureId <= 15) ? treasureNames[x.Value.TreasureId - 1] : 0)
					.ThenBy(x => x.Value.EnemyIds[0])
					.ThenBy(x => x.Value.EnemyIds[1])
					.ThenBy(x => x.Value.EnemyIds[2])
					.ThenBy(x => x.Value.EnemyIds[3])
					.ThenBy(x => x.Value.EnemyIds[4])
					.ThenBy(x => x.Value.EnemyIds[5])
					.ToArray());
			}

			EnemyInfo GetEnemyInfo(int index)
			{
				int[] enemyPointers = enemyPointer[index];

				var (enemiesIdsPointer, tilesPointer, treasureId, exitOpen) = Sprite.FindEnemiesData(rom, enemyPointers[0]);
				var enemyIds = Sprite.DumpEnemiesSprites(rom, enemiesIdsPointer, tilesPointer, TilesEnemies, index * 32, enemiesRects, index * 6, 32);

				return new EnemyInfo
				{
					EnemyPointers = enemyPointers,
					EnemyIds = enemyIds,
					Index = index,
					BossId = Array.IndexOf(boss, enemyPointers[0]),
					TreasureId = treasureId,
					ExitOpen = exitOpen
				};
			}
		}

		#endregion

		void SetControlsVisibility()
		{
			flowLayoutPanel1.SuspendLayout();

			panelWarp.Visible = currentSector != -1;
			panelMusic.Visible = currentSector == -1 && currentTreasureId == -1 && !currentCheckPoint;
			panelStatus.Visible = currentSector == -1 && currentTreasureId == -1;
			grpTileset.Visible = grpCamera.Visible = currentSector == -1 || Sector.GetWarp(rom, currentCourseId, currentSector) >= 0x5B7A;

			flowLayoutPanel1.ResumeLayout();
		}

		void LoadWarpTypeDropdown()
		{
			ignoreEvents = true;

			if (currentTreasureId != -1)
			{
				ddlWarpType.SelectedIndex = 34 + treasureNames[currentTreasureId] - 'A';
			}
			else if (currentSector != -1)
			{
				ddlWarpType.SelectedIndex = 2 + currentSector;
			}
			else if (currentCheckPoint)
			{
				ddlWarpType.SelectedIndex = 1;
			}
			else
			{
				ddlWarpType.SelectedIndex = 0;
			}
			ignoreEvents = false;
		}

		#region Load

		public void LoadRom(Rom rom)
		{
			this.rom = rom;
			formLoaded = false;
		}

		void SectorFormVisibleChanged(object sender, EventArgs e)
		{
			if (Visible && rom != null)
			{
				InitForm();
				LoadSector();
			}
		}

		public void SetZoom(int zoomLevel)
		{
			zoom = Math.Min(2, zoomLevel);
			ddlEnemies.DropDownWidth = 20 + zoom * 32 * 5 + SystemInformation.VerticalScrollBarWidth;
			ddlEnemies.ItemHeight = zoom * 32;
			labEnemies.Height = zoom * 32;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			KeyEventArgs args = new KeyEventArgs(keyData);

			ProcessCommandKey(this, args);
			if (args.Handled)
			{
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		void SectorFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
			}
		}

		public void LoadSector(int course, int sector, int treasureId, bool checkPoint)
		{
			currentCourseId = course;
			currentSector = sector;
			currentTreasureId = treasureId;
			currentCheckPoint = checkPoint;

			if (Visible)
			{
				InitForm();
				LoadSector();
			}
		}

		void LoadSector()
		{
			if (currentTreasureId != -1)
			{
				int warp = Sector.GetTreasureWarp(rom, currentTreasureId);
				LoadWarp(warp);
				LoadScroll();
			}
			else if (currentCourseId != -1 && currentSector != -1)
			{
				int warp = Sector.GetWarp(rom, currentCourseId, currentSector);
				LoadWarp(warp);
				LoadScroll();
			}
			else if (currentCourseId != -1)
			{
				LoadLevel();
				LoadDropdown(ddlMusic, Sector.GetMusic(rom, currentCourseId));
				LoadCheckpoint();
			}

			SetControlsVisibility();
			LoadWarpTypeDropdown();

			void LoadLevel()
			{
				currentWarp = Sector.GetLevelHeader(rom, currentCourseId, currentCheckPoint);
				LoadWarp();
			}

			void LoadScroll()
			{
				scroll = Sector.GetScroll(rom, currentCourseId, currentSector);
				LoadCheckBox(checkBoxLeft, (scroll & 2) == 2);
				LoadCheckBox(checkBoxRight, (scroll & 1) == 1);
			}

			void LoadCheckpoint()
			{
				bool hasCheckpoint = Sector.GetLevelHeader(rom, currentCourseId) != Sector.GetCheckpoint(rom, currentCourseId);
				LoadCheckBox(checkBoxCheckpoint, hasCheckpoint);
			}
		}

		void LoadWarp(int warp)
		{
			LoadDropdown(ddlWarp, Math.Min(warp, 0x5B7A));
			currentWarp = Sector.GetWarp(rom, warp);
			LoadWarp();
		}

		void LoadWarp()
		{
			LoadDropdown(ddlTileSet,
			(
				currentWarp.Bank,
				currentWarp.TileSetB,
				currentWarp.TileSetA,
				currentWarp.BlockIndex,
				currentWarp.Palette,
				currentWarp.GUI
			));
			LoadDropdown(ddlAnimation, currentWarp.TileAnimation);
			LoadDropdown(ddlAnimationSpeed, currentWarp.AnimationSpeed);
			LoadDropdownAny<EnemyInfo>(ddlEnemies, currentWarp.Enemy, x => x.EnemyPointers);

			LoadDropdown(ddlCameraType, currentWarp.CameraType);
			LoadNumericUpDown(txbCameraX, currentWarp.CameraX);
			LoadNumericUpDown(txbCameraY, currentWarp.CameraY);

			LoadNumericUpDown(txbWarioX, currentWarp.WarioX);
			LoadNumericUpDown(txbWarioY, currentWarp.WarioY);

			if (currentSector == -1 && currentTreasureId == -1)
			{
				LoadDropdown(ddlWarioStatus, currentWarp.WarioStatus);
				LoadDropdown(ddlWarioAttributes, currentWarp.WarioSpriteAttributes);
			}

			void LoadNumericUpDown(NumericUpDown numericUpDown, int value)
			{
				ignoreEvents = true;
				numericUpDown.Value = Math.Min(Math.Max(value, (int)numericUpDown.Minimum), (int)numericUpDown.Maximum);
				ignoreEvents = false;
			}

			void LoadDropdownAny<T>(ComboBox combo, int value, Func<T, int[]> filter)
			{
				ignoreEvents = true;
				combo.SelectedIndex = combo.Items.Cast<ComboboxItem<T>>().FindIndex(x => filter(x.Value).Contains(value));
				ignoreEvents = false;
			}
		}

		void LoadDropdown<T>(ComboBox combo, T value)
		{
			ignoreEvents = true;
			combo.SelectedIndex = combo.Items.Cast<ComboboxItem<T>>().FindIndex(x => x.Value.Equals(value));
			ignoreEvents = false;
		}

		void LoadCheckBox(CheckBox checkBox, bool value)
		{
			ignoreEvents = true;
			checkBox.Checked = value;
			ignoreEvents = false;
		}

		#endregion

		#region Save

		void CheckBoxLeftCheckedChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				scroll = (scroll & ~0x2) | (checkBoxLeft.Checked ? 0x2 : 0);
				SaveScroll();
				OnSectorChanged();
			}
		}

		void CheckBoxRightCheckedChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				scroll = (scroll & ~0x1) | (checkBoxRight.Checked ? 0x1 : 0);
				SaveScroll();
				OnSectorChanged();
			}
		}

		void SaveScroll()
		{
			Sector.SaveScroll(rom, currentCourseId, currentSector, scroll);
			Sector.SaveLevelScroll(rom, currentCourseId, false, currentSector, scroll);
			Sector.SaveLevelScroll(rom, currentCourseId, true, currentSector, scroll);
		}

		void DdlWarpSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlWarp.SelectedItem;
				int warp = item.Value;

				if (warp == 0x5B7A)
				{
					int previousWarp = Sector.GetWarp(rom, currentCourseId, currentSector);
					if (previousWarp >= 0x5B7A)
					{
						warp = previousWarp;
					}
					else
					{
						warp = GetFreeWarp();
						if (warp == -1)
						{
							MessageBox.Show("No more warps available.\r\nPlease free a warp in another sector (any level)", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							LoadDropdown(ddlWarp, previousWarp);
							return;
						}
					}
				}

				Sector.SaveWarp(rom, currentCourseId, currentSector, warp);
				LoadWarp(warp);

				OnSectorChanged();
				SetControlsVisibility();
			}

			int GetFreeWarp()
			{
				var used = Sector.GetWarpUsage(rom);
				var all = Enumerable.Range(0, 370)
					.Select(x => 0x5B7A + x * 24);

				return all
					.Except(used)
					.DefaultIfEmpty(-1)
					.First();
			}
		}

		void CheckBoxCheckpoint_CheckedChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				if (checkBoxCheckpoint.Checked)
				{
					int header = GetFreeLevelHeader();
					if (header == -1)
					{
						MessageBox.Show("No more checkpoint headers.\r\nPlease free a checkpoint in another level", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						LoadCheckBox(checkBoxCheckpoint, false);
						return;
					}

					Sector.SaveCheckpoint(rom, currentCourseId, header);
				}
				else
				{
					Sector.SaveCheckpoint(rom, currentCourseId, Sector.GetLevelHeader(rom, currentCourseId));
				}
			}

			int GetFreeLevelHeader()
			{
				int header = Sector.GetLevelHeader(rom, currentCourseId);
				var used = Sector.GetLevelHeaderUsage(rom);
				var all = Enumerable.Range(0, 77)
					.Select(x => 0x460C + x * 30);

				return all
					.Except(used)
					.OrderByDescending(x => x == (header + 30))
					.DefaultIfEmpty(-1)
					.First();
			}
		}

		void DdlTileSetSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<(int Bank, int TileSetB, int TileSetA, int BlockIndex, int Palette, int GUI)>)ddlTileSet.SelectedItem;
				currentWarp.Bank = item.Value.Bank;
				currentWarp.TileSetB = item.Value.TileSetB;
				currentWarp.TileSetA = item.Value.TileSetA;
				currentWarp.GUI = item.Value.GUI;
				currentWarp.BlockIndex = item.Value.BlockIndex;
				currentWarp.Palette = item.Value.Palette;

				SaveWarp();
				OnSectorChanged();
			}
		}

		void DdlAnimationSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlAnimation.SelectedItem;
				currentWarp.TileAnimation = item.Value;

				SaveWarp();
				OnSectorChanged();
			}
		}

		void DdlAnimationSpeedSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlAnimationSpeed.SelectedItem;
				currentWarp.AnimationSpeed = item.Value;

				SaveWarp();
				OnSectorChanged();
			}
		}

		void DdlEnemiesSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<EnemyInfo>)ddlEnemies.SelectedItem;
				currentWarp.Enemy = item.Value.EnemyPointers[0];

				SaveWarp();
				OnSectorChanged();
			}
		}

		void TxbWarioXValueChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				currentWarp.WarioX = (int)txbWarioX.Value;

				SaveWarp();
				OnSectorChanged();
			}
		}

		void TxbWarioYValueChanged(object sender, EventArgs e)
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
				Sector.SaveMusic(rom, currentCourseId, item.Value);
			}
		}

		void DdlWarioStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlWarioStatus.SelectedItem;
				currentWarp.WarioStatus = item.Value;

				SaveWarp();
				OnSectorChanged();
			}
		}

		void DdlWarioAttributes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlWarioAttributes.SelectedItem;
				currentWarp.WarioSpriteAttributes = item.Value;

				SaveWarp();
				OnSectorChanged();
			}
		}

		void OnSectorChanged()
		{
			SectorChanged?.Invoke(this, EventArgs.Empty);
		}

		void SaveWarp()
		{
			if (currentTreasureId != -1)
			{
				int warp = Sector.GetTreasureWarp(rom, currentTreasureId);
				Sector.SaveWarp(rom, warp, currentWarp);
			}
			else if (currentSector != -1)
			{
				int warp = Sector.GetWarp(rom, currentCourseId, currentSector);
				Sector.SaveWarp(rom, warp, currentWarp);
			}
			else
			{
				int warioSector = currentWarp.WarioX / 32 + currentWarp.WarioY / 32 * 16;
				currentWarp.Scroll = Sector.GetScroll(rom, currentCourseId, warioSector);

				Sector.SaveLevelHeader(rom, currentCourseId, currentCheckPoint, currentWarp);
			}
		}

		private void DdlWarpType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				if (ddlWarpType.SelectedIndex == 0)
				{
					SectorLoad(sender, (-1, false, -1));
				}
				else if (ddlWarpType.SelectedIndex == 1)
				{
					bool hasCheckpoint = Sector.GetLevelHeader(rom, currentCourseId) != Sector.GetCheckpoint(rom, currentCourseId);
					if (hasCheckpoint)
					{
						SectorLoad(sender, (-1, true, -1));
					}
					else
					{
						MessageBox.Show("Checkpoint is not enabled in this level", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						LoadWarpTypeDropdown();
					}
				}
				else if (ddlWarpType.SelectedIndex < 34)
				{
					SectorLoad(sender, (ddlWarpType.SelectedIndex - 2, false, -1));
				}
				else
				{
					SectorLoad(sender, (-1, false, Array.IndexOf(treasureNames, (char)('A' + ddlWarpType.SelectedIndex - 34))));
				}
			}
		}

		#endregion

		#region Calculate

		void CmdCalculatePosClick(object sender, EventArgs e)
		{
			int sector = currentWarp.WarioX / 32 + currentWarp.WarioY / 32 * 16;
			string sectorTxt = sector.ToString();

			do
			{
				sectorTxt = Interaction.InputBox(string.Empty, "Enter sector number (0-31)", sectorTxt, -1, -1);
			}
			while (!(sectorTxt == string.Empty || (int.TryParse(sectorTxt, out sector) && sector >= 0 && sector <= 31)));

			if (sectorTxt != string.Empty)
			{
				int doorX, doorY;
				if (currentTreasureId != -1)
				{
					var item = (ComboboxItem<EnemyInfo>)ddlEnemies.SelectedItem;
					(doorX, doorY) = Sector.FindObject(sector, currentWarp.WarioX, currentWarp.WarioY, item.Value.EnemyIds, 22); //treasure opening
					if ((doorX, doorY) != (-1, -1))
					{
						doorX--;
					}
				}
				else if (currentCheckPoint)
				{
					var item = (ComboboxItem<EnemyInfo>)ddlEnemies.SelectedItem;
					(doorX, doorY) = Sector.FindObject(sector, currentWarp.WarioX, currentWarp.WarioY, item.Value.EnemyIds, 21); //checkpoint
					if ((doorX, doorY) != (-1, -1))
					{
						doorY--;
					}
				}
				else
				{
					(doorX, doorY) = Sector.FindDoor(sector, currentWarp.WarioX, currentWarp.WarioY);
				}

				//player position
				if ((doorX, doorY) != (-1, -1))
				{
					currentWarp.WarioX = (sector % 16) * 32 + Math.Min(doorX * 2 + 1, 31);
					currentWarp.WarioY = (sector / 16) * 32 + Math.Min(doorY * 2 + 2, 31);
				}
				else
				{
					currentWarp.WarioX = (sector % 16) * 32 + currentWarp.WarioX % 32;
					currentWarp.WarioY = (sector / 16) * 32 + currentWarp.WarioY % 32;
				}

				//camera position (scroll area: 10 x 9)
				int cameraX = currentWarp.WarioX - 12;
				int cameraY = currentWarp.WarioY - 16;

				currentWarp.CameraX = Sector.LimitCameraX(rom, currentCourseId, sector, cameraX);
				(currentWarp.CameraY, currentWarp.WarioY) = Sector.LimitCameraY(currentWarp.CameraType, cameraY, currentWarp.WarioY);

				SaveWarp();
				LoadWarp();
				OnSectorChanged();
			}

		}

		#endregion
	}
}
