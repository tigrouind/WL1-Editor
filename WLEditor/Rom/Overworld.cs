using System;
using System.Collections.Generic;
using System.Linq;

namespace WLEditor
{
	public class Overworld
	{
		static readonly uint[] paletteColors = { 0xFFFFFFFF, 0xFFAAAAAA, 0xFF555555, 0xFF000000 };
		static readonly int[] directions = { 0xF0, 0xFE, 0xE0, 0xEE };

		static readonly int[,] flagsPosition =
		{
			{ 0x56F5, 0x56E8 },
			{ 0x5719, 0x570C },
			{ 0x5738, 0x572B },
			{ 0x575C, 0x574F },
			{ 0x5780, 0x5773 },
			{ 0x57A4, 0x5797 },
			{ 0x57C3, 0x57B6 },
		};

		static readonly int[] music =
		{
			0x6204,
			0x6204,
			0x78AE,
			0x7DB0,
			0x7980,
			0x7C04,
			0x7B62,
			0x7A9A,
			0x4D55,
		};

		static readonly byte[] musicTracks =
		{
			0x15,
			0x13,
			0x14,
			0x21,
			0x23,
			0x1F,
			0x1D,
			0x02
		};

		static readonly int[] overWorldNextDir =
		{
			0x40A5,
			0x40B5,
			0x40C2,
			0x40CD,
			0x40DB,
			0x40E6
		};

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

		public static IEnumerable<byte> RLECompressTiles(byte[] tilesdata)
		{
			List<byte> result = new List<byte>();
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

			yield return 0;
		}

		public static void Dump8x8Tiles(Rom rom, int bank, int tileAddress, DirectBitmap bitmap)
		{
			rom.SetBank(bank);
			byte[] data = new byte[384 * 8 * 2];
			RLEDecompressTiles(rom, tileAddress, data);
			Level.Dump8x8Tiles(Zip(Enumerable.Range(0, 384 * 8), x => data[x], x => data[x + 384 * 8]).Skip(128 * 16), bitmap, 256, 0, 0xE1, paletteColors, false);
		}

		public static void DumpAnimatedTilesA(Rom rom, int tileAddress, int tilePosition, DirectBitmap bitmap, int index, int offset)
		{
			rom.SetBank(8);
			Level.Dump8x8Tiles(Zip(Enumerable.Range(0, 8), x => rom.ReadByte(tileAddress + x * offset + index), x => (byte)0), bitmap, 1, tilePosition, 0xE1, paletteColors, false);
		}

		public static void DumpAnimatedTilesB(Rom rom, int tileAddress, int tilePosition, DirectBitmap bitmap)
		{
			rom.SetBank(8);
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
		}

		public static bool SaveTiles(Rom rom, int bank, int address, byte[] data, int maxSize, out string errorMessage)
		{
			rom.SetBank(bank);
			var compressedData = RLECompressTiles(data).ToArray();

			if (compressedData.Length > maxSize)
			{
				errorMessage = string.Format("Tile data is too big to fit in ROM.\r\n Please free at least {0} byte(s).", compressedData.Length - maxSize);
				return false;
			}

			rom.WriteBytes(address, compressedData);

			errorMessage = string.Empty;
			return true;
		}

		#endregion

		#region Events

		public static List<Tuple<int, byte>>[] LoadEvents(Rom rom, int[] events)
		{
			rom.SetBank(8);

			var result = new List<List<Tuple<int, byte>>>();
			foreach (var item in events)
			{
				int tileIndexAddress = rom.ReadWord(item + 1);
				int tilePositionAddress = rom.ReadWord(item + 4);
				var eventItem = new List<Tuple<int, byte>>();

				int position = 0;
				while (true)
				{
					byte tileIndex = rom.ReadByte(tileIndexAddress + position);
					int tilePosition = rom.ReadWordSwap(tilePositionAddress + position * 2);

					if (tileIndex == 0xFF)
					{
						break;
					}

					eventItem.Add(new Tuple<int, byte>(tilePosition - 0x9800, tileIndex));
					position++;
				};

				result.Add(eventItem);
			}

			return result.ToArray();
		}

		public static bool SaveEvents(Rom rom, List<Tuple<int, byte>>[] events,
			int[][] eventPointers, int[,] eventAddressOffset, int maxSize, out string errorMessage)
		{
			int size = events.Sum(x => x.Sum(y => 3) + 2);
			if (size > maxSize)
			{
				errorMessage = string.Format("Event data is too big to fit in ROM.\r\n Please free at least {0} byte(s).", size - maxSize);
				return false;
			}

			rom.SetBank(8);
			int position = rom.ReadWord(eventPointers[0][0] + eventAddressOffset[0, 0]);

			for (int i = 0; i < events.Length; i++)
			{
				var worldEvent = events[i];

				//fix pointers
				for (int j = 0; j < eventPointers.GetLength(0); j++)
				{
					var item = eventPointers[j];
					if (i < item.Length && item[i] != 0)
					{
						rom.WriteWord(item[i] + eventAddressOffset[j, 0], (ushort)position);
					}
				}

				//tile
				foreach (var item in worldEvent)
				{
					rom.WriteByte(position++, item.Item2);
				}
				rom.WriteByte(position++, 0xFF);

				//fix pointers
				for (int j = 0; j < eventPointers.GetLength(0); j++)
				{
					var item = eventPointers[j];
					if (i < item.Length && item[i] != 0)
					{
						rom.WriteWord(item[i] + eventAddressOffset[j, 1], (ushort)position);
					}
				}

				//position
				foreach (var item in worldEvent)
				{
					rom.WriteWordSwap(position, (ushort)(item.Item1 + 0x9800));
					position += 2;
				}
				rom.WriteByte(position++, 0xFF);
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
						Path = new List<WorldPathSegment>(),
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

			if (overWorld)
			{
				LoadFlags();
			}
			else
			{
				LoadTreasures();
			}

			return pathData;

			void LoadFlags()
			{
				rom.SetBank(0x14);
				for (int level = 0; level < flagsPosition.GetLength(0); level++)
				{
					var item = pathData[level];
					item.FlagX = rom.ReadByte(flagsPosition[level, 0] + 1) - 12;
					item.FlagY = rom.ReadByte(flagsPosition[level, 1] + 1) - 20;
				}
			}

			void LoadTreasures()
			{
				rom.SetBank(0x14);
				for (int level = 0; level < pathData.Length; level++)
				{
					var item = pathData[level];

					int pointer = rom.ReadByte(0x5A38 + level);
					if (pointer != 0xFF)
					{
						pointer = rom.ReadWord(0x5A63 + pointer * 2);
						item.TreasureX = rom.ReadByte(pointer + 1) - 12;
						item.TreasureY = rom.ReadByte(pointer + 0) - 20;
					}
				}
			}

		}

		public static bool SavePaths(Rom rom, WorldPath[] pathData, bool overWorld, out string errorMessage)
		{
			int[][] duplicates =
			{
				new [] { 7, 23 },  // rice beach 1 / flooded
				new [] { 14, 36 }, // rice beach 3 / flooded
				new [] { 5, 10 },  // mt teapot 4 / crushed
				new [] { 38, 42 }, // parsley woods 1 / flooded
			};

			if (!overWorld)
			{
				//will be stored using shared pointers
				foreach (var dup in duplicates)
				{
					pathData[dup[1]] = null;
				}
			}

			//check size
			int levelCount = pathData.Count(x => x != null);
			int dirCount = pathData.Where(x => x != null).Sum(x => x.Directions.Count(d => d.Path.Count > 0));
			int pathCount = pathData.Where(x => x != null).Sum(x => x.Directions.Where(d => d.Path.Count > 0).Sum(d => d.Path.Count * 3 + 2));

			int bytesToFree;
			if (overWorld)
			{
				bytesToFree = Math.Max(levelCount * 5 - 40, 0) + Math.Max(dirCount * 2 - 30, 0) + Math.Max(pathCount - 382, 0);
			}
			else
			{
				bytesToFree = Math.Max(levelCount * 5 - 210, 0) + Math.Max(dirCount * 2 - 168, 0) + Math.Max(pathCount - 610, 0);
			}

			if (bytesToFree > 0)
			{
				errorMessage = string.Format("Path data is too big to fit in ROM. Please free at least {0} byte(s).", bytesToFree);
				return false;
			}

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

			if (!overWorld)
			{
				//duplicates (shared pointers)
				foreach (var dup in duplicates)
				{
					var position = rom.ReadWord(0x5629 + dup[0] * 2);
					rom.WriteWord(0x5629 + dup[1] * 2, position);
				}

				//should only be done after path are written (shared pointers)
				foreach (var dup in duplicates)
				{
					pathData[dup[1]] = pathData[dup[0]];
				}
			}

			SaveProgression();
			SaveLevelsPosition();

			if (overWorld)
			{
				SaveFlags();
			}
			else
			{
				SaveTreasures();
			}

			errorMessage = string.Empty;
			return true;

			void SaveFlags()
			{
				rom.SetBank(0x14);
				for (int level = 0; level < flagsPosition.GetLength(0); level++)
				{
					var item = pathData[level];
					rom.WriteByte(flagsPosition[level, 0] + 1, (byte)Math.Max(0, Math.Min(255, item.FlagX + 12)));
					rom.WriteByte(flagsPosition[level, 1] + 1, (byte)Math.Max(0, Math.Min(255, item.FlagY + 20)));
				}
			}

			void SaveTreasures()
			{
				rom.SetBank(0x14);
				for (int level = 0; level < pathData.Length; level++)
				{
					var item = pathData[level];

					int pointer = rom.ReadByte(0x5A38 + level);
					if (pointer != 0xFF)
					{
						pointer = rom.ReadWord(0x5A63 + pointer * 2);
						rom.WriteByte(pointer + 1, (byte)Math.Max(0, Math.Min(255, item.TreasureX + 12)));
						rom.WriteByte(pointer + 0, (byte)Math.Max(0, Math.Min(255, item.TreasureY + 20)));
					}
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

		public static void SaveStartPosition(Rom rom, int x, int y, int side, int startPositionAddress, int startFunctionAddress)
		{
			rom.SetBank(8);
			rom.WriteByte(startPositionAddress + 3, (byte)Math.Max(0, Math.Min(255, x + 12)));
			rom.WriteByte(startPositionAddress + 1, (byte)Math.Max(0, Math.Min(255, y + 20)));

			if (startFunctionAddress != 0)
			{
				//screen side
				int[] functions =
				{
					0x60A7, //left
					0x60AF, //right
					0x609F, //top
					0x6097  //bottom
				};

				//jump relative address (jr)
				rom.WriteByte(startFunctionAddress + 1, (byte)((functions[side] - startFunctionAddress) - 2));
			}
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

		public static void SaveProgressNextDirection(Rom rom, WorldPath[] pathData, int[] levels, int world)
		{
			rom.SetBank(8);
			if (world == 7)
			{
				for (int flag = 0; flag < 6; flag++)
				{
					var nextDir = SearchProgressNextDirection((WorldPathProgressEnum)(1 << flag));
					rom.WriteByte(overWorldNextDir[flag], nextDir);
				}
			}
			else
			{
				for (int flag = 0; flag < 8; flag++)
				{
					var nextDir = SearchProgressNextDirection((WorldPathProgressEnum)(1 << flag));
					rom.WriteByte(0x735D + 16 * world + flag + 8, nextDir);
				}
			}

			byte SearchProgressNextDirection(WorldPathProgressEnum flag)
			{
				foreach (int level in levels)
				{
					for (int dir = 0; dir < 4; dir++)
					{
						var pathDir = pathData[level].Directions[dir];
						if (pathDir.Path.Count > 0 && pathDir.Progress == flag)
						{
							return new byte[] { 0x10, 0x20, 0x40, 0x80 }[dir];
						}
					}
				}

				return 0;
			}
		}

		#endregion
	}
}
