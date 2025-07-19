namespace WLEditor
{
	public class ClipboardData
	{
		//tiles
		public int TileSize;
		public int Width;
		public int Height;
		public ClipboardItems[] Items;

		//sectors
		public Warp LevelHeader;
		public Warp Checkpoint;
		public (int WarpType, Warp Warp)[] Warps;
		public byte[] Scroll;
		public int Music;
	}
}
