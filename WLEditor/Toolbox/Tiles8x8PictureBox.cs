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
			if(Level.levelData != null)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(Level.tiles8x8.Bitmap, 0, 0, 128 * zoom, 128 * zoom);	
			}
		}
		
		public void SetZoom(int zoomlevel)
		{
			Height = 64 * zoomlevel;
			Width = 128 * zoomlevel;	
			zoom = zoomlevel;
			Invalidate();				
		}		
	}
}
