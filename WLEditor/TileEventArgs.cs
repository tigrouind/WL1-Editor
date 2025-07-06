using System;
using System.Windows.Forms;

namespace WLEditor
{
	public class TileEventArgs(MouseButtons button, TileEventStatus status, int x, int y) : EventArgs
	{
		public readonly int TileX = x;
		public readonly int TileY = y;
		public readonly MouseButtons Button = button;
		public readonly TileEventStatus Status = status;
	}
}
