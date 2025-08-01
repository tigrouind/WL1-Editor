using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WLEditor
{
	public class Overworld
	{
		static readonly uint[] paletteColors = [0xFFFFFFFF, 0xFFAAAAAA, 0xFF555555, 0xFF000000];
		static readonly int[] directions = [0xF0, 0xFE, 0xE0, 0xEE];

		static readonly (int X, int Y)[] flagsPosition =
		[
			( 0x56F6, 0x56E9 ),
			( 0x571A, 0x570D ),
			( 0x5739, 0x572C ),
			( 0x5781, 0x5774 ),
			( 0x575D, 0x5750 ),
			( 0x57A5, 0x5798 ),
			( 0x57C4, 0x57B7 ),
		];

		static readonly int[] music =
		[
			0x6204,
			0,
			0x78AE,
			0x7DB0,
			0x7980,
			0x7C04,
			0x7B62,
			0x7A9A,
			0,
			0x4D55,
		];

		static readonly byte[] musicTracks =
		[
			0x15,
			0x13,
			0x14,
			0x21,
			0x23,
			0x1F,
			0x1D,
			0x02
		];

		static readonly int[] overWorldNextDir =
		[
			0x40A4,
			0x40B4,
			0x40C1,
			0x40CC,
			0x40DA,
			0x40E5
		];

		static readonly (int StartPositionAddress, int StartFunctionAddress, int FirstLevelAddress, WorldPathNextEnum Exit)[][] startPositionData =
		[
			[( 0x5558, 0x6075, 0x41CF, WorldPathNextEnum.Overworld)],
			[( 0x5558, 0x6075, 0x41CF, WorldPathNextEnum.Overworld)],
			[( 0x554C, 0x0000, 0x41DF, WorldPathNextEnum.Overworld), ( 0x5552, 0x6095, 0x41EF, WorldPathNextEnum.Sherbet)],
			[( 0x5527, 0x608D, 0x422F, WorldPathNextEnum.Teapot)],
			[( 0x553F, 0x607D, 0x41FF, WorldPathNextEnum.Overworld)],
			[( 0x552D, 0x6089, 0x421F, WorldPathNextEnum.Overworld)],
			[( 0x5533, 0x6085, 0x420F, WorldPathNextEnum.Overworld)],
			[( 0x5539, 0x6081, 0x423F, WorldPathNextEnum.Overworld)]
		];

		public static readonly WorldPathProgressEnum[] ProgressFlags =
		[
			WorldPathProgressEnum.None,
			WorldPathProgressEnum.Level1,
			WorldPathProgressEnum.Level2,
			WorldPathProgressEnum.Level3,
			WorldPathProgressEnum.Level4,
			WorldPathProgressEnum.Level5,
			WorldPathProgressEnum.Level6,
			WorldPathProgressEnum.Level7,
			WorldPathProgressEnum.Level8
		];

		public static bool HasPaths(int world) => world != 8 && world != 1 && world < 10;

		public static bool HasEvents(int world) => world != 8 && world < 10;

		public static bool HasMusic(int world) => world!= 8 && world != 1 && world < 10;

		public static bool IsOverworld(int world) => world == 9;

		#region World 8x8 tiles

		static void RLEDecompressTiles(Rom rom, int tilesdata, byte[] decompressed)
		{
			int position = 0;
			while (position < decompressed.Length)
			{
				byte rep = rom.ReadByte(tilesdata++);
				if (rep == 0)
				{
					break;
				}

				if ((rep & 0x80) != 0)
				{
					rep = (byte)(rep & 0x7F);
					for (int j = 0; j < rep && position < decompressed.Length; j++)
					{
						decompressed[position++] = rom.ReadByte(tilesdata++);
					}
				}
				else
				{
					byte data = rom.ReadByte(tilesdata++);
					for (int j = 0; j < rep && position < decompressed.Length; j++)
					{
						decompressed[position++] = data;
					}
				}
			}
		}

		static IEnumerable<byte> RLECompressTiles(byte[] tilesdata)
		{
			List<byte> result = [];
			int current = 0;
			while (current < tilesdata.Length)
			{
				byte repeat = 1;
				byte data = tilesdata[current++];
				while (current < tilesdata.Length && tilesdata[current] == data && repeat < 127)
				{
					current++;
					repeat++;
				}

				if (repeat > 2)
				{
					if (result.Count > 0)
					{
						yield return (byte)(0x80 | result.Count);
						foreach (byte val in result)
						{
							yield return val;
						}
						result.Clear();
					}

					yield return repeat;
					yield return data;
				}
				else
				{
					for (int i = 0; i < repeat; i++)
					{
						result.Add(data);

						if (result.Count == 127)
						{
							yield return (byte)(0x80 | result.Count);
							foreach (byte val in result)
							{
								yield return val;
							}
							result.Clear();
						}
					}
				}
			}

			if (result.Count > 0)
			{
				yield return (byte)(0x80 | result.Count);
				foreach (byte val in result)
				{
					yield return val;
				}
			}
		}

		public static void DumpFlags(Rom rom, DirectBitmap bitmap)
		{
			rom.SetBank(0x14);
			var data = rom.ReadBytes(0x7B97, 12 * 16);
			Level.Dump8x8Tiles(data, bitmap, 12, 0, 0x1E, paletteColors, true);
		}

		public static void Dump8x8Tiles(Rom rom, int bank, int tileAddress, DirectBitmap bitmap, byte palette)
		{
			Level.Dump8x8Tiles([.. Dump8x8Tiles(rom, bank, tileAddress).Skip(128 * 16)], bitmap, 256, 0, palette, paletteColors, false);
		}

		public static void Dump8x8TilesUncompressed(Rom rom, int bank, int tileAddress, int tiles, int pos, DirectBitmap bitmap, byte palette)
		{
			rom.SetBank(bank);
			var data = rom.ReadBytes(tileAddress, tiles * 16);
			Level.Dump8x8Tiles(data, bitmap, tiles, pos, palette, paletteColors, false);
		}

		public static IEnumerable<byte> Dump8x8Tiles(Rom rom, int bank, int tileAddress)
		{
			rom.SetBank(bank);
			byte[] data = new byte[384 * 8 * 2];
			RLEDecompressTiles(rom, tileAddress, data);
			return Zip(Enumerable.Range(0, 384 * 8), x => data[x], x => data[x + 384 * 8]);
		}

		public static byte[] RLECompressMapTiles(byte[] data)
		{
			return [.. RLECompressTiles(data), (byte)0];
		}

		public static byte[] RLECompress8x8Tiles(byte[] data)
		{
			return [.. RLECompressTiles([.. Unzip(data)])];
		}

		public static bool Save8x8Tiles(Rom rom, int bank, int address, byte[] data, int compressedSize, out string message)
		{
			var compressedData = RLECompress8x8Tiles(data);
			if (compressedData.Length > compressedSize)
			{
				message = $"Data is too big to fit in ROM.\r\nPlease free at least {compressedData.Length - compressedSize} byte(s)";
				return false;
			}

			rom.SetBank(bank);
			rom.WriteBytes(address, compressedData);

			message = string.Empty;
			return true;
		}

		public static void DumpAnimatedTilesA(Rom rom, int tileAddress, int tilePosition, DirectBitmap bitmap, int index, int offset)
		{
			rom.SetBank(8);
			Level.Dump8x8Tiles(Zip(Enumerable.Range(0, 8), x => rom.ReadByte(tileAddress + x * offset + index), x => (byte)0), bitmap, 1, tilePosition, 0xE1, paletteColors, false);
		}

		public static void DumpAnimatedTilesB(Rom rom, int bank, int tileAddress, int tilePosition, DirectBitmap bitmap)
		{
			rom.SetBank(bank);
			Level.Dump8x8Tiles(Enumerable.Range(0, 16)
				.Select(x => rom.ReadByte(tileAddress + x)), bitmap, 1, tilePosition, 0xE1, paletteColors, false);
		}

		static IEnumerable<TResult> Zip<T, TResult>(IEnumerable<T> source, Func<T, TResult> first, Func<T, TResult> second)
		{
			foreach (var item in source)
			{
				yield return first(item);
				yield return second(item);
			}
		}

		static IEnumerable<T> Unzip<T>(T[] source)
		{
			for (int i = 0; i < source.Length; i += 2)
			{
				yield return source[i];
			}

			for (int i = 0; i < source.Length; i += 2)
			{
				yield return source[i + 1];
			}
		}

		public static int GetScroll(Rom rom, int value)
		{
			rom.SetBank(0x14);
			unchecked
			{
				return (sbyte)rom.ReadByte(0x4F13 + value % 16);
			}
		}

		#endregion

		#region World tiles

		public static void LoadTiles(Rom rom, int bank, int tileAddress, byte[] data)
		{
			rom.SetBank(bank);
			RLEDecompressTiles(rom, tileAddress, data);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] ^= 0x80;
			}
		}

		public static bool SaveTiles(Rom rom, int bank, int address, byte[] data, int maxSize, out string errorMessage)
		{
			rom.SetBank(bank);
			var compressedData = RLECompressMapTiles([.. data.Select(x => (byte)(x ^ 0x80))]);

			if (compressedData.Length > maxSize)
			{
				errorMessage = $"Tile data is too big to fit in ROM.\r\nPlease free at least {compressedData.Length - maxSize} byte(s).";
				return false;
			}

			rom.WriteBytes(address, compressedData);

			errorMessage = string.Empty;
			return true;
		}

		#endregion

		#region Events

		public static List<(int X, int Y, byte Index)>[] LoadEvents(Rom rom, int[] events)
		{
			rom.SetBank(8);

			var result = new List<List<(int, int, byte)>>();
			foreach (var item in events)
			{
				int tileIndexAddress = rom.ReadWord(item + 1);
				int tilePositionAddress = rom.ReadWord(item + 4);
				var eventItem = new List<(int, int, byte)>();

				int position = 0;
				while (true)
				{
					byte tileIndex = rom.ReadByte(tileIndexAddress + position);
					int tilePosition = rom.ReadWordSwap(tilePositionAddress + position * 2);

					if (tileIndex == 0xFF)
					{
						break;
					}

					int tilePos = tilePosition - 0x9800;
					eventItem.Add((tilePos % 32, tilePos / 32, (byte)(tileIndex ^ 0x80)));
					position++;
				};

				result.Add(eventItem);
			}

			return [.. result];
		}

		public static bool SaveEvents(Rom rom, List<(int X, int Y, byte Index)>[] events,
			int[][] eventPointers, (int Tile, int Position)[] eventAddressOffset, int maxSize, out string errorMessage)
		{
			int size = events.Sum(x => x.Sum(y => 3) + 2);
			if (size > maxSize)
			{
				errorMessage = $"Event data is too big to fit in ROM.\r\n Please free at least {size - maxSize} byte(s).";
				return false;
			}

			rom.SetBank(8);
			int position = rom.ReadWord(eventPointers[0][0] + eventAddressOffset[0].Tile);

			for (int i = 0; i < events.Length; i++)
			{
				var worldEvent = events[i];

				SaveTilesIndex();
				SaveTilesPosition();

				void SaveTilesIndex()
				{
					//fix pointers
					for (int j = 0; j < eventPointers.GetLength(0); j++)
					{
						var item = eventPointers[j];
						if (i < item.Length && item[i] != 0)
						{
							rom.WriteWord(item[i] + eventAddressOffset[j].Tile, (ushort)position);
						}
					}

					//tile
					foreach (var item in worldEvent)
					{
						rom.WriteByte(position++, (byte)(item.Index ^ 0x80));
					}
					rom.WriteByte(position++, 0xFF);
				}

				void SaveTilesPosition()
				{
					//fix pointers
					for (int j = 0; j < eventPointers.GetLength(0); j++)
					{
						var item = eventPointers[j];
						if (i < item.Length && item[i] != 0)
						{
							rom.WriteWord(item[i] + eventAddressOffset[j].Position, (ushort)position);
						}
					}

					//position
					foreach (var item in worldEvent)
					{
						rom.WriteWordSwap(position, (ushort)(item.X + item.Y * 32 + 0x9800));
						position += 2;
					}
					rom.WriteByte(position++, 0xFF);
				}
			}

			errorMessage = string.Empty;
			return true;
		}

		#endregion

		#region Paths

		public static WorldPath[] LoadPaths(Rom rom, bool overWorld)
		{
			var pathData = new WorldPath[overWorld ? 8 : 43];

			rom.SetBank(8);
			for (int level = 0; level < pathData.Length; level++)
			{
				int levelPos = rom.ReadWord((overWorld ? 0x45BA : 0x556D) + level * 2);
				int posX = rom.ReadByte(levelPos + 1) - 12;
				int posY = rom.ReadByte(levelPos + 0) - 20;

				if (overWorld)
				{
					//scroll
					posX += rom.ReadByte(levelPos + 3);
					posY += rom.ReadByte(levelPos + 2);
				}

				int pointer = rom.ReadWord((overWorld ? 0x43B8 : 0x5629) + level * 2);
				int dirFlag = rom.ReadByte(pointer);

				var dirs = new WorldPathDirection[4];

				for (int dir = 0; dir < 4; dir++)
				{
					var direction = new WorldPathDirection
					{
						Progress = WorldPathProgressEnum.None,
						Path = [],
						Next = overWorld ? WorldPathNextEnum.TeapotOverworld : WorldPathNextEnum.Overworld
					};

					if ((dirFlag & (1 << (dir + 4))) != 0)
					{
						int position = rom.ReadByte(pointer + dir + 1);
						if (position != 0xFF)
						{
							position = rom.ReadWord((overWorld ? 0x43F0 : 0x5760) + position * 2);
							byte data = rom.ReadByte(position);

							while (data != 0xFF)
							{
								int steps = rom.ReadByte(position + 1);
								int status = rom.ReadByte(position + 2);

								direction.Path.Add(new WorldPathSegment { Status = (WorldPathStatusEnum)status, Direction = (WorldPathDirectionEnum)Array.IndexOf(directions, data), Steps = steps });

								position += 3;
								data = rom.ReadByte(position);
							}

							int next = rom.ReadByte(position + 1);
							direction.Next = (WorldPathNextEnum)next;

							int progress = rom.ReadWord((overWorld ? 0x6486 : 0x6496) + level * 2);
							direction.Progress = (WorldPathProgressEnum)rom.ReadByte(progress + 1 + dir * 2);
						}
					}

					dirs[dir] = direction;
				}

				pathData[level] = new WorldPath { X = posX, Y = posY, Directions = dirs };
			}

			return pathData;
		}

		public static bool SavePaths(Rom rom, WorldPath[] pathData, int currentWorld, out string errorMessage)
		{
			(int, int)[] duplicates =
			[
				( 7, 23 ),  // rice beach 1 / flooded
				( 14, 36 ), // rice beach 3 / flooded
				( 5, 10 ),  // mt teapot 4 / crushed
				( 38, 42 ), // parsley woods 1 / flooded
			];

			bool overWorld = IsOverworld(currentWorld);
			if (!overWorld)
			{
				//will be stored using shared pointers
				foreach (var dup in duplicates)
				{
					pathData[dup.Item2] = null;
				}
			}

			errorMessage = CheckSize();
			if (errorMessage != null)
			{
				return false;
			}

			SaveDirection();

			if (!overWorld)
			{
				//duplicates (shared pointers)
				foreach (var dup in duplicates)
				{
					var position = rom.ReadWord(0x5629 + dup.Item1 * 2);
					rom.WriteWord(0x5629 + dup.Item2 * 2, position);
				}

				//should only be done after path are written (shared pointers)
				foreach (var dup in duplicates)
				{
					pathData[dup.Item2] = pathData[dup.Item1];
				}
			}

			SpecialFixes();
			SaveProgression();
			SaveLevelsPosition();

			errorMessage = string.Empty;
			return true;

			string CheckSize()
			{
				int levelCount = pathData.Count(x => x != null) * 5;
				int dirCount = pathData.Where(x => x != null).Sum(x => x.Directions.Count(d => d.Path.Count > 0)) * 2;
				int pathCount = pathData.Where(x => x != null).Sum(x => x.Directions.Where(d => d.Path.Count > 0).Sum(d => d.Path.Count * 3 + 2));

				(int Level, int Dir, int Path) maxSize = new[]
				{
					(40, 30, 382),
					(210, 168, 610)
				}[overWorld ? 0 : 1];

				int bytesToFree = Math.Max(levelCount - maxSize.Level, 0)
						+ Math.Max(dirCount - maxSize.Dir, 0)
						+ Math.Max(pathCount - maxSize.Path, 0);

				if (bytesToFree > 0)
				{
					return $"Path data is too big to fit in ROM. Please free at least {bytesToFree} byte(s).";
				}

				return null;
			}

			void SaveDirection()
			{
				rom.SetBank(8);

				int positionDir = overWorld ? 0x43C8 : 0x5689;
				int positionDirHeader = overWorld ? 0x43F0 : 0x5760;
				int positionPath = overWorld ? 0x440E : 0x5808;

				for (int level = 0; level < pathData.Length; level++)
				{
					var item = pathData[level];
					if (item != null)
					{
						var dirs = item.Directions;

						//header
						rom.WriteWord((overWorld ? 0x43B8 : 0x5629) + level * 2, (ushort)positionDir);

						//direction flag
						int dirFlag = 0;
						for (int dir = 0; dir < 4; dir++)
						{
							if (dirs[dir].Path.Count > 0)
							{
								dirFlag |= 0x10 << dir;
							}
						}

						rom.WriteByte(positionDir++, (byte)dirFlag);

						for (int dir = 0; dir < 4; dir++)
						{
							var direction = dirs[dir];
							if (direction.Path.Count > 0)
							{
								//write headers
								rom.WriteByte(positionDir++, (byte)((positionDirHeader - (overWorld ? 0x43F0 : 0x5760)) / 2));
								rom.WriteWord(positionDirHeader, (ushort)positionPath);
								positionDirHeader += 2;

								foreach (var path in direction.Path)
								{
									rom.WriteByte(positionPath++, (byte)directions[(int)path.Direction]);
									rom.WriteByte(positionPath++, (byte)path.Steps);
									rom.WriteByte(positionPath++, (byte)path.Status);
								}

								rom.WriteByte(positionPath++, 0xFF);
								rom.WriteByte(positionPath++, (byte)direction.Next);
							}
							else
							{
								rom.WriteByte(positionDir++, 0xFF);
							}
						}
					}
				}
			}

			void SpecialFixes()
			{
				if (currentWorld == 2)
				{
					//fix current path being set to 0x2F after beating teapot 6 (hidden path to teapot 4 / boss)
					rom.SetBank(0x8);
					rom.WriteByte(0x78CD, 0x09); //replace "ld a, 2F" by "ld a, 09"
				}
				else if (currentWorld == 6)
				{
					//fix parsley woods unpassable unless lake is drained
					rom.SetBank(0x8);
					rom.WriteByte(0x417C, 0x18); //replace "jr nz, 4159" by "jr 4159"
				}
			}

			void SaveProgression()
			{
				rom.SetBank(8);
				for (int level = 0; level < pathData.Length; level++)
				{
					var item = pathData[level];
					for (int dir = 0; dir < 4; dir++)
					{
						var direction = item.Directions[dir];
						int pointer = rom.ReadWord((overWorld ? 0x6486 : 0x6496) + level * 2);
						rom.WriteByte(pointer + 1 + dir * 2, (byte)direction.Progress);
					}
				}
			}

			void SaveLevelsPosition()
			{
				rom.SetBank(8);
				for (int level = 0; level < pathData.Length; level++)
				{
					var item = pathData[level];
					int posX = item.X + 12;
					int posY = item.Y + 20;
					int pointer = rom.ReadWord((overWorld ? 0x45BA : 0x556D) + level * 2);

					if (overWorld)
					{
						int scrollX = Math.Max(0, Math.Min(96, posX - 88));
						int scrollY = Math.Max(0, Math.Min(112, posY - 96));

						rom.WriteByte(pointer + 3, (byte)scrollX);
						rom.WriteByte(pointer + 2, (byte)scrollY);
						rom.WriteByte(pointer + 1, (byte)Math.Max(0, Math.Min(255, posX - scrollX)));
						rom.WriteByte(pointer + 0, (byte)Math.Max(0, Math.Min(255, posY - scrollY)));
					}
					else
					{
						rom.WriteByte(pointer + 1, (byte)Math.Max(0, Math.Min(255, posX)));
						rom.WriteByte(pointer + 0, (byte)Math.Max(0, Math.Min(255, posY)));
					}
				}
			}
		}

		#endregion

		#region Flags

		public static (int X, int Y)[] LoadFlags(Rom rom)
		{
			var result = new List<(int X, int Y)>();
			rom.SetBank(0x14);
			for (int level = 0; level < flagsPosition.GetLength(0); level++)
			{
				int x = rom.ReadByte(flagsPosition[level].X) - 16;
				int y = rom.ReadByte(flagsPosition[level].Y) - 32;
				result.Add((x, y));
			}

			return [.. result];
		}

		public static void SaveFlags(Rom rom, (int X, int Y)[] items)
		{
			rom.SetBank(0x14);
			for (int level = 0; level < flagsPosition.GetLength(0); level++)
			{
				var item = items[level];
				rom.WriteByte(flagsPosition[level].X, (byte)Math.Max(0, Math.Min(255, item.X + 16)));
				rom.WriteByte(flagsPosition[level].Y, (byte)Math.Max(0, Math.Min(255, item.Y + 32)));
			}
		}

		#endregion

		public static void SaveTreasures(Rom rom, WorldPath[] pathData)
		{
			var treasures = Enumerable.Range(0, 15)
				.Select<int, (int TreasureId, int Level)>(x => (x, 255))
				.Concat(Enumerable.Range(0, 43)
					.Where(x => pathData[x].Directions.Any(d => d.Path.Count > 0)) //skip unreachable levels
					.Select<int, (int TreasureId, int Level)>(x => (Sector.GetTreasureId(rom, x), x))
					.Where(x => x.TreasureId != -1))
				.GroupBy(x => x.TreasureId, (x, y) => y.Last().Level)
				.ToArray();

			var overworldTreasureList = new Dictionary<int, int>();
			SaveTreasureOverworldLists();
			SaveTreasureLevelBinding();
			SaveTreasurePositionPerLevel();

			void SaveTreasureOverworldLists()
			{
				var overworldLists = new (int Offset, int[] Levels)[]
				{
					(0x5D15, new [] { 7, 23, 15, 14, 36, 12, 25, 41 }),
					(0x78F0, new [] { 6, 16, 13, 5, 10, 17, 9 }),
					(0x7DE1, new [] { 33, 2, 4, 8, 32, 24 }),
					(0x79B1, new [] { 3, 21, 22, 39, 27, 28 }),
					(0x7C2D, new [] { 0, 30, 31, 11, 20 }),
					(0x7B9D, new [] { 38, 42, 29, 1, 19, 18, 26 }),
					(0x7AE7, new [] { 37, 34, 35, 40 })
				};

				rom.SetBank(0x8);
				int position = rom.ReadWord(overworldLists[0].Offset + 1);
				foreach (var (Offset, Levels) in overworldLists)
				{
					rom.WriteWord(Offset + 1, (ushort)position); //ld hl, XXXX
					foreach (var level in Levels)
					{
						overworldTreasureList[level] = position;
					}

					int treasureCount = Levels.Count(x => treasures.Contains(x));
					position += treasureCount;
					position++; //0xFF marker
				}

				overworldTreasureList[255] = position;
			}

			void SaveTreasureLevelBinding()
			{
				rom.SetBank(0x1);
				int position = 0x586B;

				for (int treasureId = 0; treasureId < 15; treasureId++)
				{
					int level = treasures[treasureId];
					rom.WriteWord(position + 1, (ushort)overworldTreasureList[level]); //ld hl, XXXX
					rom.WriteByte(position + 4, (byte)level); //ld b, XX
					position += 7;
				}
			}

			void SaveTreasurePositionPerLevel()
			{
				rom.SetBank(0x14);
				int position = 0;

				//group levels at same position (usually duplicates)
				foreach (var levels in pathData
					.Select((x, i) => (Index: i, x.X, x.Y))
					.GroupBy(x => (x.X, x.Y)))
				{
					int index = 0xFF;
					if (levels.Any(x => treasures.Contains(x.Index)))
					{
						index = position++;
						int pointer = rom.ReadWord(0x5A63 + index * 2);
						rom.WriteByte(pointer + 0, (byte)Math.Max(0, Math.Min(255, levels.Key.Y + 24)));
						rom.WriteByte(pointer + 1, (byte)Math.Max(0, Math.Min(255, levels.Key.X + 16)));
					}

					foreach (var level in levels)
					{
						//map level to a treasure position index (or 0xFF)
						rom.WriteByte(0x5A38 + level.Index, (byte)index);
					}
				}
			}
		}

		public static void SaveStartPosition(Rom rom, int currentWorld, WorldPath[] pathData, int[] levels,
			Func<WorldPathNextEnum, bool> isSpecialExit,
			Func<WorldPath, WorldPathDirection, (int posX, int posY)> getPathPosition)
		{
			foreach ((int startPositionAddress, int startFunctionAddress, int firstLevelAddress, WorldPathNextEnum exit) in startPositionData[currentWorld])
			{
				int startLevel = GetFirstLevel(pathData, levels, isSpecialExit, exit);
				(int x, int y) = FindExitPosition(pathData[startLevel], exit);
				SaveStartPosition(x, y, startPositionAddress, startFunctionAddress, FindClosestSide(x, y));
				SaveFirstLevel(firstLevelAddress, startLevel);
			}

			void SaveStartPosition(int x, int y, int startPositionAddress, int startFunctionAddress, int side)
			{
				rom.SetBank(8);
				rom.WriteByte(startPositionAddress + 3, (byte)Math.Max(0, Math.Min(255, x + 12)));
				rom.WriteByte(startPositionAddress + 1, (byte)Math.Max(0, Math.Min(255, y + 20)));

				if (startFunctionAddress != 0)
				{
					//screen side
					int[] functions =
					[
						0x60A7, //left
						0x60AF, //right
						0x609F, //top
						0x6097  //bottom
					];

					//jump relative address (jr)
					rom.WriteByte(startFunctionAddress + 1, (byte)((functions[side] - startFunctionAddress) - 2));
				}
			}

			void SaveFirstLevel(int firstLevelAddress, int startLevel)
			{
				rom.SetBank(8);
				rom.WriteByte(firstLevelAddress + 1, (byte)startLevel); //ld a, XX
			}

			int FindClosestSide(int x, int y)
			{
				int[] borders = [x, 160 - x, y, 144 - y];
				int bestSide = -1, min = int.MaxValue;

				for (int i = 0; i < borders.Length; i++)
				{
					if (borders[i] < min)
					{
						min = borders[i];
						bestSide = i;
					}
				}

				return bestSide;
			}

			(int posX, int posY) FindExitPosition(WorldPath item, WorldPathNextEnum exit)
			{
				var dir = item.Directions.Where(x => x.Path.Count > 0 && x.Next == exit)
					.Concat(item.Directions.Where(x => x.Path.Count > 0 && isSpecialExit(x.Next))) //fallback
					.FirstOrDefault();

				if (dir != null)
				{
					return getPathPosition(item, dir);
				}

				return (item.X, item.Y);
			}
		}

		static int GetFirstLevel(WorldPath[] pathData, int[] levels, Func<WorldPathNextEnum, bool> isSpecialExit, WorldPathNextEnum exit)
		{
			return levels.Where(x => pathData[x].Directions.Any(d => d.Path.Count > 0 && d.Next == exit))
				.Concat(levels.Where(x => pathData[x].Directions.Any(d => d.Path.Count > 0 && isSpecialExit(d.Next)))) //fallback
				.Append(levels.First())
				.First();
		}

		#region Music

		public static int GetMusic(Rom rom, int world)
		{
			rom.SetBank(0x8);
			return Array.IndexOf(musicTracks, rom.ReadByte(music[world]));
		}

		public static void SetMusic(Rom rom, int world, int musicTrack)
		{
			rom.SetBank(0x8);
			rom.WriteByte(music[world], musicTracks[musicTrack]);
		}

		#endregion

		#region Progress

		public static void SaveProgressNextDirection(Rom rom, int currentWorld, WorldPath[] pathData, int[] levels, Func<WorldPathNextEnum, bool> isSpecialExit)
		{
			var directions = new byte[] { 0x10, 0x20, 0x40, 0x80 };

			rom.SetBank(8);
			if (IsOverworld(currentWorld))
			{
				foreach (var (progress, dir) in GetProgressNextDirections(0)
					.Where(x => x.Progress < overWorldNextDir.Length))
				{
					rom.WriteByte(overWorldNextDir[progress] + 1, directions[dir]); //ld a, XX
				}
			}
			else
			{
				int[] worldIndex = [0, 0, 1, 5, 2, 3, 4, 6];
				int world = worldIndex[currentWorld];

				int firstLevel = GetFirstLevel(pathData, levels, isSpecialExit, WorldPathNextEnum.Overworld);
				foreach (var (progress, dir) in GetProgressNextDirections(firstLevel))
				{
					rom.WriteByte(0x735D + world * 16 + progress + 8, directions[dir]);
				}
			}

			IEnumerable<(int Progress, int Dir)> GetProgressNextDirections(int startLevel)
			{
				var result = new Dictionary<WorldPathProgressEnum, int>();
				var seen = new HashSet<int>();
				var queue = new Queue<int>();
				queue.Enqueue(startLevel);
				seen.Add(startLevel);

				while (queue.Count > 0) //bfs
				{
					var path = pathData[queue.Dequeue()];

					for (int dir = 0; dir < 4; dir++)
					{
						var pathDir = path.Directions[dir];
						if (pathDir.Path.Count > 0)
						{
							if (pathDir.Progress != WorldPathProgressEnum.None && !result.ContainsKey(pathDir.Progress))
							{
								int progress = Array.IndexOf(ProgressFlags, pathDir.Progress);
								if (progress != -1) //should never happen
								{
									yield return (progress - 1, dir);
								}

								result.Add(pathDir.Progress, dir);
							}

							if (!isSpecialExit(pathDir.Next) && !seen.Contains((int)pathDir.Next))
							{
								queue.Enqueue((int)pathDir.Next);
								seen.Add((int)pathDir.Next);
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
