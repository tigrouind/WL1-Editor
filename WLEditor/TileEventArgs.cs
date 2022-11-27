using System;
using System.Windows.Forms;

namespace WLEditor
{
	public class TileEventArgs : MouseEventArgs
	{
		public int Status { private set; get; }
		
		public TileEventArgs(MouseEventArgs e, int status)
			: base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
		{
			this.Status = status;
		}
	}
}
