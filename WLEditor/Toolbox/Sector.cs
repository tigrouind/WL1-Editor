﻿using System;
using System.Collections.Generic;

namespace WLEditor
{
	public static class Sector
	{		
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
					int block = Level.LevelData[(x + startX) + (y + startY) * 256 + 0x1000];
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
	}
}