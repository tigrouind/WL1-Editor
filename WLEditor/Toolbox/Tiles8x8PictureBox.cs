using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class Tiles8x8PictureBox : PictureBox
	{
		public MainForm MainForm;
		
		protected override void OnPaint(PaintEventArgs e)
		{
			if(Level.levelData != null)
			{
				int zoom = MainForm.zoom;
				
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(MainForm.tiles8x8.Bitmap, 0, 0, 128 * zoom, 128 * zoom);	
			}
		}
		
		public void SetZoom(int zoom)
		{
			Height = 64 * zoom;
			Width = 128 * zoom;							
			Refresh();				
		}		
	}
}
