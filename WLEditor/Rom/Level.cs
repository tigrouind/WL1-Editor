using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WLEditor
{
	public static class Level
	{
		readonly static uint[] paletteColors = { 0xFFFFFFFF, 0xFFAAAAAA, 0xFF555555, 0xFF000000 };

		public readonly static byte[] LevelData = new byte[0x3000];
		public readonly static byte[] ObjectsData = new byte[0x2000];
		public readonly static byte[] ScrollData = new byte[32];
		public readonly static byte[] Warps = new byte[32];
		public static int WarioPosition;
		public static int CameraPosition;
		public readonly static bool[] Animated16x16Tiles = new bool[16 * 8];
		private readonly static int[] animated8x8Tiles = new int[16 * 8 * 2 * 2];
		public static int AnimatedTilesMask;
		public static bool WarioRightFacing;

		public static bool ShowColliders = true;
		public static bool ShowCollectibles = true;
		public static int SwitchMode;
		public static int SwitchType;

		public readonly static DirectBitmap Tiles8x8 = new DirectBitmap(16 * 8, 8 * 8);
		public readonly static DirectBitmap Tiles16x16 = new DirectBitmap(16 * 8, 16 * 16);

		readonly static (int Type, string Text)[] tileInfo = new (int, string)[]
		{
			(0, "Bonus"),
			(0, "Block (dark background)"),
			(0, "Block (dark background)"),
			(0, "Block (light background)"),
			(0, "Block (light background)"),
			(0, "Block (top door)"),
			(6, "Block (bottom door)"),
			(0, "Jump"),
			(1, "Conveyor belt (backward)"),
			(1, "Conveyor belt (forward)"),
			(7, "[!] block"),
			(1, "Falling platform"),
			(2, "Ice"),
			(2, "Ice"),
			(2, "Ice"),
			(3, "Hidden bonus (platform)"),
			(7, "[!] block (unused)"),
			(7, "[!] block"),
			(1, "Disappearing platform"),
			(1, "Disappearing platform"),
			(0, ""),
			(0, ""),
			(3, "Falling sand"),
			(5, "Damage"),
			(1, "Platform"),
			(1, "Platform"),
			(1, "Platform"),
			(1, "Platform"),
			(4, "Ladder"),
			(4, "Ladder"),
			(3, "Coin"),
			(3, "Coin"),
			(6, "Bottom door"),
			(3, "Hidden bonus (fall through)"),
			(3, "Hidden bonus (fall through) (water)"),
			(6, "Bottom door (water)"),
			(2, "Water current (north)"),
			(2, "Water current (south)"),
			(2, "Water current (east)"),
			(2, "Water current (west)"),
			(2, "Block (water)"),
			(2, "Block (water)"),
			(2, "Block (top door) (water)"),
			(2, "Coin (water)"),
			(6, "Block (bottom door) (water)"),
			(2, "Water"),
			(2, "Water"),
			(2, "Water"),
			(2, "Water"),
			(5, "Damage (water)"),
			(5, "Damage (water)"),
			(2, "Top door (water)"),
			(5, "Death"),
			(5, "Damage"),
			(5, "Damage"),
			(5, "Damage")
		};

		public static void DumpBlocks(Rom rom, int course)
		{
			rom.SetBank(0xC);
			int header = rom.ReadWord(0x4560 + course * 2);

			//dump 16x16 blocks
			rom.SetBank(0xC);
			int blockbank = rom.ReadByte(header + 9);
			int blockubbank = rom.ReadByte(header + 10);

			rom.SetBank(blockbank);
			int tilesdata = rom.ReadWord(0x4000 + blockubbank * 2);
			RLEDecompressTiles(rom, tilesdata, LevelData);

			//dump objects
			rom.SetBank(0x7);
			int objectsPosition = rom.ReadWord(0x4199 + course * 2);
			RLEDecompressObjects(rom, objectsPosition, ObjectsData);
		}

		public static void DumpLevel(Rom rom, int course, int warp, bool checkPoint, int animatedTileIndex, bool reloadAnimatedTilesOnly)
		{
			int tilebank;
			int tileaddressB;
			int tileaddressA;
			int tileanimated;
			int blockindex;
			byte palette;
			int enemiesData;

			rom.SetBank(0xC);
			int header = rom.ReadWord((checkPoint ? 0x45B6 : 0x4560) + course * 2);

			if (warp != -1)
			{
				tilebank = rom.ReadByte(warp + 11);
				tileaddressB = rom.ReadWord(warp + 12);
				tileaddressA = rom.ReadWord(warp + 14);
				tileanimated = rom.ReadWord(warp + 18);
				blockindex = rom.ReadWord(warp + 20);
				palette = rom.ReadByte(warp + 10);
				enemiesData = rom.ReadWord(warp + 22);
			}
			else
			{
				tilebank = rom.ReadByte(header + 0);
				tileaddressB = rom.ReadWord(header + 1);
				tileaddressA = rom.ReadWord(header + 3);
				tileanimated = rom.ReadWord(header + 7);
				blockindex = rom.ReadWord(header + 11);
				palette = rom.ReadByte(header + 27);
				enemiesData = rom.ReadWord(header + 28);
			}

			if (!reloadAnimatedTilesOnly)
			{
				Array.Clear(Sprite.TilesEnemies.Bits, 0, Sprite.TilesEnemies.Bits.Length);
				Array.Clear(Sprite.LoadedSprites, 0, Sprite.LoadedSprites.Length);

				var (enemiesIdsPointer, enemiesTiles, _, _) = Sprite.FindEnemiesData(rom, enemiesData);
				Sprite.DumpEnemiesSprites(rom, enemiesIdsPointer, enemiesTiles, Sprite.TilesEnemies, 0, Sprite.LoadedSprites, 0, 40);

				//dump 8x8 tiles
				Array.Clear(Tiles8x8.Bits, 0, Tiles8x8.Width * Tiles8x8.Height);
				rom.SetBank(0x11);
				Dump8x8Tiles(rom, Tiles8x8, tileaddressA, 2 * 16, 0 * 16, palette, paletteColors, false);
				rom.SetBank(tilebank);
				Dump8x8Tiles(rom, Tiles8x8, tileaddressB, 6 * 16, 2 * 16, palette, paletteColors, false);

				//dump scroll
				rom.SetBank(0xC);
				rom.ReadBytes(0x4000 + course * 32, 32, ScrollData);

				//dump warps
				rom.SetBank(0xC);
				for (int i = 0; i < 32; i++)
				{
					int warpdata = rom.ReadWord(0x4F30 + course * 64 + i * 2);
					Warps[i] = rom.ReadByte(warpdata);
				}

				rom.SetBank(0xC);
				if (warp != -1)
				{
					int targetSector = rom.ReadByte(warp);
					WarioPosition = (targetSector % 16) * 256 + rom.ReadByte(warp + 2)
								+ ((targetSector / 16) * 256 + rom.ReadByte(warp + 1)) * 4096;
					WarioRightFacing = false;

					int cameraSector = rom.ReadByte(warp + 4);
					CameraPosition = rom.ReadByte(warp + 6) + (cameraSector % 16) * 256 + 16
								+ (rom.ReadByte(warp + 5) + (cameraSector / 16) * 256 + 16) * 4096;
					AnimatedTilesMask = rom.ReadByte(warp + 9);
				}
				else
				{
					WarioPosition = rom.ReadWordSwap(header + 15) + rom.ReadWordSwap(header + 13) * 4096;
					WarioRightFacing = (rom.ReadByte(header + 18) & 0x20) == 0;
					CameraPosition = rom.ReadWordSwap(header + 21) + 16
								+ (rom.ReadWordSwap(header + 19) + 16) * 4096;
					AnimatedTilesMask = rom.ReadByte(header + 26);
				}
			}

			if (AnimatedTilesMask != 0)
			{
				rom.SetBank(0x11);
				Dump8x8Tiles(rom, Tiles8x8, tileanimated + animatedTileIndex * 16 * 4, 4, 2 * 16, palette, paletteColors, false);
			}

			//dump 16x16 tiles
			if (reloadAnimatedTilesOnly)
			{
				rom.SetBank(0x11);
				DumpAnimated16x16Tiles(paletteColors[0]);
			}
			else
			{
				rom.SetBank(0xB);
				Dump16x16Tiles(blockindex, paletteColors[0]);
			}

			void DumpAnimated16x16Tiles(uint defaultColor)
			{
				using (Graphics g = Graphics.FromImage(Tiles16x16.Bitmap))
				{
					for (int y = 0; y < 16; y++)
					{
						for (int x = 0; x < 8; x++)
						{
							byte tileIndex = (byte)(x + y * 8);
							tileIndex = ReplaceTile(tileIndex);
							DumpAnimated16x16Tile(x, y, tileIndex);
						}
					}

					void DumpAnimated16x16Tile(int x, int y, byte tileIndex)
					{
						for (int k = 0; k < 2; k++)
						{
							for (int j = 0; j < 2; j++)
							{
								int subTileIndex = animated8x8Tiles[(x + y * 8) * 4 + k * 2 + j];
								if (subTileIndex != -1)
								{
									Point dest = new Point(j * 8 + x * 16, k * 8 + y * 16);
									Dump8x8Tile(dest, subTileIndex, defaultColor);

									if (ShowColliders)
									{
										int specialTile = GetTileInfo(tileIndex).Type;
										if (specialTile != -1)
										{
											g.FillRectangle(LevelPictureBox.TransparentBrushes[specialTile], new Rectangle(dest, new Size(8, 8)));
										}
									}
								}
							}
						}
					}
				}
			}

			void Dump16x16Tiles(int tileindexaddress, uint defaultColor)
			{
				using (Graphics g = Graphics.FromImage(Tiles16x16.Bitmap))
				{
					for (int y = 0; y < 16; y++)
					{
						for (int x = 0; x < 8; x++)
						{
							byte tileIndex = (byte)(x + y * 8);
							byte newTileIndex = ReplaceTile(tileIndex);

							bool isAnimatedTile = Dump16x16Tile(x, y, newTileIndex);
							Animated16x16Tiles[tileIndex] = isAnimatedTile;
						}
					}

					bool Dump16x16Tile(int x, int y, byte tileIndex)
					{
						bool isAnimatedTile = false;
						for (int k = 0; k < 2; k++)
						{
							for (int j = 0; j < 2; j++)
							{
								byte subTileIndex = rom.ReadByte(tileindexaddress + tileIndex * 4 + k * 2 + j);

								if (((SwitchMode & 2) != 0 && tileIndex == 0x32)
									|| ((SwitchMode & 1) != 0 && tileIndex == 0x39)
									|| ((SwitchMode & 4) != 0 && tileIndex == 0x38)) //[!] block
								{
									subTileIndex = (byte)(4 + k * 2 + j);
								}

								bool isAnimated = subTileIndex >= (2 * 16) && subTileIndex < (2 * 16 + 4);
								isAnimatedTile |= isAnimated;
								animated8x8Tiles[(x + y * 8) * 4 + k * 2 + j] = isAnimated ? subTileIndex : -1;

								Point dest = new Point(j * 8 + x * 16, k * 8 + y * 16);
								Dump8x8Tile(dest, subTileIndex, defaultColor);
							}
						}

						if (ShowColliders)
						{
							int specialTile = GetTileInfo(tileIndex).Type;
							if (specialTile != -1)
							{
								g.FillRectangle(LevelPictureBox.TransparentBrushes[specialTile], new Rectangle(x * 16, y * 16, 16, 16));
							}
						}

						return isAnimatedTile;
					}
				}
			}
		}

		static void RLEDecompressTiles(Rom rom, int tilesdata, byte[] decompressed)
		{
			int position = 0;

			while (position < decompressed.Length)
			{
				byte data = rom.ReadByte(tilesdata++);
				if ((data & 0x80) != 0)
				{
					data = (byte)(data & 0x7F);
					byte rep = rom.ReadByte(tilesdata++);
					for (int j = 0; j <= rep; j++)
					{
						decompressed[position++] = data;
					}
				}
				else
				{
					decompressed[position++] = data;
				}
			}
		}

		static void RLEDecompressObjects(Rom rom, int enemiesData, byte[] decompressed)
		{
			int position = 0;

			while (position < decompressed.Length)
			{
				byte data = rom.ReadByte(enemiesData++);
				decompressed[position++] = (byte)(data >> 4);
				decompressed[position++] = (byte)(data & 0xF);

				byte repeat = rom.ReadByte(enemiesData++);
				for (int i = 0; i < repeat; i++)
				{
					decompressed[position++] = 0;
					decompressed[position++] = 0;
				}
			}
		}

		static void Dump8x8Tile(Point dest, int subTileIndex, uint defaultColor)
		{
			if (subTileIndex < 128)
			{
				Point source = new Point((subTileIndex % 16) * 8, (subTileIndex / 16) * 8);
				for (int y = 0; y < 8; y++)
				{
					Array.Copy(Tiles8x8.Bits, source.X + (source.Y + y) * Tiles8x8.Width, Tiles16x16.Bits, dest.X + (dest.Y + y) * Tiles16x16.Width, 8);
				}
			}
			else
			{
				//fill 8x8 block with default color
				for (int y = 0; y < 8; y++)
				{
					int destIndex = dest.X + (dest.Y + y) * Tiles16x16.Width;
					for (int x = 0; x < 8; x++)
					{
						Tiles16x16.Bits[destIndex + x] = defaultColor;
					}
				}
			}
		}

		public static void Dump8x8Tiles(Rom rom, DirectBitmap bitmap, int gfxAddress, int tiles, int pos, byte palette, uint[] customPalette, bool transparency)
		{
			Dump8x8Tiles(Enumerable.Range(0, tiles * 16).Select(x => rom.ReadByte(gfxAddress + x)), bitmap, tiles, pos, palette, customPalette, transparency);
		}

		public static void Dump8x8Tiles(IEnumerable<byte> data, DirectBitmap bitmap, int tiles, int pos, byte palette, uint[] customPalette, bool transparency)
		{
			using (var enumerator = data.GetEnumerator())
			{
				for (int n = 0; n < tiles; n++)
				{
					int tilePosX = ((n + pos) % 16) * 8;
					int tilePosY = ((n + pos) / 16) * 8;

					for (int y = 0; y < 8; y++)
					{
						enumerator.MoveNext();
						byte data0 = enumerator.Current;
						enumerator.MoveNext();
						byte data1 = enumerator.Current;
						int destIndex = tilePosX + (y + tilePosY) * Tiles8x8.Width;

						for (int x = 0; x < 8; x++)
						{
							int pixelA = (data0 >> (7 - x)) & 0x1;
							int pixelB = (data1 >> (7 - x)) & 0x1;
							int pixel = pixelA + pixelB * 2;

							if (!transparency || pixel != 0)
							{
								int palindex = (palette >> pixel * 2) & 0x3;
								bitmap.Bits[destIndex + x] = customPalette[palindex];
							}
						}
					}
				}
			}
		}

		public static List<(int CourseId, int CourseNo)> GetCourseIds(Rom rom)
		{
			//convert course id => course no using data in ROM
			rom.SetBank(0);
			var courseIdToNo = new List<(int, int)>();
			for (int i = 0; i <= 0x2A; i++)
			{
				int levelpointer = rom.ReadWord(0x0534 + i * 2);
				int courseNo = (levelpointer - 0x0587) / 3;
				courseIdToNo.Add((i, courseNo));
			}

			return courseIdToNo;
		}

		public static (int Type, string Text) GetTileInfo(byte tileIndex)
		{
			(int Type, string Text) info;

			if (tileIndex < 0x28)
			{
				info = (0, "");
			}
			else if (tileIndex > 0x5F)
			{
				info = (-1, "");
			}
			else
			{
				info = tileInfo[tileIndex - 0x28];
			}

			if (SwitchTile(tileIndex, SwitchType) != tileIndex)
			{
				info = (7, string.IsNullOrWhiteSpace(info.Text) ? "Switchable block" : $"{info.Text} (switchable block)");
			}

			return info;
		}

		static byte SwitchTile(byte tileIndex, int switchMode)
		{
			//replace tiles when a (!) block is hit
			if ((switchMode & 1) != 0)
			{
				switch (tileIndex)
				{
					case 0x7C:
						return 0x27;

					case 0x27:
						return 0x7C;

					case 0x7A:
						return 0x44;

					case 0x79:
						return 0x45;
				}
			}

			if ((switchMode & 2) != 0)
			{
				switch (tileIndex)
				{
					case 0x7C:
						return 0x55;

					case 0x55:
						return 0x7C;

					case 0x7A:
						return 0x7B;

					case 0x7B:
						return 0x7A;

					case 0x59:
						return 0x5D;

					case 0x5D:
						return 0x59;
				}
			}

			return tileIndex;
		}

		static byte ReplaceTile(byte tileIndex)
		{
			if (!ShowCollectibles)
			{
				tileIndex = RemoveCollectible();
			}

			tileIndex = SwitchTile(tileIndex, SwitchMode);

			return tileIndex;

			byte RemoveCollectible()
			{
				switch (tileIndex)
				{
					case 0x29: //block
					case 0x2A: //block
					case 0x46: //coin
					case 0x33: //platform
						return 0x7F;

					case 0x2B: //block
					case 0x2C: //block
					case 0x47: //coin
						return 0x7E;

					case 0x2E: //block with door
						return 0x48;

					case 0x2D: //block with door
						return 0x7D;

					case 0x37: //bonus
					case 0x49: //bonus (hidden)
					case 0x28: //bonus (hidden)
					case 0x4A: //bonus (hidden water)
						return 0x3C;

					case 0x51: //block (water)
					case 0x50: //block (water)
					case 0x53: //coin (water)
						return 0x58;

					case 0x52: //block with door (water)
						return 0x5B;

					case 0x54: //block with door (water)
						return 0x4B;
				}

				return tileIndex;
			}
		}

		public static int GetSwitchType()
		{
			int result = 0;
			for (int tileIndex = 0; tileIndex < 8192; tileIndex++)
			{
				byte data = LevelData[tileIndex + 0x1000];
				switch (data)
				{
					case 0x39:
						result |= 1;
						break;

					case 0x32:
						result |= 2;
						break;

					case 0x38:
						result |= 4;
						break;
				}
			}

			return result;
		}

		public static int GetEmptyTile(uint[] bitmap, int tileSize, int tileWidth)
		{
			int emptyTile = 0;
			var tilesOccurence = new int[256];

			for (int i = 0; i < bitmap.Length; i++)
			{
				uint color = bitmap[i];
				if (color == 0xFFFFFFFF)
				{
					int tile = (i % 128) / tileSize + (i / 128) / tileSize * tileWidth;
					tilesOccurence[tile]++;
					if (tilesOccurence[tile] >= tilesOccurence[emptyTile])
					{
						emptyTile = tile;
					}
				}
			}

			return emptyTile;
		}

		public static bool SaveChanges(Rom rom, int course, out string errorMessage)
		{
			if (!SaveBlocksToRom(out errorMessage))
			{
				return false;
			}

			if (!SaveObjectsToRom(out errorMessage))
			{
				return false;
			}

			return true;

			bool SaveBlocksToRom(out string message)
			{
				var allTiles = GetAllTiles()
					.Select(x => RLECompressTiles(x).ToArray())
					.ToArray();

				if (!CheckSize())
				{
					message = $"Block data is too big to fit in ROM.";
					return false;
				}

				//rom expansion give new banks for level data
				rom.ExpandTo1MB();
				if (rom.IsMBC3())
				{
					rom.SetBank(0x20);
					rom.WriteBytes(0x4000, new byte[0x80000]); //zero out (before IPS patch)
				}

				//write them back to ROM
				WriteBlocks();

				message = string.Empty;
				return true;

				void WriteBlocks()
				{
					byte bank = 0x3F;
					byte subbank = 0;
					int writePosition = 0x4040; //first 64 bytes are reserved for level pointers
					for (int i = 0; i < 0x2B; i++)
					{
						byte[] tileData = allTiles[i];
						if ((writePosition + tileData.Length) >= 0x8000 || subbank >= 32)
						{
							//no more space, switch to another bank
							bank--;
							subbank = 0;
							writePosition = 0x4040;
						}

						//write data to bank
						rom.SetBank(bank);
						rom.WriteWord(0x4000 + subbank * 2, (ushort)writePosition);
						rom.WriteBytes(writePosition, tileData);

						//update level header
						rom.SetBank(0xC);
						int headerposition = rom.ReadWord(0x4560 + i * 2);
						rom.WriteByte(headerposition + 9, bank);
						rom.WriteByte(headerposition + 10, subbank);

						headerposition = rom.ReadWord(0x45B6 + i * 2);
						rom.WriteByte(headerposition + 9, bank);
						rom.WriteByte(headerposition + 10, subbank);

						subbank++;
						writePosition += tileData.Length;
					}
				}

				bool CheckSize()
				{
					int writePosition = 0;
					byte bank = 0x3F;

					foreach (var tileData in allTiles)
					{
						if ((writePosition + tileData.Length) >= (0x4000 - 0x40))
						{
							writePosition = 0;
							bank--;

							if (bank <= 0x32)
							{
								return false;
							}
						}

						writePosition += tileData.Length;
					}

					return true;
				}

				byte[][] GetAllTiles()
				{
					//grab all levels data at once
					byte[][] result = new byte[0x2B][];

					for (int i = 0; i < 0x2B; i++)
					{
						rom.SetBank(0xC);
						int headerposition = rom.ReadWord(0x4560 + i * 2);
						int tilebank = rom.ReadByte(headerposition + 9);
						int tilesubbank = rom.ReadByte(headerposition + 10);

						rom.SetBank(tilebank);
						int tilePosition = rom.ReadWord(0x4000 + tilesubbank * 2);
						result[i] = new byte[0x3000];
						RLEDecompressTiles(rom, tilePosition, result[i]);
					}

					Array.Copy(LevelData, result[course], 0x3000); //copy current level
					return result;
				}

				IEnumerable<byte> RLECompressTiles(byte[] tilesdata)
				{
					int current = 0;
					while (current < tilesdata.Length)
					{
						byte repeat = 0;
						byte data = tilesdata[current++];
						while (current < tilesdata.Length && tilesdata[current] == data && repeat < 255 && current % 256 != 0)
						{
							current++;
							repeat++;
						}

						if (repeat > 0)
						{
							yield return (byte)(0x80 | data);
							yield return repeat;
						}
						else
						{
							yield return data;
						}
					}
				}
			}

			bool SaveObjectsToRom(out string message)
			{
				byte[][] enemyData = new byte[0x2B][];

				//save objects
				rom.SetBank(0x7);
				for (int i = 0; i < 0x2B; i++)
				{
					int objectsPos = rom.ReadWord(0x4199 + i * 2);
					enemyData[i] = new byte[0x2000];
					RLEDecompressObjects(rom, objectsPos, enemyData[i]);
				}
				enemyData[course] = ObjectsData;

				const int maxSize = 4198;
				int objectSize = enemyData.Sum(x => RLECompressObjects(x).Count());
				if (objectSize > maxSize)
				{
					message = $"Object data is too big to fit in ROM.\r\nPlease remove some objects to free at least {objectSize - maxSize} byte(s).";
					return false;
				}

				int startPos = rom.ReadWord(0x4199);
				for (int i = 0; i < 0x2B; i++)
				{
					rom.WriteWord(0x4199 + i * 2, (ushort)startPos);

					byte[] data = RLECompressObjects(enemyData[i]).ToArray();
					rom.WriteBytes(startPos, data);
					startPos += data.Length;
				}

				message = string.Empty;
				return true;
			}

			IEnumerable<byte> RLECompressObjects(byte[] data)
			{
				byte[] halfData = RLECompressObjectsHelper().ToArray();

				int current = 0;
				while (current < halfData.Length)
				{
					byte value = halfData[current++];
					yield return value;

					byte count = 0;
					while (current < halfData.Length && halfData[current] == 0 && count < 255)
					{
						current++;
						count++;
					}
					yield return count;
				}

				IEnumerable<byte> RLECompressObjectsHelper()
				{
					for (int i = 0; i < data.Length; i += 2)
					{
						yield return (byte)((data[i] << 4) | data[i + 1]);
					}
				}
			}
		}
	}
}
