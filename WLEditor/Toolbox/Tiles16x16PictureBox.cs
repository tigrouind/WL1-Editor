using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class Tiles16x16PictureBox : PictureBox
	{
		public MainForm MainForm;
				
		protected override void OnPaint(PaintEventArgs e)
		{			
			if(Level.levelData != null)
			{
				int zoom = MainForm.zoom;
				
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(Level.tiles16x16.Bitmap, 0, 0, 128 * zoom, 256 * zoom);
	
				if(MainForm.currentTile != -1)
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
					{
						e.Graphics.FillRectangle(brush, (MainForm.currentTile % 8) * 16 * zoom, (MainForm.currentTile / 8) * 16 * zoom, 16 * zoom, 16 * zoom);
					}
				}
			}
		}		
					
		protected override void OnMouseDown(MouseEventArgs e)
		{
			int zoom = MainForm.zoom;
			
			MainForm.currentTile = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 8;
			Refresh();
		}	

		public void SetZoom(int zoom)
		{
			Height = 256 * zoom;
			Width = 128 * zoom;							
			Refresh();	
		}
	}
}
