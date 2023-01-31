using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class Tiles8x8PictureBox : PictureBox
	{
		int zoom;

		protected override void OnPaint(PaintEventArgs e)
		{
			if(Level.LevelData != null && !DesignMode)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(Level.Tiles8x8.Bitmap, 0, 0, 128 * zoom, 64 * zoom);
			}
		}

		public void SetZoom(int zoomlevel)
		{
			Height = 64 * zoomlevel;
			Width = 128 * zoomlevel;
			zoom = zoomlevel;
		}
	}
}
