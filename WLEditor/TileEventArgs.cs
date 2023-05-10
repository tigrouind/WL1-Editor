using System;
using System.Windows.Forms;

namespace WLEditor
{
	public class TileEventArgs : EventArgs
	{
		public readonly int TileIndex;
		public readonly MouseButtons Button;
		public readonly TileEventStatus Status;

		public TileEventArgs(MouseButtons button, TileEventStatus status, int index)
		{
			this.Button = button;
			this.Status = status;
			this.TileIndex = index;
		}
	}
}
