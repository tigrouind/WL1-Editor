using System;
using System.IO;
using System.Linq;

namespace WLEditor
{
	public static class Gfx
	{
		static readonly (int Bank, int Address, int CompressedSize, string Name)[] tileData = new(int, int, int, string)[]
		{
			(0x12, 0x54C2, 4761, "Intro"),
			(0x06, 0x5706, 4141, "SaveSelection"),
			(0x06, 0x7469, 1085, "MiniGameSelection"),
			(0x06, 0x41E3, 5078, "Statistics"),
			(0x09, 0x407A, 3477, "World_1"),
			(0x14, 0x6909, 3524, "World_2"),
			(0x09, 0x4E13, 3667, "World_3"),
			(0x09, 0x5C6C, 3639, "World_4"),
			(0x14, 0x5AA0, 3682, "Overworld"),
			(0x05, 0x4FF1, 2289, "GameOver"),
			(0x1F, 0x53C8, 5430, "EndGame")
		};

		static readonly (int Bank, int Address, int UncompressedSize, int CompressedSize, string Name)[] mapData = new (int, int, int, int, string)[]
		{
			(0x12, 0x6763,  308, 199, "Intro"),
			(0x06, 0x6737,  564, 306, "SaveSelection"),
			(0x06, 0x55BC,  564, 330, "Statistics"),
			(0x06, 0x78AA,  564, 284, "MiniGameSelection"),
			(0x05, 0x5987,  564, 162, "GameOver"),
			(0x09, 0x6DBE,  564, 373, "World_1_Rice"),
			(0x09, 0x74E1,  564, 346, "World_1_RiceFlooded"),
			(0x09, 0x6F33,  564, 368, "World_1_Teapot"),
			(0x14, 0x76D2,  564, 321, "World_2_Sherbet"),
			(0x14, 0x7813,  564, 388, "World_2_Parsley"),
			(0x09, 0x70A3,  564, 371, "World_3_Stove"),
			(0x09, 0x7216,  564, 393, "World_3_Teacup"),
			(0x09, 0x739F,  564, 322, "World_4_Syrup"),
			(0x09, 0x763B,  564, 247, "World_4_SyrupDefeat"),
			(0x09, 0x6AA5, 1024, 787, "Overworld"),
			(0x1F, 0x754D,  436,  88, "EndGame_1"),
			(0x1F, 0x75A6,  436, 124, "EndGame_2"),
			(0x1F, 0x74B4,  436, 148, "EndGame_3"),
			(0x1F, 0x7627,  436, 120, "EndGame_4"),
			(0x1F, 0x73D3,  436, 225, "EndGame_5"),
			(0x1F, 0x727B,  564, 296, "EndGame_6")
		};

		public static void Export(Rom rom, string path)
		{
			foreach (var (Bank, Address, _, Name) in tileData)
			{
				var data = Overworld.Dump8x8Tiles(rom, Bank, Address).Skip(128 * 16).ToArray();
				var filePath = Path.Combine(path, $"{Name}.chr");
				if (!File.Exists(filePath))
				{
					File.WriteAllBytes(filePath, data);
				}
			}

			foreach (var (Bank, Address, UncompressedSize, _, Name) in mapData)
			{
				var data = new byte[UncompressedSize];
				Overworld.LoadTiles(rom, Bank, Address, data);

				var filePath = Path.Combine(path, $"{Name}.prg");
				if (!File.Exists(filePath))
				{
					File.WriteAllBytes(filePath, data);
				}
			}
		}

		public static bool ImportCHR(Rom rom, string filePath, out string message)
		{
			var data = File.ReadAllBytes(filePath);
			var item = tileData.FirstOrDefault(x => string.Equals(Path.GetFileNameWithoutExtension(filePath), x.Name, StringComparison.InvariantCultureIgnoreCase));

			if (item == default)
			{
				message = "Unknown file name";
				return false;
			}

			var romData = Overworld.Dump8x8Tiles(rom, item.Bank, item.Address).ToArray();
			Array.Copy(data, 0, romData, 128 * 16, Math.Min(data.Length, 4096));

			return Overworld.Save8x8Tiles(rom, item.Bank, item.Address, romData, item.CompressedSize, out message);
		}

		public static bool ImportPRG(Rom rom, string filePath, out string message)
		{
			var data = File.ReadAllBytes(filePath);
			var item = mapData.FirstOrDefault(x => string.Equals(Path.GetFileNameWithoutExtension(filePath), x.Name, StringComparison.InvariantCultureIgnoreCase));

			if (item == default)
			{
				message = "Unknown file name";
				return false;
			}

			var romData = new byte[item.UncompressedSize];
			Overworld.LoadTiles(rom, item.Bank, item.Address, romData);
			Array.Copy(data, 0, romData, 0, Math.Min(data.Length, item.UncompressedSize));

			return Overworld.SaveTiles(rom, item.Bank, item.Address, romData, item.CompressedSize, out message);
		}
	}
}
