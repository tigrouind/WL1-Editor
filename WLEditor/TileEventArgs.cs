using System;
using System.Windows.Forms;

namespace WLEditor
{
	public class TileEventArgs : EventArgs
	{
		public readonly int TileX;
		public readonly int TileY;
		public readonly MouseButtons Button;
		public readonly TileEventStatus Status;

		public TileEventArgs(MouseButtons button, TileEventStatus status, int x, int y)
		{
			Button = button;
			Status = status;
			TileX = x;
			TileY = y;
		}
	}
}
