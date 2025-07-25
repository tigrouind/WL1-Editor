using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class SectorForm : Form
	{
		public event EventHandler<KeyEventArgs> ProcessCommandKey;
		public event EventHandler SectorChanged;
		public event EventHandler<(int Sector, bool Checkpoint, int TreasureId)> SectorLoad;

		public readonly DirectBitmap TilesEnemies = new(32 * 6, 32 * 147);
		public readonly (Rectangle Rectangle, Point Point)[] enemiesRects = new (Rectangle, Point)[6 * 147];
		readonly char[] treasureNames = ['C', 'I', 'F', 'O', 'A', 'N', 'H', 'M', 'L', 'K', 'B', 'D', 'G', 'J', 'E'];
		readonly int[] boss = [0x4CA9, 0x460D, 0x4C0C, 0x4E34, 0x4B06, 0x4D1A, 0x527D];
		public PictureBox PictureBox => pictureBox;

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

		readonly ComboboxItemCollection<(int Bank, int TileSetB, int TileSetA, int BlockIndex, int Palette, int GUI)> tileSets = new()
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
		[
			//pointers referencing similar code are grouped together
			[0x41EF],
			[0x4208],
			[0x4219],
			[0x422A],
			[0x423B],
			[0x4254],
			[0x42A9, 0x42BA, 0x42FE, 0x4927, 0x4986, 0x49CA, 0x4BD9, 0x4D61, 0x5028, 0x5096, 0x5228],
			[0x42CB, 0x4997, 0x49B9],
			[0x42DC, 0x4464, 0x44A8, 0x45EB, 0x47B3, 0x4818, 0x4B3B, 0x4B6E, 0x4C32, 0x4E23, 0x4EEA, 0x4F60, 0x4F71, 0x4FAC, 0x506C, 0x5158, 0x51BA, 0x51F5, 0x5217, 0x52E5, 0x4D09],
			[0x42ED],
			[0x430F, 0x49DB],
			[0x4325, 0x4E95],
			[0x4336],
			[0x4347],
			[0x4358, 0x437A, 0x4402, 0x4413, 0x46A0, 0x486D, 0x4DE8],
			[0x4369, 0x4520, 0x46B1, 0x4807, 0x4FF5, 0x5147],
			[0x438B],
			[0x43A4],
			[0x43C2],
			[0x43D8],
			[0x43F1],
			[0x4424],
			[0x443D],
			[0x4453],
			[0x4475],
			[0x4486],
			[0x4497, 0x4B7F],
			[0x44B9, 0x44E0, 0x4BA1, 0x4CF3],
			[0x44CF, 0x4B90, 0x4FD3, 0x4FE4],
			[0x44F6],
			[0x4507],
			[0x4531],
			[0x4542],
			[0x455B],
			[0x456C],
			[0x457D],
			[0x4596, 0x4A1B],
			[0x45A7],
			[0x45B8, 0x467E, 0x46D3, 0x4975, 0x4D9C, 0x50A7],
			[0x45C9, 0x50EB],
			[0x45DA],
			[0x45FC],
			[0x460D],
			[0x464C],
			[0x4665],
			[0x468F],
			[0x46C2, 0x4778, 0x4C87, 0x4C98, 0x4D72, 0x5039, 0x504A, 0x50DA],
			[0x46E4],
			[0x46FD],
			[0x4716],
			[0x4727],
			[0x473D],
			[0x4756],
			[0x4767],
			[0x4789],
			[0x47A2],
			[0x47C4],
			[0x47DD],
			[0x47F6],
			[0x4829, 0x5169],
			[0x483A],
			[0x484B],
			[0x485C],
			[0x487E],
			[0x488F],
			[0x48A0],
			[0x48B1],
			[0x48C2],
			[0x48D3],
			[0x48EC],
			[0x48FD],
			[0x4916],
			[0x4938],
			[0x494E],
			[0x495F],
			[0x49A8],
			[0x49F1],
			[0x4A0A],
			[0x4A2C],
			[0x4A3D],
			[0x4A56],
			[0x4A6F],
			[0x4A88],
			[0x4AA1],
			[0x4AB2],
			[0x4AC3],
			[0x4ADC],
			[0x4AED],
			[0x4B06],
			[0x4B2A],
			[0x4B4C],
			[0x4B5D],
			[0x4BB7],
			[0x4BC8],
			[0x4BEA],
			[0x4BFB, 0x4F25, 0x4F4F],
			[0x4C0C],
			[0x4C43],
			[0x4C54],
			[0x4C65],
			[0x4C76],
			[0x4CA9],
			[0x4CE2],
			[0x4D1A],
			[0x4D48],
			[0x4D83, 0x4DAD],
			[0x4DC6],
			[0x4DD7],
			[0x4DF9],
			[0x4E12],
			[0x4E34],
			[0x4E62],
			[0x4E73],
			[0x4E84],
			[0x4EA6],
			[0x4EB7],
			[0x4EC8],
			[0x4ED9],
			[0x4EFB],
			[0x4F14],
			[0x4F36],
			[0x4F82],
			[0x4F9B],
			[0x4FBD],
			[0x5006],
			[0x5017],
			[0x505B],
			[0x507D],
			[0x50B8, 0x526C],
			[0x50C9],
			[0x50FC],
			[0x5115],
			[0x512E],
			[0x517A],
			[0x5190],
			[0x51A9],
			[0x51CB],
			[0x51E4],
			[0x5206],
			[0x5239],
			[0x524A],
			[0x525B],
			[0x527D],
			[0x52AA],
			[0x52BB],
			[0x52CC],
			[0x52F6]
		];

		readonly ComboboxItemCollection<int> tilesAnimation = new()
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

		readonly ComboboxItemCollection<int> music = new()
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

		readonly ComboboxItemCollection<int> animationSpeed = new()
		{
			{ 0x00, "None" },
			{ 0x1F, "Slow" },
			{ 0x0F, "Normal" },
			{ 0x07, "Fast" },
			{ 0x03, "Fastest" }
		};

		readonly ComboboxItemCollection<int> cameraTypes = new()
		{
			{ 0x00, "X scroll" },
			{ 0x10, "X/Y scroll" },
			{ 0x01, "Train bumps" },
			{ 0x30, "Train auto-scroll right" },
			{ 0x31, "Train auto-scroll left" },
			{ 0xFF, "No scroll (boss fight)" }
		};

		readonly ComboboxItemCollection<int> warioStatus = new()
		{
			{ 0x00, "Standing" },
			{ 0x01, "Swimming" },
		};

		readonly ComboboxItemCollection<int> warioAttributesLevel = new()
		{
			{ 0x00, "Left facing" },
			{ 0x20, "Right facing" },
			{ 0xA0, "Behind background" },
		};

		readonly ComboboxItemCollection<int> warioAttributesSector = new()
		{
			{ 0x00, "Normal" },
			{ 0x01, "Behind background" },
		};

		readonly ComboboxItemCollection<int> warps = new()
		{
			{ 0x5B76, "None" },
			{ 0x5B77, "Exit map" },
			{ 0x5B78, "Exit A" },
			{ 0x5B79, "Exit B" },
			{ 0x5B7A, "Sector" }
		};

		readonly ComboboxItemCollection<(string, int)> warpTypes = new()
		{
			{ ("H", 0), "Level header" },
			{ ("C", 0), "Checkpoint" },
			{ ("S", 0), "Sector 00" },
			{ ("S", 1), "Sector 01" },
			{ ("S", 2), "Sector 02" },
			{ ("S", 3), "Sector 03" },
			{ ("S", 4), "Sector 04" },
			{ ("S", 5), "Sector 05" },
			{ ("S", 6), "Sector 06" },
			{ ("S", 7), "Sector 07" },
			{ ("S", 8), "Sector 08" },
			{ ("S", 9), "Sector 09" },
			{ ("S", 10), "Sector 10" },
			{ ("S", 11), "Sector 11" },
			{ ("S", 12), "Sector 12" },
			{ ("S", 13), "Sector 13" },
			{ ("S", 14), "Sector 14" },
			{ ("S", 15), "Sector 15" },
			{ ("S", 16), "Sector 16" },
			{ ("S", 17), "Sector 17" },
			{ ("S", 18), "Sector 18" },
			{ ("S", 19), "Sector 19" },
			{ ("S", 20), "Sector 20" },
			{ ("S", 21), "Sector 21" },
			{ ("S", 22), "Sector 22" },
			{ ("S", 23), "Sector 23" },
			{ ("S", 24), "Sector 24" },
			{ ("S", 25), "Sector 25" },
			{ ("S", 26), "Sector 26" },
			{ ("S", 27), "Sector 27" },
			{ ("S", 28), "Sector 28" },
			{ ("S", 29), "Sector 29" },
			{ ("S", 30), "Sector 30" },
			{ ("S", 31), "Sector 31" },
			{ ("T", 0), "Treasure A" },
			{ ("T", 1), "Treasure B" },
			{ ("T", 2), "Treasure C" },
			{ ("T", 3), "Treasure D" },
			{ ("T", 4), "Treasure E" },
			{ ("T", 5), "Treasure F" },
			{ ("T", 6), "Treasure G" },
			{ ("T", 7), "Treasure H" },
			{ ("T", 8), "Treasure I" },
			{ ("T", 9), "Treasure J" },
			{ ("T", 10), "Treasure K" },
			{ ("T", 11), "Treasure L" },
			{ ("T", 12), "Treasure M" },
			{ ("T", 13), "Treasure N" },
			{ ("T", 14), "Treasure O" },
		};

		void DdlEnemiesDrawItem(object sender, DrawItemEventArgs e)
		{
			if (!DesignMode && e.Index >= 0)
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

				bool selected = (e.State & DrawItemState.Selected) != 0;
				var item = ((ComboboxItem<EnemyInfo>)ddlEnemies.Items[e.Index]).Value;

				DrawBackground();
				DrawSprites();
				DrawText();

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
								using var darkBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 0));
								e.Graphics.FillRectangle(darkBrush, destRect);
							}

							var imgRect = new Rectangle(index * 32, item.Index * 32, 32, 32);

							e.Graphics.DrawImage(TilesEnemies.Bitmap, destRect, imgRect, GraphicsUnit.Pixel);
						}
					}
				}

				void DrawBackground()
				{
					e.DrawBackground();

					if (!selected)
					{
						Brush brush = null;
						if (item.BossId >= 0)
						{
							brush = Brushes.MistyRose;
						}
						else if (item.TreasureId >= 1 && item.TreasureId <= 15)
						{
							brush = Brushes.Wheat;
						}

						if (brush != null)
						{
							e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.Left, e.Bounds.Top, 20, e.Bounds.Height));
						}
					}
				}

				void DrawText()
				{
					string text = $"{e.Index:D2}";
					if (item.BossId >= 0)
					{
						text = $"B{item.BossId + 1}";
					}
					else if (item.TreasureId >= 1 && item.TreasureId <= 15)
					{
						text = $"{treasureNames[item.TreasureId - 1]}";
					}

					using var format = new StringFormat();
					using var textBrush = new SolidBrush(e.ForeColor);
					format.Alignment = StringAlignment.Center;
					format.LineAlignment = StringAlignment.Center;

					e.Graphics.DrawString(text, e.Font, textBrush,
						new Rectangle(10, e.Bounds.Top + 16 * zoom, 0, 0), format);
				}
			}
		}

		void InitForm()
		{
			if (!formLoaded)
			{
				formLoaded = true;
				ddlAnimationSpeed.Items.Clear();
				ddlAnimationSpeed.Items.AddRange([.. animationSpeed]);

				ddlCameraType.Items.Clear();
				ddlCameraType.Items.AddRange([.. cameraTypes]);

				ddlWarioStatus.Items.Clear();
				ddlWarioStatus.Items.AddRange([.. warioStatus]);

				ddlWarioAttributesLevel.Items.Clear();
				ddlWarioAttributesLevel.Items.AddRange([.. warioAttributesLevel]);

				ddlWarioAttributesSector.Items.Clear();
				ddlWarioAttributesSector.Items.AddRange([.. warioAttributesSector]);

				ddlMusic.Items.Clear();
				ddlMusic.Items.AddRange([.. music]);

				ddlWarp.Items.Clear();
				ddlWarp.Items.AddRange([.. warps]);

				ddlTileSet.Items.Clear();
				ddlTileSet.Items.AddRange([.. tileSets]);

				ddlAnimation.Items.Clear();
				ddlAnimation.Items.AddRange([.. tilesAnimation]);

				Array.Clear(TilesEnemies.Bits, 0, TilesEnemies.Bits.Length);
				ddlEnemies.Items.Clear();
				ddlEnemies.Items.AddRange([.. enemyPointer.Select((x, i) => new ComboboxItem<EnemyInfo>(GetEnemyInfo(i), string.Empty))
					.OrderBy(x => x.Value.BossId)
					.ThenBy(x => (x.Value.TreasureId >= 1 && x.Value.TreasureId <= 15) ? treasureNames[x.Value.TreasureId - 1] : 0)
					.ThenBy(x => x.Value.EnemyIds[0])
					.ThenBy(x => x.Value.EnemyIds[1])
					.ThenBy(x => x.Value.EnemyIds[2])
					.ThenBy(x => x.Value.EnemyIds[3])
					.ThenBy(x => x.Value.EnemyIds[4])
					.ThenBy(x => x.Value.EnemyIds[5])]);
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

			bool validWarp = currentSector != -1 && Sector.GetWarp(rom, currentCourseId, currentSector) >= 0x5B7A;
			panelScroll.Visible = panelWarp.Visible = currentSector != -1;
			panelCheckpoint.Visible = panelMusic.Visible = currentSector == -1 && currentTreasureId == -1 && !currentCheckPoint;
			panelStatusLevel.Visible = currentSector == -1 && currentTreasureId == -1;
			panelStatusSector.Visible = validWarp || currentTreasureId != -1;
			panelEnemy.Visible = panelTileSet.Visible = panelCamera.Visible = currentSector == -1 || validWarp;

			flowLayoutPanel1.ResumeLayout();
		}

		void LoadWarpTypeDropdown()
		{
			ignoreEvents = true;

			ddlWarpType.Items.Clear();
			ddlWarpType.Items.AddRange([.. warpTypes]);

			bool hasCheckpoint = Sector.GetLevelHeader(rom, currentCourseId) != Sector.GetCheckpoint(rom, currentCourseId);
			if (!hasCheckpoint)
			{
				ddlWarpType.Items.RemoveAt(1);
			}

			(string, int) selectedValue;
			if (currentTreasureId != -1)
			{
				selectedValue = ("T", treasureNames[currentTreasureId] - 'A');
			}
			else if (currentSector != -1)
			{
				selectedValue = ("S", currentSector);
			}
			else if (currentCheckPoint)
			{
				selectedValue = ("C", 0);
			}
			else
			{
				selectedValue = ("H", 0);
			}

			LoadDropdown(ddlWarpType, selectedValue);

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

			pictureBox.Size = new Size(zoom * 16 * 8, zoom * 6 * 8);
			pictureBox.Invalidate();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			KeyEventArgs args = new(keyData);

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
			pictureBox.Invalidate();

			LoadDropdown(ddlAnimation, currentWarp.TileAnimation);
			LoadDropdown(ddlAnimationSpeed, currentWarp.AnimationSpeed);
			LoadDropdownAny<EnemyInfo>(ddlEnemies, currentWarp.Enemy, x => x.EnemyPointers);

			LoadDropdown(ddlCameraType, currentWarp.CameraType);
			LoadNumericUpDown(txbCameraX, currentWarp.CameraX);
			LoadNumericUpDown(txbCameraY, currentWarp.CameraY);

			LoadNumericUpDown(txbWarioX, currentWarp.WarioX);
			LoadNumericUpDown(txbWarioY, currentWarp.WarioY);

			LoadDropdown(ddlWarioStatus, currentWarp.WarioStatus);
			LoadDropdown(ddlWarioAttributesLevel, currentWarp.WarioSpriteAttributes);
			LoadDropdown(ddlWarioAttributesSector, currentWarp.WarioSpriteAttributes);

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
				Setchanges();
			}
		}

		void CheckBoxRightCheckedChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				scroll = (scroll & ~0x1) | (checkBoxRight.Checked ? 0x1 : 0);
				SaveScroll();
				Setchanges();
			}
		}

		void SaveScroll()
		{
			Sector.SaveScroll(rom, currentCourseId, currentSector, scroll);
			Sector.SaveLevelScroll(rom, currentCourseId, currentSector, scroll, false);
			Sector.SaveLevelScroll(rom, currentCourseId, currentSector, scroll, true);
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
						warp = Sector.GetFreeWarp(rom);
						if (warp == -1)
						{
							MessageBox.Show("No more warps available.\r\nPlease free a warp in another sector (any level)", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							LoadDropdown(ddlWarp, previousWarp);
							return;
						}
					}
				}
				else
				{
					Sector.FreeWarp(rom, currentCourseId, currentSector);
				}

				Sector.SaveWarp(rom, currentCourseId, currentSector, warp);
				LoadWarp(warp);

				Setchanges();
				SetControlsVisibility();
			}
		}

		void CheckBoxCheckpoint_CheckedChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				if (checkBoxCheckpoint.Checked)
				{
					int header = Sector.GetFreeCheckpoint(rom, currentCourseId);
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
					Sector.FreeCheckpoint(rom, currentCourseId);
				}

				LoadWarpTypeDropdown();
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
				pictureBox.Invalidate();

				SaveWarp();
				Setchanges();
			}
		}

		void DdlAnimationSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlAnimation.SelectedItem;
				currentWarp.TileAnimation = item.Value;
				pictureBox.Invalidate();

				SaveWarp();
				Setchanges();
			}
		}

		void DdlAnimationSpeedSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlAnimationSpeed.SelectedItem;
				currentWarp.AnimationSpeed = item.Value;
				pictureBox.Invalidate();

				SaveWarp();
				Setchanges();
			}
		}

		void DdlEnemiesSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<EnemyInfo>)ddlEnemies.SelectedItem;
				currentWarp.Enemy = item.Value.EnemyPointers[0];

				SaveWarp();
				Setchanges();
			}
		}

		void TxbWarioXValueChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				currentWarp.WarioX = (int)txbWarioX.Value;

				SaveWarp();
				Setchanges();
			}
		}

		void TxbWarioYValueChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				currentWarp.WarioY = (int)txbWarioY.Value;

				SaveWarp();
				Setchanges();
			}
		}

		void TxbCameraXValueChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				currentWarp.CameraX = (int)txbCameraX.Value;

				SaveWarp();
				Setchanges();
			}
		}

		void TxbCameraYValueChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				currentWarp.CameraY = (int)txbCameraY.Value;

				SaveWarp();
				Setchanges();
			}
		}

		void DdlCameraTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlCameraType.SelectedItem;
				currentWarp.CameraType = item.Value;

				SaveWarp();
				Setchanges();
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
				Setchanges();
			}
		}

		void DdlWarioAttributesLevel_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlWarioAttributesLevel.SelectedItem;
				currentWarp.WarioSpriteAttributes = item.Value;

				SaveWarp();
				Setchanges();
			}
		}


		void DdlWarioAttributesSector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!ignoreEvents)
			{
				var item = (ComboboxItem<int>)ddlWarioAttributesSector.SelectedItem;
				currentWarp.WarioSpriteAttributes = item.Value;

				SaveWarp();
				Setchanges();
			}
		}

		void Setchanges()
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
				var selectedValue = (ComboboxItem<(string Type, int Index)>)ddlWarpType.Items[ddlWarpType.SelectedIndex];
				switch (selectedValue.Value.Type)
				{
					case "H":
						SectorLoad(sender, (-1, false, -1));
						break;

					case "C":
						SectorLoad(sender, (-1, true, -1));
						break;

					case "S":
						SectorLoad(sender, (selectedValue.Value.Index, false, -1));
						break;

					case "T":
						SectorLoad(sender, (-1, false, Array.IndexOf(treasureNames, (char)('A' + selectedValue.Value.Index))));
						break;
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
				Setchanges();
			}

		}

		#endregion

		private void PictureBoxTileSet_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.DrawImage(Level.Tiles8x8.Bitmap,
				new Rectangle(0, 0, 16 * 8 * zoom, 6 * 8 * zoom),
				new Rectangle(0, 2 * 8, 16 * 8, 6 * 8), GraphicsUnit.Pixel);
		}
	}
}
