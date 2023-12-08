using System;

namespace WLEditor
{
	[Flags]
	public enum ChangeEnum
	{
		None = 0,
		Blocks = 1,
		Sectors = 2,
		WorldTile = 4,
		WorldEvent = 8,
		WorldPath = 16
	}
}
