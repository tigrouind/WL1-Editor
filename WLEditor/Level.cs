using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WLEditor
{
	public static class Level
	{
		static uint[] paletteColors =  { 0xFFFFFFFF, 0xFFAAAAAA, 0xFF555555, 0xFF000000 };
		
		static uint[] enemyPalette = { 0xFFFFFFFF, 0xFF9DC9FD, 0xFFFF00FF, 0xFF000029 };
				
		static ushort[] enemySpriteAddress = 
		{
			0x4FAA, 0x6FBC, 0x74E9, 0x6B4E, 0x6C70, 0x4DB0, 0x0000, 0x5BAF, 0x66A0, 0x66A0, 0x686C, 0x60D1, 0x6373, 0x4035, 0x40F5, 
			0x4308, 0x459E, 0x4AA0, 0x53A3, 0x5C86, 0x5E0F, 0x64D4, 0x6D16, 0x6726, 0x6C2F, 0x7C56, 0x7C56, 0x435D, 0x4D26, 0x5ECA, 0x460F, 
			0x54F8, 0x5536, 0x0000, 0x6B57, 0x6DFC, 0x6DE4, 0x6E87, 0x736F, 0x75BA, 0x6082, 0x64BA, 0x67BF, 0x6933, 0x6933, 0x6BEE, 0x6F27, 
			0x72BF, 0x7570, 0x7B10, 0x4F33, 0x42D0, 0x45AC, 0x45BC, 0x6B39, 0x7AB7, 0x7ABB, 0x7AC3, 0x7ABF, 0x6F1F, 0x6D59, 0x6F84, 0x7338, 
			0x6D56, 0x6D8E, 0x730D, 0x7567, 0x7B16, 0x778B, 0x776D, 0x6D56, 0x796C, 0x7992, 0x7572, 0x7C24, 0x7DE6, 0x7F1F, 0x7153
		};
		
		static ushort[] bonusSpriteAddress = { 0x4ACB, 0x4AEB, 0x4ADB, 0x4AFB, 0x4B0B, 0x4B1B, 0x4B2B, 0x4B4B, 0x4B5B };
		
		static byte[] enemyCodePattern = 
		{
			0x3E, 0x00,       //ld  a, ..
			0xEA, 0x2F, 0xA3, //ld  (A32F),a
			0x3E, 0x00,       //ld  a, ..
			0xEA, 0x30, 0xA3, //ld  (A330),a
			0x01, 0x00, 0x00, //ld  bc, ..
			0xCD, 0xA7, 0x40  //call 40A7
		};
		
		static byte[] treasurePattern = 
		{
			0x3E, 0x00,      // ld a, ..
			0xEA, 0x82, 0xA3 // ld (A382), a
		};
		
		static byte[] treasureCheckPattern = 
		{
			0xCD, 0x6A, 0x42 //call 426A
		};
		
		static byte[] exitPattern = 
		{
			0x3E, 0x02,      // ld a, 02
			0xEA, 0x76, 0xA3 // ld (A376), a
		};
		
		static byte[] bonusPattern = 
		{
			0x3E, 0x00,      // ld a, ..
			0xEA, 0xB6, 0xA3 // ld (A3B6), a
		};		
		
		public static byte[] LevelData = new byte[0x3000];
		public static byte[] ObjectsData = new byte[0x2000];
		public static byte[] ScrollData = new byte[32];
		public static byte[] Warps = new byte[32];
		public static int WarioPosition;
		public static int CameraPosition;
		public static Rectangle[] LoadedSprites = new Rectangle[6];
		public static bool[] Animated16x16Tiles = new bool[16 * 8];
		private static int[] animated8x8Tiles = new int[16 * 8 * 2 * 2];
		public static int AnimatedTilesMask;
		public static bool WarioRightFacing;
		public static Rectangle[] PlayerRectangles = new Rectangle[2];
		
		public static DirectBitmap Tiles8x8 = new DirectBitmap(16 * 8, 16 * 8); 
		public static DirectBitmap Tiles16x16 = new DirectBitmap(16 * 8, 16 * 16); 
		public static DirectBitmap TilesObjects = new DirectBitmap(16 * 9, 16);
		public static DirectBitmap TilesEnemies = new DirectBitmap(64, 128 * 6); 		
		public static DirectBitmap PlayerSprite = new DirectBitmap(64, 64 * 2);
		
		public static void DumpLevel(Rom rom, int course, int warp, bool reloadAll, int switchMode, int animatedTileIndex, bool reloadAnimatedTilesOnly)
		{			
			int tilebank;
			int tileaddressB;
			int tileaddressA;
			int tileanimated;			
			int blockindex;			
			byte palette;
			int enemiesData;
			
			rom.SetBank(0xC);
			int header = rom.ReadWord(0x4560 + course * 2);
						
			if(warp != -1)
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
							
			if(!reloadAnimatedTilesOnly)
			{
				int enemiesIdsPointer, enemiesTiles, treasureID;
				bool exitOpen, treasureCheck, bonus;
				
				FindEnemiesData(rom, enemiesData, out enemiesIdsPointer, out enemiesTiles, out treasureID, out treasureCheck, out exitOpen, out bonus);
				DumpEnemiesSprites(rom, enemiesIdsPointer, enemiesTiles);	

				//dump 8x8 tiles
				Array.Clear(Tiles8x8.Bits, 0, Tiles8x8.Width * Tiles8x8.Height);
				rom.SetBank(0x11);
				Dump8x8Tiles(rom, tileaddressA, 2*16, 0*16, palette, paletteColors, false);
				rom.SetBank(tilebank);
				Dump8x8Tiles(rom, tileaddressB, 6*16, 2*16, palette, paletteColors, false);				

				//dump scroll
				rom.SetBank(0xC);
				rom.ReadBytes(0x4000 + course * 32, 32, ScrollData);
				
				//dump warps
				rom.SetBank(0xC);				
				for(int i = 0 ; i < 32 ; i++)
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
						
			if(AnimatedTilesMask != 0)
			{
				rom.SetBank(0x11);
				Dump8x8Tiles(rom, tileanimated + animatedTileIndex * 16 * 4, 4, 2*16, palette, paletteColors, false);			
			}				

			//dump 16x16 tiles			
			if(reloadAnimatedTilesOnly)
			{
				rom.SetBank(0x11);
				DumpAnimated16x16Tiles(paletteColors[0]);
			}
			else
			{
				rom.SetBank(0xB);
				Dump16x16Tiles(rom, blockindex, switchMode, paletteColors[0]);
			}

			if(reloadAll)
			{						
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
		}
				
		public static Warp GetLevelHeader(Rom rom, int course)
		{
			rom.SetBank(0xC);
			int header = rom.ReadWord(0x4560 + course * 2);
			
			Warp warp = new Warp
			{
				Bank = rom.ReadByte(header + 0),
				TileSetB = rom.ReadWord(header + 1),
				TileSetA = rom.ReadWord(header + 3),
				TileAnimation = rom.ReadWord(header + 7),						
				BlockIndex = rom.ReadWord(header + 11),
				WarioY = rom.ReadWordSwap(header + 13) / 8,
				WarioX = rom.ReadWordSwap(header + 15) / 8,	
				WarioSpriteAttributes = rom.ReadByte(header + 18),
				CameraY = rom.ReadWordSwap(header + 19) / 8,
				CameraX = rom.ReadWordSwap(header + 21) / 8,
				CameraType = rom.ReadByte(header + 24),
				AnimationSpeed = rom.ReadByte(header + 26),
				Palette = rom.ReadByte(header + 27),
				Enemy = rom.ReadWord(header + 28),
				Music = rom.ReadWord(0x7E73 + course * 2)	
			};
			
			return warp;
		}
		
		public static void SaveLevelHeader(Rom rom, int course, Warp warp)
		{			
			rom.SetBank(0xC);
			int header = rom.ReadWord(0x4560 + course * 2);
			int warioSector = warp.WarioX / 32 + warp.WarioY / 32 * 16;
			int scroll = GetScroll(rom, course, warioSector);
			
			rom.WriteByte(header + 0, (byte)warp.Bank);
			rom.WriteWord(header + 1, (ushort)warp.TileSetB);
			rom.WriteWord(header + 3, (ushort)warp.TileSetA);
			rom.WriteWord(header + 7, (ushort)warp.TileAnimation); 			
			rom.WriteWord(header + 11, (ushort)warp.BlockIndex);		
			rom.WriteWordSwap(header + 13, (ushort)(warp.WarioY * 8));
			rom.WriteWordSwap(header + 15, (ushort)(warp.WarioX * 8));
			rom.WriteByte(header + 18, (byte)warp.WarioSpriteAttributes);
			rom.WriteWordSwap(header + 19, (ushort)(warp.CameraY * 8));
			rom.WriteWordSwap(header + 21, (ushort)(warp.CameraX * 8));
			rom.WriteByte(header + 23, (byte)scroll);
			rom.WriteByte(header + 24, (byte)warp.CameraType);
			rom.WriteByte(header + 26, (byte)warp.AnimationSpeed);
			rom.WriteByte(header + 27, (byte)warp.Palette);					
			rom.WriteWord(header + 28, (ushort)warp.Enemy);  
			rom.WriteWord(0x7E73 + course * 2, (ushort)warp.Music);
		}
				
		public static int SearchWarp(Rom rom, int course, int sector)
		{
			if (sector != -1)
			{
				rom.SetBank(0xC);
				int warp = rom.ReadWord(0x4F30 + course * 64 + sector * 2);		
				if (warp >= 0x5B7A)
				{
					return warp;
				}
			}

			return -1;
		}
		
		public static int GetWarp(Rom rom, int course, int sector)
		{
			rom.SetBank(0xC);
			return rom.ReadWord(0x4F30 + course * 64 + sector * 2);		
		}
		
		public static Warp GetWarp(Rom rom, int warp)		
		{
			rom.SetBank(0xC);
			
			Warp warpInfo = new Warp();
			
			if (warp >= 0x5B7A)
			{
				int warioSector = rom.ReadByte(warp);
				int cameraSector = rom.ReadByte(warp+4);
				
				warpInfo.WarioY = rom.ReadByte(warp+1) / 8 + (warioSector / 16) * 32;
				warpInfo.WarioX = rom.ReadByte(warp+2) / 8 + (warioSector % 16) * 32;
				warpInfo.CameraY = rom.ReadByte(warp+5) / 8 + (cameraSector / 16) * 32;
				warpInfo.CameraX = rom.ReadByte(warp+6) / 8 + (cameraSector % 16) * 32;
				warpInfo.CameraType = rom.ReadByte(warp+7);
				warpInfo.AnimationSpeed = rom.ReadByte(warp + 9);
				warpInfo.Palette = rom.ReadByte(warp + 10);
				warpInfo.Bank = rom.ReadByte(warp + 11);
				warpInfo.TileSetB = rom.ReadWord(warp + 12);
				warpInfo.TileSetA = rom.ReadWord(warp + 14);
				warpInfo.TileAnimation = rom.ReadWord(warp + 18);
				warpInfo.BlockIndex = rom.ReadWord(warp + 20);
				warpInfo.Enemy = rom.ReadWord(warp + 22);
			}
			
			return warpInfo;				
		}
		
		public static void SaveWarp(Rom rom, int course, int sector, int warp)
		{
			rom.SetBank(0xC);	
			rom.WriteWord(0x4F30 + course * 64 + sector * 2, (ushort)warp);
		}
		
		public static void SaveWarp(Rom rom, int course, int sector, Warp warpInfo)
		{			
			rom.SetBank(0xC);
			int warp = rom.ReadWord(0x4F30 + course * 64 + sector * 2);
			rom.WriteByte(warp, (byte)(warpInfo.WarioX / 32 + (warpInfo.WarioY / 32) * 16));
			rom.WriteByte(warp + 1, (byte)((warpInfo.WarioY % 32) * 8));
			rom.WriteByte(warp + 2, (byte)((warpInfo.WarioX % 32) * 8));
			rom.WriteByte(warp + 4, (byte)(warpInfo.CameraX / 32 + (warpInfo.CameraY / 32) * 16));
          	rom.WriteByte(warp + 5, (byte)((warpInfo.CameraY % 32) * 8));
            rom.WriteByte(warp + 6, (byte)((warpInfo.CameraX % 32) * 8));
			rom.WriteByte(warp + 7, (byte)warpInfo.CameraType);
			rom.WriteByte(warp + 9, (byte)warpInfo.AnimationSpeed);
			rom.WriteByte(warp + 10, (byte)warpInfo.Palette);
			rom.WriteByte(warp + 11, (byte)warpInfo.Bank);
			rom.WriteWord(warp + 12, (ushort)warpInfo.TileSetB);
			rom.WriteWord(warp + 14, (ushort)warpInfo.TileSetA);
			rom.WriteWord(warp + 18, (ushort)warpInfo.TileAnimation); 			
			rom.WriteWord(warp + 20, (ushort)warpInfo.BlockIndex);
			rom.WriteWord(warp + 22, (ushort)warpInfo.Enemy);  
		}
		
		public static HashSet<int> GetWarpUsage(Rom rom)
		{			
			HashSet<int> used = new HashSet<int>();
			
			rom.SetBank(0xC);
			for(int i = 0 ; i < 43 ;i++)
			{
				for(int j = 0 ; j < 32 ; j++)
				{
					int warp = rom.ReadWord(0x4F30 + i * 64 + j * 2);
					if (warp >= 0x5B7A)
					{
						used.Add(warp);
					}
				}
			}
			
			return used;
		}
		
		public static int GetScroll(Rom rom, int course, int sector)
		{
			rom.SetBank(0xC);
			return rom.ReadByte(0x4000 + course * 32 + sector);
		}
		
		public static void SaveScroll(Rom rom, int course, int sector, int scroll)
		{
			rom.SetBank(0xC);
			rom.WriteByte(0x4000 + course * 32 + sector, (byte)scroll);
			
			int header = rom.ReadWord(0x4560 + course * 2);
			int warioSector = rom.ReadWordSwap(header + 15) / 256 + rom.ReadWordSwap(header + 13) / 256 * 16;
			if (sector == warioSector)
			{
				rom.WriteByte(header + 23, (byte)scroll);
			}
		}
				
		public static bool FindDoorInSector(int sector, int warioX, int warioY, out int posX, out int posY)
		{
			int startX = (sector % 16) * 16;
			int startY = (sector / 16) * 16;
			int distance = int.MaxValue;
			
			posX = -1;
			posY = -1;
			
			for (int y = 0 ; y < 16 ; y++)
			{
				for (int x = 0 ; x < 16 ; x++)
				{
					int block = LevelData[(x + startX) + (y + startY) * 256 + 0x1000];
					switch (block)
					{
						case 0x2E: 
						case 0x48:
						case 0x4B:				
						case 0x54:
							int dx = (x * 2 - (warioX - 1));
							int dy = (y * 2 - (warioY - 2));
							int length = dx * dx + dy * dy;
							if (length < distance)
							{
								distance = length;
								posX = x;
								posY = y;
							}
							break;
					}
				}
			}
			
			return (posX != -1 && posY != -1);
		}
		
		public static void DumpPlayerSprite(Rom rom)
		{	
			Array.Clear(Tiles8x8.Bits, 0, Tiles8x8.Bits.Length);
			
			rom.SetBank(0x5);			
			Dump8x8Tiles(rom, 0x42F1, 64, 0, 0x1E, enemyPalette, true);			
			PlayerRectangles[0] = DumpSpritePlayer(rom, 32, 56, 0x603B, false);
			PlayerRectangles[1] = DumpSpritePlayer(rom, 32, 56 + 64, 0x603B, true);
		}
		
		public static void DumpBonusSprites(Rom rom)
		{
			Array.Clear(Tiles8x8.Bits, 0, Tiles8x8.Bits.Length);
			//bonus sprites
			rom.SetBank(0x5);
			Dump8x8Tiles(rom, 0x4C81, 23, 0, 0x1E, enemyPalette, true); 
			
			rom.SetBank(0x11);
			Dump8x8Tiles(rom, 0x7300, 4, 16+23, 0x1E, enemyPalette, true); 
			
			rom.SetBank(0xF);			
			for (int i = 0 ; i < bonusSpriteAddress.Length ; i++)
			{			
				DumpSprite(rom, 8 + i * 16, 16, bonusSpriteAddress[i], TilesObjects);
			}
		}

		static void RLEDecompressTiles(Rom rom, int tilesdata, byte[] decompressed)
		{
			int position = 0;
			
			while(position < decompressed.Length)
			{
				byte data = rom.ReadByte(tilesdata++);
				if((data & 0x80) == 0x80)
				{
					data = (byte)(data & 0x7F);
					byte rep = rom.ReadByte(tilesdata++);
					for(int j = 0 ; j <= rep ; j++)
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

		static IEnumerable<byte> RLECompressTiles(byte[] tilesdata)
		{
			int current = 0;
			while(current < tilesdata.Length)
			{
				byte repeat = 0;
				byte data = tilesdata[current++];
				while(current < tilesdata.Length && tilesdata[current] == data && repeat < 255 && current % 256 != 0)
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

		static void RLEDecompressObjects(Rom rom, int enemiesData, byte[] decompressed)
		{
			int position = 0;
			
			while(position < decompressed.Length)
			{
				byte data = rom.ReadByte(enemiesData++);
				decompressed[position++] = (byte)(data >> 4);
				decompressed[position++] = (byte)(data & 0xF);

				byte repeat = rom.ReadByte(enemiesData++);
				for(int i = 0 ; i < repeat ; i++)
				{
					decompressed[position++] = 0;
					decompressed[position++] = 0;
				}
			}
		}

		static IEnumerable<byte> RLECompressObjectsHelper(byte[] data)
		{
			for(int i = 0 ; i < data.Length ; i+=2)
			{
				yield return (byte)((data[i] << 4) | data[i+1]);
			}
		}

		static IEnumerable<byte> RLECompressObjects(byte[] data)
		{
			byte[] halfData = RLECompressObjectsHelper(data).ToArray();

			int current = 0;
			while(current < halfData.Length)
			{
				byte value = halfData[current++];
				yield return value;

				byte count = 0;
				while(current < halfData.Length && halfData[current] == 0 && count < 255)
				{
					current++;
					count++;
				}
				yield return count;
			}
		}
		
		static void DumpAnimated16x16Tiles(uint defaultColor)
		{
			int m = 0;
			for(int n = 0 ; n < 16 ; n++)
			{
				for(int i = 0 ; i < 8 ; i++)
				{				
					for(int k = 0 ; k < 2 ; k++)
					{
						for(int j = 0 ; j < 2 ; j++)
						{							
							int subTileIndex = animated8x8Tiles[m++];
							if(subTileIndex != -1)
							{
								Point dest = new Point(j * 8 + i * 16, k * 8 + n * 16);
								Dump8x8Tile(dest, subTileIndex, defaultColor);	
							}							
						}
					}
				}
			}
		}

		static void Dump16x16Tiles(Rom rom, int tileindexaddress, int switchMode, uint defaultColor)
		{						
			int m = 0;
			for(int n = 0 ; n < 16 ; n++)
			{
				for(int i = 0 ; i < 8 ; i++)
				{
					int tileIndex = i + n * 8;
					int newTileIndex = SwitchTile(tileIndex, switchMode);

					bool isAnimatedTile = false;
					for(int k = 0 ; k < 2 ; k++)
					{
						for(int j = 0 ; j < 2 ; j++)
						{
							byte subTileIndex = rom.ReadByte(tileindexaddress + newTileIndex * 4 + k * 2 + j);
							bool isAnimated = subTileIndex >= (2 * 16) && subTileIndex < (2 * 16 + 4);
							isAnimatedTile |= isAnimated;
							animated8x8Tiles[m++] = isAnimated ? subTileIndex : -1;
														
							Point dest = new Point(j * 8 + i * 16, k * 8 + n * 16);
							Dump8x8Tile(dest, subTileIndex, defaultColor);
						}
					}
					
					Animated16x16Tiles[tileIndex] = isAnimatedTile;
				}
			}
		}
		
		static void Dump8x8Tile(Point dest, int subTileIndex, uint defaultColor)
		{
			if(subTileIndex < 128)
			{
				Point source = new Point((subTileIndex % 16) * 8, (subTileIndex / 16) * 8);										
				for(int y = 0 ; y < 8 ; y++)
				{
					Array.Copy(Tiles8x8.Bits, source.X + (source.Y + y) * Tiles8x8.Width, Tiles16x16.Bits, dest.X + (dest.Y + y) * Tiles16x16.Width, 8);
				}
			}
			else
			{
				//fill 8x8 block with default color							
				for(int y = 0 ; y < 8 ; y++)
				{
					int destIndex = dest.X + (dest.Y + y) * Tiles16x16.Width;									
					for(int x = 0 ; x < 8 ; x++)
					{
						Tiles16x16.Bits[destIndex + x] = defaultColor;
					}
				}
			}
		}

		static void Dump8x8Tiles(Rom rom, int gfxaddress, int tiles, int pos, byte palette, uint[] customPalette, bool transparency)
		{
			for(int n = 0 ; n < tiles ; n++)
			{
				int tilePosX = ((n + pos) % 16) * 8;
				int tilePosY = ((n + pos) / 16) * 8;
								
				for(int y = 0 ; y < 8 ; y++)
				{
					byte data0 = rom.ReadByte(gfxaddress++);
					byte data1 = rom.ReadByte(gfxaddress++);
					int destIndex = tilePosX + (y + tilePosY) * Tiles8x8.Width;

					for(int x = 0 ; x < 8 ; x++)
					{
						int pixelA = (data0 >> (7 - x)) & 0x1;
						int pixelB = (data1 >> (7 - x)) & 0x1;
						int pixel = pixelA + pixelB * 2;
						
						if(!transparency || pixel != 0)
						{
							int palindex = (palette >> pixel * 2) & 0x3;
							Tiles8x8.Bits[destIndex + x] = customPalette[palindex];
						}
					}
				}
			}
		}
		
		public static List<int> GetEnemyIds(Rom rom, int enemyPointer)
		{			
			rom.SetBank(0x7);
			List<int> enemyIds = new List<int>();
			for(int i = 0 ; i < 6 ; i++)
			{
				int enemyId = rom.ReadWord(enemyPointer + (i + 1) * 2);
				enemyId = (enemyId - 0x530F) / 4;	
				if (enemyId == 0)
				{
					break;
				}
				
				enemyIds.Add(enemyId);
			}
			
			return enemyIds;
		}
		
		public static void FindEnemiesData(Rom rom, int enemiesPointer, out int enemyId, out int tilesPointer, 
		                                   out int treasureID, out bool treasureCheck, out bool exitOpen, out bool bonus)
		{
			rom.SetBank(0x7);
			int position = enemiesPointer;
			while(rom.ReadByte(position) != 0xC9) //ret
			{		
				if (MatchPattern(rom, position, enemyCodePattern))
				{
					enemyId = rom.ReadByte(position + 6) << 8 | rom.ReadByte(position + 1);
					tilesPointer = rom.ReadWord(position + 11);
					
					position += enemyCodePattern.Length;
					CheckExtraCode(rom, position, out treasureID, out treasureCheck, out exitOpen, out bonus);
					return;
				}
				
				position++;
			}	

			throw new Exception("cannot find enemies data");
		}
		
		static void CheckExtraCode(Rom rom, int position, out int treasureID, out bool treasureCheck, out bool exitOpen, out bool bonus)
		{			
			treasureCheck = false;
			exitOpen = false;
			bonus = false;
			treasureID = -1;
			
			while (rom.ReadByte(position) != 0xC9) //ret
			{
				if (MatchPattern(rom, position, treasurePattern))
				{
					treasureID = rom.ReadByte(position + 1);							
					position += treasurePattern.Length;							
					
					if (MatchPattern(rom, position, treasureCheckPattern))
					{
						treasureCheck = true;
						position += treasureCheckPattern.Length;								
					}
				}
				else if (MatchPattern(rom, position, exitPattern))
				{
					exitOpen = true;
					position += exitPattern.Length;		
				}
				else if (MatchPattern(rom, position, bonusPattern))
				{
					bonus = true;
					position += bonusPattern.Length;		
				}
				else 
				{
					position++;
				}
			}
		}
		
		static bool MatchPattern(Rom rom, int position, byte[] pattern)
		{
			for(int j = 0 ; j < pattern.Length ; j++)
			{
				byte valueFromRom = rom.ReadByte(position + j);
				byte valueToCompare = pattern[j];
				
				if(valueToCompare != 0x00 && valueFromRom != valueToCompare)
				{
					return false;
				}
			}
			
			return true;
		}
		
		static Rectangle DumpSpritePlayer(Rom rom, int posx, int posy, int spriteAddress, bool horizontalFlip)
		{						
			Rectangle rectangle = Rectangle.Empty;
			int pos = spriteAddress;
			
			Func<sbyte, sbyte> hflip = x => (sbyte)(horizontalFlip ? ((~(sbyte)x) - 7) : x);
						
			pos = rom.ReadWord(pos);			
			while(rom.ReadByte(pos) != 0x80)
			{		
				int spx, spy;
				unchecked
				{
					spy = (sbyte)rom.ReadByte(pos++) + posy;
					spx = hflip((sbyte)rom.ReadByte(pos++)) + posx;
				}
				int spriteIndex = rom.ReadByte(pos++);
				int spriteFlags = rom.ReadByte(pos++) ^ (horizontalFlip ? 0x40 : 0x00);
				
				DumpSpriteInternal(spx, spy, spriteIndex, spriteFlags, PlayerSprite, ref rectangle);			
			}
			
			return rectangle;
		}
		
		static Rectangle DumpSprite(Rom rom, int posx, int posy, int spriteAddress, DirectBitmap tilesDest)
		{						
			Rectangle rectangle = Rectangle.Empty;
			int pos = spriteAddress;
						
			pos = rom.ReadWord(pos);
			pos++; //skip sprite flags
			
			while(rom.ReadByte(pos) != 0x80)
			{		
				int spx, spy;
				unchecked
				{
					spy = (sbyte)rom.ReadByte(pos++) + posy;
					spx = (sbyte)rom.ReadByte(pos++) + posx;
				}
				int spriteData = rom.ReadByte(pos++);
				int spriteIndex = (spriteData & 0x3F);
				
				DumpSpriteInternal(spx, spy, spriteIndex, spriteData, tilesDest, ref rectangle);				
			}
			
			return rectangle;
		}
		
		static void DumpSpriteInternal(int spx, int spy, int spriteIndex, int spriteFlags, DirectBitmap tilesDest, ref Rectangle rectangle)
		{
			Point source = new Point((spriteIndex % 16) * 8, (spriteIndex / 16) * 8);
				
			Rectangle tileRect = new Rectangle(spx, spy, 8, 8);
			rectangle = rectangle == Rectangle.Empty ? tileRect : Rectangle.Union(rectangle, tileRect);
			
			Func<int, int> getY;
			Func<int, int> getX;
			
			if((spriteFlags & 0x40) != 0) //horizontal flip
			{
				getX = x => 7 - x;				
			}
			else
			{
				getX = x => x;
			}	
			
			if((spriteFlags & 0x80) != 0) //vertical flip
			{
				getY = y => 7 - y;				
			}
			else
			{
				getY = y => y;
			}				
			
			for(int y = 0 ; y < 8 ; y++)
			{		
				int src = source.X + (source.Y + getY(y)) * Tiles8x8.Width;
				int dst = spx + (spy + y) * tilesDest.Width;				
					
				for(int x = 0 ; x < 8 ; x++)
				{			
					if(tilesDest.Bits[dst + x] == 0)
					{
						tilesDest.Bits[dst + x] = Tiles8x8.Bits[src + getX(x)];
					}
				}				
			}	
		}
		
		static void DumpEnemiesSprites(Rom rom, int enemiesIdsPointer, int tilesDataAddress)
		{
			Array.Clear(TilesEnemies.Bits, 0, TilesEnemies.Bits.Length);
			Array.Clear(LoadedSprites, 0, LoadedSprites.Length);
											
			for(int i = 0 ; i < 6 ; i++)
			{
				rom.SetBank(0x7);
				int enemyId = rom.ReadWord(enemiesIdsPointer + (i + 1) * 2);
				enemyId = (enemyId - 0x530F) / 4;
				
				if(enemyId > 0 && enemyId <= enemySpriteAddress.Length)
				{
					int spriteDataAddress = enemySpriteAddress[enemyId - 1];
					if(spriteDataAddress != 0)
					{		
						if (LoadEnemiesTiles(rom, i, spriteDataAddress, tilesDataAddress))
						{
							Rectangle rectangle = DumpSprite(rom, 32, 120 + i * 128, spriteDataAddress, TilesEnemies);
							LoadedSprites[i] = rectangle;
						}
					}
				}		
			}
		}		
		
		static bool LoadEnemiesTiles(Rom rom, int enemyIndex, int spriteDataAddress, int tilesDataAddress)
		{
			int index = 0;	

			do
			{
				rom.SetBank(0x7);
				int tilesBank = rom.ReadByte(tilesDataAddress++);
				int tilesAddress = rom.ReadWord(tilesDataAddress++); tilesDataAddress++;
				int tilesCount = rom.ReadByte(tilesDataAddress++);			
						
				if(tilesCount == 0xFF)
				{		
					return false;
				}
				
				if(index == enemyIndex)
				{					
					LoadEnemiesTilesInternal(rom, spriteDataAddress, tilesBank, tilesCount, tilesAddress);
					return true;
				}
				
				index++;
			}
			while(true);						
		}
		
		static void LoadEnemiesTilesInternal(Rom rom, int spriteDataAddress, int tilesBank, int tilesCount, int tilesAddress)
		{
			rom.SetBank(tilesBank);	
			int pos = rom.ReadWord(spriteDataAddress);
			int spriteFlags = rom.ReadByte(pos);
			int palette = (spriteFlags & 0x10) != 0 ? 0xD1 : 0x1E;
			
			Array.Clear(Tiles8x8.Bits, 0, Tiles8x8.Bits.Length);
			Dump8x8Tiles(rom, tilesAddress, tilesCount, 0, (byte)palette, enemyPalette, true);					
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
		
		public static int IsSpecialTile(int tileIndex)
		{
			switch(tileIndex)
			{
				case 0x2E: 
				case 0x48:
				case 0x4B:
				case 0x54:					
					return 6; //door
					
				case 0x44: 
				case 0x45:					
					return 4; //ladder		

				case 0x47: 
				case 0x46:
				case 0x53:	
				case 0x3E: 							
					return 3; //coins or sand
									
				case 0x59: 
				case 0x5A:
				case 0x5C: 	
				case 0x5D:
				case 0x5E:	
				case 0x5F:							
					return 5; //damage
					
				case 0x3A: 
				case 0x3B:
				case 0x40:
				case 0x41:
				case 0x42:
				case 0x43:					
					return 1; //plateform	 

				case 0x4A:
				case 0x4C:
				case 0x4D:
				case 0x4E:
				case 0x4F:
				case 0x50: 	
				case 0x51: 	
				case 0x52: 	
				case 0x55: 	
				case 0x56: 		
				case 0x57: 	
				case 0x58: 
				case 0x5B: 					
					return 2; //water		 					
			}	

			if((tileIndex & 64) != 64)
				return 0;
			
			return -1;
		}

		//replace tiles when a (!) block is hit
		public static int SwitchTile(int tileData, int switchMode)
		{
			if(switchMode == 1)
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
			else if(switchMode == 2)
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
		
		public static int GetTypeOfSwitch()
		{
			for (int tileIndex = 0 ; tileIndex < 8192 ; tileIndex++)
			{
				byte data = Level.LevelData[tileIndex + 0x1000];
				if(data == 0x39)
				{
					return 1;
				}
				
				if(data == 0x32)
				{
					return 2;
				}
			}
			
			return 0;
		}

		public static bool SaveChanges(Rom rom, int course, out string errorMessage)
		{
			//rom expansion give new banks for level data
			rom.ExpandTo1MB();

			SaveBlocksToRom(rom, course);
			if(!SaveObjectsToRom(rom, course, out errorMessage))
			{
				return false;
			}

			rom.FixCRC();
			return true;
		}

		static void SaveBlocksToRom(Rom rom, int course)
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
				allTiles[i] = new byte[0x3000];
				RLEDecompressTiles(rom, tilePosition, allTiles[i]);
			}

			Array.Copy(LevelData, allTiles[course], 0x3000);

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

		static bool SaveObjectsToRom(Rom rom, int course, out string errorMessage)
		{
			byte[][] enemyData = new byte[0x2B][];

			//save objects
			rom.SetBank(0x7);
			for(int i = 0 ; i < 0x2B ; i++)
			{
				int objectsPos = rom.ReadWord(0x4199 + i * 2);
				enemyData[i] = new byte[0x2000];
				RLEDecompressObjects(rom, objectsPos, enemyData[i]);
			}
			enemyData[course]= ObjectsData;

			const int maxSize = 4198;
			int objectSize = enemyData.Sum(x => RLECompressObjects(x).Count());
			if(objectSize > maxSize)
			{
				errorMessage = string.Format("Object data is too big to fit in ROM.\r\nPlease remove some objects to free at least {0} byte(s).", objectSize - maxSize);
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
