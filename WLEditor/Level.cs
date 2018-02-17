using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WLEditor
{
	public static class Level
	{
		public static Color[][] palettes =
		{
			new[] { Color.FromArgb(255, 224, 248, 208), Color.FromArgb(255, 135, 201, 140), Color.FromArgb(255, 52, 104, 86), Color.FromArgb(255, 8, 24, 32) },
			new[] { Color.White, Color.FromArgb(255, 170, 170, 170), Color.FromArgb(255, 85, 85, 85), Color.Black },
			new[] { Color.White, Color.FromArgb(255, 230, 214, 156), Color.FromArgb(255, 180, 165, 106), Color.FromArgb(255, 57, 56, 41) }			
		};
		
		public static Color[] enemyPalette =
		{
			Color.White, Color.FromArgb(255, 157, 201, 253), Color.FromArgb(255, 50, 50, 155), Color.FromArgb(255, 0, 0, 41)
		};

		public static string[] levelNames =
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
		
		public static Dictionary<int, int> enemiesIdsToSpriteData = new Dictionary<int, int>
		{
			{ 0x5313, 0x4FAA },
			{ 0x5323, 0x6C70 },
			{ 0x542B, 0x6D56 },
			{ 0x534B, 0x40F5 },
			{ 0x53D3, 0x7570 },
			{ 0x540F, 0x6D56 },
			{ 0x5337, 0x66A0 },
			{ 0x5333, 0x66A0 },
			{ 0x5447, 0x7153 },
			{ 0x533B, 0x686C },
			{ 0x5407, 0x6F84 },
			{ 0x535F, 0x5C86 },
			{ 0x5327, 0x4DB0 },
			{ 0x533F, 0x60D1 },
			{ 0x5363, 0x5E0F },
			{ 0x53C7, 0x6BEE },
			{ 0x5443, 0x7F1F },
			{ 0x53AB, 0x736F },
			{ 0x5357, 0x4AA0 },
			{ 0x5367, 0x64D4 },
			{ 0x536B, 0x6D16 },
			{ 0x5317, 0x6FBC },
			{ 0x535B, 0x53A3 },
			{ 0x53FF, 0x6F1F },
			{ 0x538B, 0x460F },
			{ 0x537B, 0x7C56 },
			{ 0x53D7, 0x7B10 },
			{ 0x53CF, 0x72BF },
			{ 0x53EB, 0x6B39 },
			{ 0x5347, 0x4035 },
			{ 0x5433, 0x7992 },
			{ 0x53A7, 0x6E87 },
			{ 0x537F, 0x435D },
			{ 0x53CB, 0x6F27 },
			{ 0x53B7, 0x64BA },
			{ 0x5383, 0x4D26 },
			{ 0x53B3, 0x6082 },
			{ 0x5427, 0x776D },
			{ 0x540B, 0x7338 },
			{ 0x541B, 0x7567 },
			{ 0x53FB, 0x7ABF },
			{ 0x53EF, 0x7AB7 },
			{ 0x53F7, 0x7AC3 },
			{ 0x53F3, 0x7ABB },
			{ 0x5387, 0x5ECA },
			{ 0x5403, 0x6D59 },
			{ 0x5343, 0x6373 },
			{ 0x5353, 0x459E },
			{ 0x543F, 0x7DE6 },
			{ 0x5417, 0x730D },
			{ 0x531B, 0x74E9 },
			{ 0x5423, 0x778B },
			{ 0x531F, 0x6B4E },
			{ 0x534F, 0x4308 },
			{ 0x538F, 0x54F8 },
			{ 0x542F, 0x796C },
			{ 0x53BF, 0x6933 },
			{ 0x53C3, 0x6933 },
			{ 0x5437, 0x7572 },
			{ 0x543B, 0x7C24 },
			{ 0x53DB, 0x4F33 },
			{ 0x5413, 0x6D8E },
			{ 0x541F, 0x7B16 },
			{ 0x532F, 0x5BAF },
			{ 0x53AF, 0x75BA },
			{ 0x5373, 0x6C2F },			
			{ 0x536F, 0x6726 }
		};
		
		public static byte[] levelData;
		public static byte[] objectsData;
		public static byte[] scrollData;
		public static byte[] warps;
		public static int warioPosition;
		public static Rectangle[] loadedSprites;
		
		public static void DumpLevel(Rom rom, int course, int warp, DirectBitmap tiles8x8, DirectBitmap tiles16x16, DirectBitmap tilesEnemies, bool reloadAll, bool switchA, bool switchB, int paletteIndex)
		{
			rom.SetBank(0xC);
			int header = rom.ReadWord(0x4560 + course * 2);

			int tilebank = rom.ReadByte(header + 0);
			int tileaddressB = rom.ReadWord(header + 1);
			int tileaddressA = rom.ReadWord(header + 3);
			int tileanimated = rom.ReadWord(header + 7);

			int blockbank = rom.ReadByte(header + 9);
			int blockubbank = rom.ReadByte(header + 10);
			int blockindex = rom.ReadWord(header + 11);
			byte palette = rom.ReadByte(header + 27);
			int enemiesIdsPointer, enemiesTiles;
			
			//load warp
			if(warp != -1)
			{
				tilebank = rom.ReadByte(warp + 11);
				tileaddressB = rom.ReadWord(warp + 12);
				tileaddressA = rom.ReadWord(warp + 14);
				tileanimated = rom.ReadWord(warp + 18);
				blockindex = rom.ReadWord(warp + 20);
				palette = rom.ReadByte(warp + 10);
				FindEnemiesData(rom, rom.ReadWord(warp + 22), out enemiesIdsPointer, out enemiesTiles);
			}
			else
			{
				FindEnemiesData(rom, rom.ReadWord(header + 28), out enemiesIdsPointer, out enemiesTiles);
			}
			
			Color[] paletteColors = palettes[paletteIndex];

			//dump 8x8 blocks
			Array.Clear(tiles8x8.Bits, 0, tiles8x8.Width * tiles8x8.Height);
			rom.SetBank(0x11);
			Dump8x8Tiles(rom, tileaddressA, tiles8x8, 2*16, 0*16, palette, paletteColors);
			rom.SetBank(tilebank);
			Dump8x8Tiles(rom, tileaddressB, tiles8x8, 6*16, 2*16, palette, paletteColors);
			rom.SetBank(0x11);
			Dump8x8Tiles(rom, tileanimated, tiles8x8, 4, 2*16, palette, paletteColors);				

			//dump 16x16 blocks
			rom.SetBank(0xB);
			Dump16x16Tiles(rom, blockindex, tiles8x8, tiles16x16, switchA, switchB, paletteColors[0]);
			
			DumpEnemiesSprites(rom, enemiesIdsPointer, enemiesTiles, tiles8x8, tilesEnemies);			
			
			if(reloadAll)
			{
				//dump level
				rom.SetBank(blockbank);
				int tilesdata = rom.ReadWord(0x4000 + blockubbank * 2);
				levelData = RLEDecompressTiles(rom, tilesdata).Take(0x3000).ToArray();

				//dump scroll
				rom.SetBank(0xC);
				scrollData = rom.ReadBytes(0x4000 + course * 32, 32);

				//dump objects
				rom.SetBank(0x7);
				int objectsPosition = rom.ReadWord(0x4199 + course * 2);
				objectsData = RLEDecompressObjects(rom, objectsPosition).Take(0x2000).ToArray();

				//dump warps
				rom.SetBank(0xC);
				warps = new Byte[32];
				for(int i = 0 ; i < 32 ; i++)
				{
					int warpdata = rom.ReadWord(0x4F30 + course * 64 + i * 2);
					warps[i] = rom.ReadByte(warpdata);
				}

				warioPosition = rom.ReadWordSwap(header + 15) + rom.ReadWordSwap(header + 13) * 8192;								
			}
		}

		public static int SearchWarp(Rom rom, int course, int sector)
		{
			rom.SetBank(0xC);
			int warp = rom.ReadWord(0x4F30 + course * 64 + sector * 2);
			if(rom.ReadByte(warp) != 32)
			{
				//search sector that target that sector
				for(int i = 0 ; i < 32 ; i++)
				{
					warp = rom.ReadWord(0x4F30 + course * 64 + i * 2);

					if(rom.ReadByte(warp) == sector)
					{
						return warp;
					}
				}
			}

			return -1;
		}
		
		public static void DumpBonusSprites(Rom rom, DirectBitmap tiles8x8, DirectBitmap tilesObject)
		{
			Array.Clear(tiles8x8.Bits, 0, tiles8x8.Width * tiles8x8.Height);
			//bonus sprites
			rom.SetBank(0x5);
			Dump8x8Tiles(rom, 0x4C81, tiles8x8, 23, 0, 0x1E, enemyPalette, true); 
			
			rom.SetBank(0x11);
			Dump8x8Tiles(rom, 0x7300, tiles8x8, 4, 16+23, 0x1E, enemyPalette, true); 
			
			rom.SetBank(0xF);
			int[] sprites = { 0x4ACB, 0x4AEB, 0x4ADB, 0x4AFB, 0x4B0B, 0x4B1B, 0x4B2B, 0x4B4B, 0x4B5B };
			for (int i = 0 ; i < sprites.Length ; i++)
			{			
				DumpSprite(rom, 8 + i * 16, 16, sprites[i], 0, tiles8x8, tilesObject);
			}
		}

		public static IEnumerable<byte> RLEDecompressTiles(Rom rom, int tilesdata)
		{
			while(true)
			{
				byte data = rom.ReadByte(tilesdata++);
				if((data & 0x80) == 0x80)
				{
					data = (byte)(data & 0x7F);
					byte rep = rom.ReadByte(tilesdata++);
					for(int j = 0 ; j <= rep ; j++)
					{
						yield return data;
					}
				}
				else
				{
					yield return data;
				}
			}
		}

		public static IEnumerable<byte> RLECompressTiles(byte[] levelData)
		{
			int current = 0;
			while(current < levelData.Length)
			{
				byte repeat = 0;
				byte data = levelData[current++];
				while(current < levelData.Length && levelData[current] == data && repeat < 255 && current % 256 != 0)
				{
					current++;
					repeat++;
				}

				if(repeat > 0)
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

		public static IEnumerable<byte> RLEDecompressObjects(Rom rom, int enemiesData)
		{
			while(true)
			{
				byte data = rom.ReadByte(enemiesData++);
				yield return (byte)(data >> 4);
				yield return (byte)(data & 0xF);

				byte repeat = rom.ReadByte(enemiesData++);
				for(int i = 0 ; i < repeat ; i++)
				{
					yield return 0;
					yield return 0;
				}
			}
		}

		public static IEnumerable<byte> RLECompressObjectsHelper(byte[] levelData)
		{
			for(int i = 0 ; i < levelData.Length ; i+=2)
			{
				yield return (byte)((levelData[i] << 4) | levelData[i+1]);
			}
		}

		public static IEnumerable<byte> RLECompressObjects(byte[] levelData)
		{
			byte[] halfData = RLECompressObjectsHelper(levelData).ToArray();

			int current = 0;
			while(current < halfData.Length)
			{
				byte data = halfData[current++];
				yield return data;

				byte count = 0;
				while(current < halfData.Length && halfData[current] == 0 && count < 255)
				{
					current++;
					count++;
				}
				yield return count;
			}
		}

		public static void Dump16x16Tiles(Rom rom, int tileindexaddress, DirectBitmap gfx8, DirectBitmap gfx16, bool switchA, bool switchB, Color defaultColor)
		{	
			for(int n = 0 ; n < 16 ; n++)
			{
				for(int i = 0 ; i < 8 ; i++)
				{
					int tileIndex = i + n * 8;
					tileIndex = Level.Switch(tileIndex, switchA, switchB);

					for(int k = 0 ; k < 2 ; k++)
					{
						for(int j = 0 ; j < 2 ; j++)
						{
							byte subTileIndex = rom.ReadByte(tileindexaddress + tileIndex * 4 + k * 2 + j);
							Point dest = new Point(j * 8 + i * 16, k * 8 + n * 16);

							if(subTileIndex < 128)
							{
								Point source = new Point((subTileIndex % 16) * 8, (subTileIndex / 16) * 8);
															
								for(int y = 0 ; y < 8 ; y++)
								{
									Array.Copy(gfx8.Bits, source.X + (source.Y + y) * gfx8.Width, gfx16.Bits, dest.X + (dest.Y + y) * gfx16.Width, 8);
								}
							}
							else
							{
								int defaultColorInt = defaultColor.ToArgb();								
								for(int y = 0 ; y < 8 ; y++)
								{
									int destIndex = dest.X + (dest.Y + y) * gfx16.Width;									
									for(int x = 0 ; x < 8 ; x++)
									{
										gfx16.Bits[destIndex + x] = defaultColorInt;
									}
								}
							}
						}
					}
				}
			}
		}

		public static void Dump8x8Tiles(Rom rom, int gfxaddress, DirectBitmap gfx8, int tiles, int pos, byte palette, Color[] customPalette, bool transparency = false)
		{
			for(int n = 0 ; n < tiles ; n++)
			{
				for(int j = 0 ; j < 8 ; j++)
				{
					byte data0 = rom.ReadByte(gfxaddress++);
					byte data1 = rom.ReadByte(gfxaddress++);

					for(int i = 0 ; i < 8 ; i++)
					{
						int pixelA = (data0 >> (7 - i)) & 0x1;
						int pixelB = (data1 >> (7 - i)) & 0x1;
						int pixel = pixelA + pixelB * 2;
						
						if(!transparency || pixel != 0)
						{
							int palindex = (palette >> pixel * 2) & 0x3;

							int x = i + ((n + pos) % 16) * 8;
							int y = j + ((n + pos) / 16) * 8;
							gfx8.Bits[x + gfx8.Width * y] = customPalette[palindex].ToArgb();
						}
					}
				}
			}
		}
		
		public static void FindEnemiesData(Rom rom, int position, out int enemyId, out int tilesPointer)
		{
			byte[] pattern = { 
				0x3E, 0x00,       //ld  a, ..
				0xEA, 0x2F, 0xA3, //ld  (A32F),a
				0x3E, 0x00,       //ld  a, ..
				0xEA, 0x30, 0xA3, //ld  (A330),a
				0x01, 0x00, 0x00, //ld  bc, ..
				0xCD, 0xA7, 0x40  //call 40A7
			};
			
			rom.SetBank(0x7);
			while(rom.ReadByte(position) != 0xC9) //ret
			{				
				bool match = true;
				for(int j = 0 ; j < pattern.Length ; j++)
				{
					byte valueFromRom = rom.ReadByte(position + j);
					byte valueToCompare = pattern[j];
					
					if(valueToCompare != 0x00 && valueFromRom != valueToCompare)
					{
						match = false;
						break;
					}
				}
				
				if(match)
				{
					enemyId = rom.ReadByte(position + 6) << 8 | rom.ReadByte(position + 1);
					tilesPointer = rom.ReadByte(position + 12) << 8 | rom.ReadByte(position + 11);
					return;
				}
				
				position++;
			}	

			throw new Exception("cannot find enemies data");
		}
		
		public static Rectangle DumpSprite(Rom rom, int posx, int posy, int spriteAddress, int startIndex, DirectBitmap tiles8x8, DirectBitmap tilesDest)
		{						
			Rectangle rectangle = Rectangle.Empty;
			int pos = spriteAddress;
						
			pos = rom.ReadWord(pos);
			rom.ReadByte(pos++);
			
			while(rom.ReadByte(pos) != 0x80)
			{		
				int spx, spy;
				unchecked
				{
					spy = (sbyte)rom.ReadByte(pos++) + posy;
					spx = (sbyte)rom.ReadByte(pos++) + posx;
				}
				int spriteData = rom.ReadByte(pos++);
				int spriteIndex = (spriteData & 0x3F) + startIndex;
				Point source = new Point((spriteIndex % 16) * 8, (spriteIndex / 16) * 8);
				
				Rectangle tileRect = new Rectangle(spx, spy, 8, 8);
				rectangle = rectangle == Rectangle.Empty ? tileRect : Rectangle.Union(rectangle, tileRect);
				
				Func<int, int> getX, getY;
				
				if((spriteData & 0x40) != 0) //horizontal flip
				{
					getX = x => 7 - x;
				}
				else
				{
					getX = x => x;
				}
				
				if((spriteData & 0x80) != 0) //vertical flip
				{
					getY = y => 7 - y;				
				}
				else
				{
					getY = y => y;
				}				
				
				for(int y = 0 ; y < 8 ; y++)
				{												
					int src = source.X + (source.Y + getY(y)) * tiles8x8.Width;
					int dst = spx + (spy + y) * tilesDest.Width;
					
					for(int x = 0 ; x < 8 ; x++)
					{											
						tilesDest.Bits[dst + x] = tiles8x8.Bits[src + getX(x)];
					}
				}									
			}
			
			return rectangle;
		}
		
		public static void DumpEnemiesSprites(Rom rom, int enemiesIdsPointer, int enemiesTiles, DirectBitmap tiles8x8, DirectBitmap tilesEnemies)
		{
			loadedSprites = new Rectangle[6];
			
			bool quit = false;

			int tilePos = 16*8;		
			int enemyIndex = 0;
			Array.Clear(tilesEnemies.Bits, 0, tilesEnemies.Width * tilesEnemies.Height);

			do
			{
				rom.SetBank(0x7);
				int bank = rom.ReadByte(enemiesTiles++);
				int address = rom.ReadWord(enemiesTiles++); enemiesTiles++;
				int num = rom.ReadByte(enemiesTiles++);			
						
				if(num != 0xFF)
				{							
					int spriteDataAddress;
					rom.SetBank(0x7);
					int enemiesId = rom.ReadWord(enemiesIdsPointer + (enemyIndex + 1) * 2);
					
					if(enemiesIdsToSpriteData.TryGetValue(enemiesId, out spriteDataAddress))
					{
						rom.SetBank(bank);	
						int pos = rom.ReadWord(spriteDataAddress);
						int spriteFlags = rom.ReadByte(pos);
						int palette = (spriteFlags & 0x10) != 0 ? 0xD1 : 0x1E;
						
						Dump8x8Tiles(rom, address, tiles8x8, num, tilePos, (byte)palette, enemyPalette, true);
						Rectangle rectangle = DumpSprite(rom, 32, 56 + enemyIndex * 64, spriteDataAddress, tilePos, tiles8x8, tilesEnemies);
						loadedSprites[enemyIndex] = rectangle;
					}	
					
					tilePos += num;
					enemyIndex++;					
				}
				else
				{
					quit = true;
				}
			}
			while(!quit);
		}
		
		public static List<KeyValuePair<int, int>> GetCourseIds(Rom rom)
		{
			//convert course id => course no using data in ROM
			rom.SetBank(0);
			var courseIdToNo = new List<KeyValuePair<int, int>>();
			for(int i = 0 ; i <= 0x2A ; i++)
			{
				int levelpointer = rom.ReadWord(0x0534 + i * 2);
				int courseNo = (levelpointer - 0x0587) / 3;
				courseIdToNo.Add(new KeyValuePair<int, int>(i, courseNo));
			}
			
			return courseIdToNo;
		}

		public static bool IsCollidable(int tileIndex)
		{
			return (tileIndex & 64) != 64;
		}

		//replace tiles when a (!) block is hit
		public static int Switch(int tileData, bool switchA, bool switchB)
		{
			if(switchA)
			{
				switch(tileData)
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
			else if(switchB)
			{
				switch(tileData)
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

			return tileData;
		}

		public static bool SaveChanges(Rom rom, int course, string filePath, out string errorMessage)
		{
			//rom expansion give new banks for level data
			rom.ExpandTo1MB();

			SaveBlocksToRom(rom, course);
			if(!SaveObjectsToRom(rom, course, out errorMessage))
			{
				return false;
			}

			rom.FixCRC();
			rom.Save(filePath);
			return true;
		}

		public static void SaveBlocksToRom(Rom rom, int course)
		{
			//grab all levels data at once
			byte[][] allTiles = new byte[0x2B][];

			for(int i = 0 ; i < 0x2B ; i++)
			{
				rom.SetBank(0xC);
				int headerposition = rom.ReadWord(0x4560 + i * 2);
				int tilebank = rom.ReadByte(headerposition + 9);
				int tilesubbank = rom.ReadByte(headerposition + 10);

				rom.SetBank(tilebank);
				int tilePosition = rom.ReadWord(0x4000 + tilesubbank * 2);
				byte[] tileData = RLEDecompressTiles(rom, tilePosition).Take(0x3000).ToArray();
				allTiles[i] = tileData;
			}

			Array.Copy(levelData, allTiles[course], 0x3000);

			//write them back to ROM
			byte bank = 0x20;
			byte subbank = 0;
			int writePosition = 0x4040; //first 64 bytes are reserved for level pointers
			for(int i = 0 ; i < 0x2B ; i++)
			{
				byte[] tileData = RLECompressTiles(allTiles[i]).ToArray();
				if((writePosition + tileData.Length) >= 0x8000 || subbank >= 32)
				{
					//no more space, switch to another bank
					bank++;
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

				subbank++;
				writePosition += tileData.Length;
			}
		}

		public static bool SaveObjectsToRom(Rom rom, int course, out string errorMessage)
		{
			byte[][] enemyData = new byte[0x2B][];

			//save objects
			rom.SetBank(0x7);
			for(int i = 0 ; i < 0x2B ; i++)
			{
				int objectsPos = rom.ReadWord(0x4199 + i * 2);
				enemyData[i] = RLEDecompressObjects(rom, objectsPos).Take(0x2000).ToArray();
			}
			enemyData[course]= objectsData;

			int objectSize = enemyData.Sum(x => RLECompressObjects(x).Count());
			if(objectSize > 4198)
			{
				errorMessage = string.Format("Object data is too big to fit in ROM.\r\nPlease remove at least {0} byte(s).", objectSize - 4198);
				return false;
			}

			int startPos = rom.ReadWord(0x4199);
			for(int i = 0 ; i < 0x2B ; i++)
			{
				rom.WriteWord(0x4199 + i * 2, (ushort)startPos);

				byte[] data = RLECompressObjects(enemyData[i]).ToArray();
				rom.WriteBytes(startPos, data);
				startPos += data.Length;
			}

			errorMessage = string.Empty;
			return true;
		}
	}
}
