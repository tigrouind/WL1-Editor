using System;
using System.Drawing;

namespace WLEditor
{
	public class SelectionEventArgs : EventArgs
	{
		public readonly Rectangle ClipRectangle;
		public SelectionEventArgs(Rectangle rectangle)
		{
			ClipRectangle = rectangle;
		}
	}
}