using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WLEditor
{
	public static class Sprite
	{
		readonly static uint[] enemyPalette = { 0xFFFFFFFF, 0xFF9DC9FD, 0xFFFF00FF, 0xFF000029 };

		public static Rectangle[] PlayerRectangles = new Rectangle[2];
		public static Rectangle[] LoadedSprites = new Rectangle[6];

		public static DirectBitmap TilesObjects = new DirectBitmap(16 * 9, 16);
		public static DirectBitmap TilesEnemies = new DirectBitmap(64, 128 * 6);
		public static DirectBitmap PlayerSprite = new DirectBitmap(64, 64 * 2);
		public static DirectBitmap Tiles8x8 = new DirectBitmap(16 * 8, 4 * 8);

		readonly static ushort[] enemySpriteAddress =
		{
			0x4FAA, 0x6FBC, 0x74E9, 0x6B4E, 0x6C70, 0x4DB0, 0x0000, 0x5BAF, 0x66A0, 0x66A0, 0x686C, 0x60D1, 0x6373, 0x4035, 0x40F5,
			0x4308, 0x459E, 0x4AA0, 0x53A3, 0x5C86, 0x5E0F, 0x64D4, 0x6D16, 0x6726, 0x6C2F, 0x7C56, 0x7C56, 0x435D, 0x4D26, 0x5ECA, 0x460F,
			0x54F8, 0x5536, 0x0000, 0x6B57, 0x6DFC, 0x6DE4, 0x6E87, 0x736F, 0x75BA, 0x6082, 0x64BA, 0x67BF, 0x6933, 0x6933, 0x6BEE, 0x6F27,
			0x72BF, 0x7570, 0x7B10, 0x4F33, 0x42D0, 0x45AC, 0x45BC, 0x6B39, 0x7AB7, 0x7ABB, 0x7AC3, 0x7ABF, 0x6F1F, 0x6D59, 0x6F84, 0x7338,
			0x6D56, 0x6D8E, 0x730D, 0x7567, 0x7B16, 0x778B, 0x776D, 0x6D56, 0x796C, 0x7992, 0x7572, 0x7C24, 0x7DE6, 0x7F1F, 0x7153
		};

		readonly static ushort[] bonusSpriteAddress = { 0x4ACB, 0x4AEB, 0x4ADB, 0x4AFB, 0x4B0B, 0x4B1B, 0x4B2B, 0x4B4B, 0x4B5B };

		readonly static byte[] enemyCodePattern =
		{
			0x3E, 0x00,       //ld  a, ..
			0xEA, 0x2F, 0xA3, //ld  (A32F),a
			0x3E, 0x00,       //ld  a, ..
			0xEA, 0x30, 0xA3, //ld  (A330),a
			0x01, 0x00, 0x00, //ld  bc, ..
			0xCD, 0xA7, 0x40  //call 40A7
		};

		readonly static byte[] treasurePattern =
		{
			0x3E, 0x00,      // ld a, ..
			0xEA, 0x82, 0xA3 // ld (A382), a
		};

		readonly static byte[] treasureCheckPattern =
		{
			0xCD, 0x6A, 0x42 //call 426A
		};

		readonly static byte[] exitPattern =
		{
			0x3E, 0x02,      // ld a, 02
			0xEA, 0x76, 0xA3 // ld (A376), a
		};

		public static void DumpBonusSprites(Rom rom)
		{
			Array.Clear(Tiles8x8.Bits, 0, Tiles8x8.Bits.Length);
			//bonus sprites
			rom.SetBank(0x5);
			Level.Dump8x8Tiles(rom, Tiles8x8, 0x4C81, 23, 0, 0x1E, enemyPalette, true);

			rom.SetBank(0x11);
			Level.Dump8x8Tiles(rom, Tiles8x8, 0x7300, 4, 16 + 23, 0x1E, enemyPalette, true);

			rom.SetBank(0xF);
			for (int i = 0; i < bonusSpriteAddress.Length; i++)
			{
				DumpSprite(rom, 8 + i * 16, 16, bonusSpriteAddress[i], TilesObjects);
			}
		}

		#region Player

		public static void DumpPlayerSprite(Rom rom)
		{
			Array.Clear(Tiles8x8.Bits, 0, Tiles8x8.Bits.Length);

			rom.SetBank(0x5);
			Level.Dump8x8Tiles(rom, Tiles8x8, 0x42F1, 64, 0, 0x1E, enemyPalette, true);
			PlayerRectangles[0] = DumpSpritePlayer(rom, 32, 56, 0x603B, false);
			PlayerRectangles[1] = DumpSpritePlayer(rom, 32, 56 + 64, 0x603B, true);
		}

		static Rectangle DumpSpritePlayer(Rom rom, int posx, int posy, int spriteAddress, bool horizontalFlip)
		{
			Rectangle rectangle = Rectangle.Empty;
			int pos = spriteAddress;

			sbyte hflip(sbyte x) => (sbyte)(horizontalFlip ? ((~x) - 7) : x);

			pos = rom.ReadWord(pos);
			while (rom.ReadByte(pos) != 0x80)
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

		#endregion

		static Rectangle DumpSprite(Rom rom, int posx, int posy, int spriteAddress, DirectBitmap tilesDest)
		{
			Rectangle rectangle = Rectangle.Empty;
			int pos = spriteAddress;

			pos = rom.ReadWord(pos);
			pos++; //skip sprite flags

			while (rom.ReadByte(pos) != 0x80)
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

			if ((spriteFlags & 0x40) != 0) //horizontal flip
			{
				getX = x => 7 - x;
			}
			else
			{
				getX = x => x;
			}

			if ((spriteFlags & 0x80) != 0) //vertical flip
			{
				getY = y => 7 - y;
			}
			else
			{
				getY = y => y;
			}

			for (int y = 0; y < 8; y++)
			{
				int src = source.X + (source.Y + getY(y)) * Tiles8x8.Width;
				int dst = spx + (spy + y) * tilesDest.Width;

				for (int x = 0; x < 8; x++)
				{
					if (tilesDest.Bits[dst + x] == 0)
					{
						tilesDest.Bits[dst + x] = Tiles8x8.Bits[src + getX(x)];
					}
				}
			}
		}

		#region Enemies

		public static void FindEnemiesData(Rom rom, int enemiesPointer, out int enemyId, out int tilesPointer,
										out int treasureID, out bool treasureCheck, out bool exitOpen)
		{
			rom.SetBank(0x7);
			int position = enemiesPointer;
			while (rom.ReadByte(position) != 0xC9) //ret
			{
				if (MatchPattern(rom, position, enemyCodePattern))
				{
					enemyId = rom.ReadByte(position + 6) << 8 | rom.ReadByte(position + 1);
					tilesPointer = rom.ReadWord(position + 11);

					position += enemyCodePattern.Length;
					CheckExtraCode(rom, position, out treasureID, out treasureCheck, out exitOpen);
					return;
				}

				position++;
			}

			throw new Exception("cannot find enemies data");
		}

		static void CheckExtraCode(Rom rom, int position, out int treasureID, out bool treasureCheck, out bool exitOpen)
		{
			treasureCheck = false;
			exitOpen = false;
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
				else
				{
					position++;
				}
			}
		}

		static bool MatchPattern(Rom rom, int position, byte[] pattern)
		{
			for (int j = 0; j < pattern.Length; j++)
			{
				byte valueFromRom = rom.ReadByte(position + j);
				byte valueToCompare = pattern[j];

				if (valueToCompare != 0x00 && valueFromRom != valueToCompare)
				{
					return false;
				}
			}

			return true;
		}

		public static void DumpEnemiesSprites(Rom rom, int enemiesIdsPointer, int tilesDataAddress)
		{
			Array.Clear(TilesEnemies.Bits, 0, TilesEnemies.Bits.Length);
			Array.Clear(LoadedSprites, 0, LoadedSprites.Length);

			DumpEnemiesSprites(rom, enemiesIdsPointer, tilesDataAddress, TilesEnemies, 0, LoadedSprites, 0, out _);
		}

		public static void DumpEnemiesSprites(Rom rom, int enemiesIdsPointer, int tilesDataAddress,
			DirectBitmap bitmap, int destX, Rectangle[] spriteRects, int index, out int[] enemyIds)
		{
			enemyIds = GetEnemyIds(rom, enemiesIdsPointer).ToArray();
			var enemyTileInfo = LoadEnemiesTiles(rom, tilesDataAddress).ToArray();

			for (int i = 0; i < enemyTileInfo.Length; i++)
			{
				int enemyId = enemyIds[i];
				if (enemyId > 0 && enemyId <= enemySpriteAddress.Length)
				{
					int spriteDataAddress = enemySpriteAddress[enemyId - 1];
					if (spriteDataAddress != 0)
					{
						var (tileBank, tileAddress, tileCount) = enemyTileInfo[i];
						LoadEnemiesTilesInternal(rom, spriteDataAddress, tileBank, tileCount, tileAddress);

						Rectangle rectangle = DumpSprite(rom, destX + 32, 120 + i * 128, spriteDataAddress, bitmap);
						spriteRects[index + i] = rectangle;
					}
				}
			}
		}

		static IEnumerable<int> GetEnemyIds(Rom rom, int enemiesIdsPointer)
		{
			rom.SetBank(0x7);
			for (int i = 0; i < 6; i++)
			{
				int enemyId = rom.ReadWord(enemiesIdsPointer + (i + 1) * 2);
				enemyId = (enemyId - 0x530F) / 4;
				yield return enemyId;
			}
		}

		static IEnumerable<(int Bank, int Address, int Count)> LoadEnemiesTiles(Rom rom, int tilesDataAddress)
		{
			rom.SetBank(0x7);
			do
			{
				int tilesBank = rom.ReadByte(tilesDataAddress++);
				int tilesAddress = rom.ReadWord(tilesDataAddress++); tilesDataAddress++;
				int tilesCount = rom.ReadByte(tilesDataAddress++);

				if (tilesCount == 0xFF)
				{
					yield break;
				}

				yield return (tilesBank, tilesAddress, tilesCount);
			}
			while (true);
		}

		static void LoadEnemiesTilesInternal(Rom rom, int spriteDataAddress, int tilesBank, int tilesCount, int tilesAddress)
		{
			rom.SetBank(tilesBank);
			int pos = rom.ReadWord(spriteDataAddress);
			int spriteFlags = rom.ReadByte(pos);
			int palette = (spriteFlags & 0x10) != 0 ? 0xD1 : 0x1E;

			Array.Clear(Tiles8x8.Bits, 0, Tiles8x8.Bits.Length);
			Level.Dump8x8Tiles(rom, Tiles8x8, tilesAddress, tilesCount, 0, (byte)palette, enemyPalette, true);
		}

		#endregion
	}
}
