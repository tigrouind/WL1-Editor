using System;
using System.Collections.Generic;
using System.Linq;

namespace WLEditor
{
	public class Map
	{				
		static uint[] paletteColors =  { 0xFFFFFFFF, 0xFFAAAAAA, 0xFF555555, 0xFF000000 };
		static int[] directions = { 0xF0, 0xFE, 0xE0, 0xEE };
		
		static readonly int[][] flagsPosition = 
		{
			new int[] { 0x56F5, 0x56E8 },
			new int[] { 0x5719, 0x570C },
			new int[] { 0x5738, 0x572B },
			new int[] { 0x575C, 0x574F },
			new int[] { 0x5780, 0x5773 },
			new int[] { 0x57A4, 0x5797 },
			new int[] { 0x57C3, 0x57B6 },
		};
		
		#region World tiles
		
		static void RLEDecompressTiles(Rom rom, int tilesdata, byte[] decompressed)
		{
			int position = 0;
			while(position < decompressed.Length)
			{
				byte rep = rom.ReadByte(tilesdata++);
				if (rep == 0)
				{
					break;
				}
				
				if((rep & 0x80) != 0)
				{
					rep = (byte)(rep & 0x7F);					
					for(int j = 0 ; j < rep && position < decompressed.Length ; j++)
					{									
						decompressed[position++] = rom.ReadByte(tilesdata++);
					}
				}
				else
				{					
					byte data = rom.ReadByte(tilesdata++);
					for(int j = 0 ; j < rep && position < decompressed.Length ; j++)
					{						
						decompressed[position++] = data;
					}
				}
			}
		}
				
		static IEnumerable<byte> DecompressTilesHelper(byte[] data, int length)
		{
			for(int i = 0 ; i < length; i++)
			{
				yield return data[i];
				yield return data[i+length];
			}
		}
		
		public static IEnumerable<byte> RLECompressTiles(byte[] tilesdata)
		{
			List<byte> result = new List<byte>();
			int current = 0;
			while(current < tilesdata.Length)
			{
				byte repeat = 1;
				byte data = tilesdata[current++];
				while(current < tilesdata.Length && tilesdata[current] == data && repeat < 127)
				{
					current++;
					repeat++;
				}

				if(repeat > 2)
				{
					if (result.Count > 0)
					{
						yield return (byte)(0x80 | result.Count);
						foreach(byte val in result)
						{
							yield return val;	
						}
						result.Clear();
					}
					
					yield return repeat;
					yield return (byte)(data);					
				}
				else
				{
					for(int i = 0 ; i < repeat ; i++)
					{
						result.Add(data);
						
						if (result.Count == 127)
						{
							yield return (byte)(0x80 | result.Count);
							foreach(byte val in result)
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
				foreach(byte val in result)
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
			Level.Dump8x8Tiles(DecompressTilesHelper(data, 384 * 8).Skip(128 * 16), bitmap, 256, 0, 0xE1, paletteColors, false);
		}
		
		public static void DumpAnimatedTilesA(Rom rom, int tileAddress, int tilePosition, DirectBitmap bitmap, int index, int offset)
		{
			rom.SetBank(8);
			Level.Dump8x8Tiles(Zip(Enumerable.Range(0, 8)
				.Select(x => rom.ReadByte(tileAddress + x * offset + index)), Enumerable.Range(0, 8).Select(x => (byte)0)), bitmap, 1, tilePosition, 0xE1, paletteColors, false);
		}
		
		public static void DumpAnimatedTilesB(Rom rom, int tileAddress, int tilePosition, DirectBitmap bitmap)
		{
			rom.SetBank(8);
			Level.Dump8x8Tiles(Enumerable.Range(0, 16)
               .Select(x => rom.ReadByte(tileAddress + x)), bitmap, 1, tilePosition, 0xE1, paletteColors, false);
		}
		
		static IEnumerable<byte> Zip(IEnumerable<byte> first, IEnumerable<byte> second)
		{
			using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
			{
                while (e1.MoveNext() && e2.MoveNext())
                {
                	yield return e1.Current;
                	yield return e2.Current;
                }
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
				errorMessage = string.Format("World data is too big to fit in ROM.\r\n Please free at least {0} byte(s).", compressedData.Length - maxSize);
				return false;
			}
			
			rom.WriteBytes(address, compressedData);			
			
			errorMessage = string.Empty;
			return true;
		}
		
		#endregion
		
		#region Events
		
		public static List<List<KeyValuePair<int, byte>>> LoadEvents(Rom rom, int[] events)
		{
			rom.SetBank(8);			
			
			var result = new List<List<KeyValuePair<int, byte>>>();
			foreach (var item in events)
			{					
				int tileIndexAddress = rom.ReadWord(item + 1);
				int tilePositionAddress = rom.ReadWord(item + 4);
				var eventItem = new List<KeyValuePair<int, byte>>();
				
				int position = 0;
				while(true)
				{				
					byte tileIndex = rom.ReadByte(tileIndexAddress + position);
					int tilePosition = rom.ReadWordSwap(tilePositionAddress + position * 2);
					
					if(tileIndex == 0xFF)
					{
						break;
					}
					
					eventItem.Add(new KeyValuePair<int, byte>(tilePosition - 0x9800, tileIndex));
					position++;
				};
				
				result.Add(eventItem);
			}		
			
			return result;
		}
		
		public static bool SaveEvents(Rom rom, List<List<KeyValuePair<int, byte>>> events, 
			int[][][] eventPointers, int maxSize, out string errorMessage)
		{			
			int size = events.Sum(x => x.Sum(y => 3) + 2);
			if (size > maxSize)
			{
				errorMessage = string.Format("Event data is too big to fit in ROM.\r\n Please free at least {0} byte(s).", size - maxSize);
				return false;
			}
			
			rom.SetBank(8);	
			int position = rom.ReadWord(eventPointers[0][0][0] + eventPointers[0][1][0]);
			
			for(int i = 0 ; i < events.Count; i++)
			{				
				var worldEvent = events[i];
				
				//fix pointers
				foreach(var item in eventPointers)
				{
					if (i < item[0].Length && item[0][i] != 0)
					{
						rom.WriteWord(item[0][i] + item[1][0], (ushort)position);
					}	
				}
				
				//tile
				foreach(var item in worldEvent)
				{
					rom.WriteByte(position++, item.Value);
				}				
				rom.WriteByte(position++, 0xFF);

				//fix pointers
				foreach(var item in eventPointers)
				{
					if (i < item[0].Length && item[0][i] != 0)
					{
						rom.WriteWord(item[0][i] + item[1][1], (ushort)position);
					}	
				}
				
				//position
				foreach(var item in worldEvent)
				{
					rom.WriteWordSwap(position, (ushort)(item.Key + 0x9800));
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
			var result = new WorldPath[overWorld ? 8 : 43];
			
			rom.SetBank(8);	
			for(int level = 0 ; level < result.Length ; level++)
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
				
				for(int dir = 0 ; dir < 4 ; dir++)
				{			
					var direction = new WorldPathDirection
					{
						Progress = 0xFD, 
						Path = new List<WorldPathSegment>(), 
						Next = level 
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
								
								direction.Path.Add(new WorldPathSegment { Status = status, Direction = Array.IndexOf(directions, data), Steps = steps });
															
								position += 3;
								data = rom.ReadByte(position);
							}
							
							int next = rom.ReadByte(position + 1);
							direction.Next = next;
													
							int progress = rom.ReadWord((overWorld ? 0x6486 : 0x6496) + level * 2);
							direction.Progress = rom.ReadByte(progress + 1 + dir * 2);							
						}
					}
					
					dirs[dir] = direction;
				}
				
				result[level] = new WorldPath { X = posX, Y = posY, Directions = dirs };
			}
			
			if (overWorld)
			{
				LoadFlags(rom, result);
			}
			else				
			{
				LoadTreasures(rom, result);
			}
			
			return result;		
		}
		
		public static bool SavePaths(Rom rom, WorldPath[] pathData, bool overWorld, out string errorMessage)
		{			
			int[][] duplicates = new int[][]
			{
				new int[] { 7, 23 },  // rice beach 1 / flooded
				new int[] { 14, 36 }, // rice beach 3 / flooded
				new int[] { 5, 10 },  // mt teapot 4 / crushed
				new int[] { 38, 42 }, // parsley woods 1 / flooded				
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
			int levelCount = pathData.Count(x => x!= null);
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
			
			for (int level = 0 ; level < pathData.Length ; level++)
			{												
				var item = pathData[level];	
				if (item != null)
				{
					var dirs = item.Directions;
										
					//header
					rom.WriteWord((overWorld ? 0x43B8 : 0x5629) + level * 2, (ushort)positionDir);
					
					//direction flag
					int dirFlag = 0;
					for (int dir = 0 ; dir < 4 ; dir++)
					{
						if (dirs[dir].Path.Count > 0)
						{
							dirFlag |= 0x10 << dir;
						}
					}
					
					rom.WriteByte(positionDir++, (byte)dirFlag);					
										
					for (int dir = 0 ; dir < 4 ; dir++)
					{
						var direction = dirs[dir];
						if (direction.Path.Count > 0)
						{						
							//write headers							
							rom.WriteByte(positionDir++, (byte)((positionDirHeader - (overWorld ? 0x43F0 : 0x5760)) / 2));
							rom.WriteWord(positionDirHeader, (ushort)positionPath);
			              	positionDirHeader += 2;
			              	
			              	foreach(var path in direction.Path)
			              	{
			              		rom.WriteByte(positionPath++, (byte)directions[path.Direction]);
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
						
			SaveProgression(rom, pathData, overWorld);
			SaveLevelsPosition(rom, pathData, overWorld);
						
			if(overWorld)
			{
				SaveFlags(rom, pathData);
			}
			else
			{
				SaveTreasures(rom, pathData);
			}
						
			errorMessage = string.Empty;
			return true;
		}
		
		static void SaveProgression(Rom rom, WorldPath[] pathData, bool overWorld)
		{
			rom.SetBank(8);
			for (int level = 0 ; level < pathData.Length ; level++)
			{							
				var item = pathData[level];
				for (int dir = 0 ; dir < 4 ; dir++)
				{
					var direction = item.Directions[dir];
					int pointer = rom.ReadWord((overWorld ? 0x6486 : 0x6496) + level * 2);						
					rom.WriteByte(pointer + 1 + dir * 2, (byte)direction.Progress);
				}
			}
		}
		
		static void SaveLevelsPosition(Rom rom, WorldPath[] pathData, bool overWorld)
		{
			rom.SetBank(8);
			for (int level = 0 ; level < pathData.Length ; level++)
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
		
		#region Flags
		
		public static void LoadFlags(Rom rom, WorldPath[] pathData)
		{
			rom.SetBank(0x14);
			for (int level = 0 ; level < flagsPosition.Length ; level++)
			{
				var item = pathData[level];
				item.FlagX = rom.ReadByte(flagsPosition[level][0] + 1) - 12;
				item.FlagY = rom.ReadByte(flagsPosition[level][1] + 1) - 20;
			}
		}
		
		public static void SaveFlags(Rom rom, WorldPath[] pathData)
		{
			rom.SetBank(0x14);
			for (int level = 0 ; level < flagsPosition.Length ; level++)
			{
				var item = pathData[level];
				rom.WriteByte(flagsPosition[level][0] + 1, (byte)Math.Max(0, Math.Min(255, item.FlagX + 12)));
				rom.WriteByte(flagsPosition[level][1] + 1, (byte)Math.Max(0, Math.Min(255, item.FlagY + 20)));
			}
		}					
		
		#endregion
		
		#region Treasures

		static void LoadTreasures(Rom rom, WorldPath[] pathData)
		{
			rom.SetBank(0x14);
			for (int level = 0 ; level < pathData.Length ; level++)
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
		
		static void SaveTreasures(Rom rom, WorldPath[] pathData)
		{
			rom.SetBank(0x14);
			for (int level = 0 ; level < pathData.Length ; level++)
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
		
		#endregion
	}
}
