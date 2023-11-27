using System;

namespace WLEditor
{
	[Flags]
	public enum ChangeEnum
	{
		None = 0,
		Blocks = 1,
		Sectors = 2,
		WorldMap = 4,
		WorldTile = 8,
		WorldEvent = 16,
		WorldPath = 32
	}
}
