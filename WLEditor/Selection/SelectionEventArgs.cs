using System;
using System.Drawing;

namespace WLEditor
{
	public class SelectionEventArgs(Rectangle rectangle) : EventArgs
	{
		public readonly Rectangle ClipRectangle = rectangle;
	}
}