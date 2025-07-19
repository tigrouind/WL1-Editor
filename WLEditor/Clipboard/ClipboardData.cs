namespace WLEditor
{
	public class ClipboardData
	{
		//tiles
		public int TileWidth;
		public int TileHeight;
		public ClipboardTile[] Tiles;

		//sectors
		public Warp LevelHeader;
		public Warp Checkpoint;
		public (int Type, Warp Warp)[] Warps;
		public byte[] Scroll;
		public int Music;
	}
}
