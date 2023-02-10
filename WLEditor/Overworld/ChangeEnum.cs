using System;

namespace WLEditor
{
	[Flags]
	public enum ChangeEnum
	{
		None = 0,
		Tile = 1,
		Event = 2,
		Path = 4
	}
}
