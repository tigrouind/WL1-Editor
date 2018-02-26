using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class ToolboxForm : Form
	{		
		public MainForm MainForm;
				
		public ToolboxForm()
		{
			InitializeComponent();
		}
		
		public void SetZoom(int zoom)
		{
			tiles16x16PictureBox.Height = 256 * zoom;
			tiles16x16PictureBox.Width = 128 * zoom;							
			tiles16x16PictureBox.Refresh();	

			objectPictureBox.Height = 128 * zoom;
			objectPictureBox.Width = 128 * zoom;
			objectPictureBox.Refresh();	

			tiles8x8PictureBox.Height = 64 * zoom;
			tiles8x8PictureBox.Width = 128 * zoom;							
			tiles8x8PictureBox.Refresh();				
		}
		
		public void RefreshPictureBoxes()
		{
			tiles16x16PictureBox.Refresh();	
			tiles8x8PictureBox.Refresh();
			objectPictureBox.Refresh();			
		}
		
		void TilesPictureBoxPaint(object sender, PaintEventArgs e)
		{			
			if(Level.levelData != null)
			{
				int zoom = MainForm.zoom;
				
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(MainForm.tiles16x16.Bitmap, 0, 0, 128 * zoom, 256 * zoom);
	
				if(MainForm.currentTile != -1)
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
					{
						e.Graphics.FillRectangle(brush, (MainForm.currentTile % 8) * 16 * zoom, (MainForm.currentTile / 8) * 16 * zoom, 16 * zoom, 16 * zoom);
					}
				}
			}
		}
		
		void TilesPictureBoxMouseDown(object sender, MouseEventArgs e)
		{
			int zoom = MainForm.zoom;
			
			MainForm.currentTile = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 8;
			tiles16x16PictureBox.Refresh();
		}
					
		void ObjectPictureBoxMouseDown(object sender, MouseEventArgs e)
		{
			int zoom = MainForm.zoom;
			
			int index = e.Location.X / 32 / MainForm.zoom + (e.Location.Y / 32 / zoom) * 4;
			index = GUIToObjectIndex(index);
			
			if(!(index >= 1 && index <= 6) || Level.enemiesAvailable[index - 1])
			{
				MainForm.currentObject = index;
				objectPictureBox.Refresh();
			}					
		}
		
		int ObjectIndexToGUI(int index)
		{
			if(index >= 1 && index <= 6) return index + 9;
			if(index > 6) return index - 6;
			return 0;
		}
		
		int GUIToObjectIndex(int index)
		{
			if(index >= 1 && index <= 9) return index + 6;
			if(index > 9) return index - 9;
			return 0;
		}
		
		void ObjectFormPaint(object sender, PaintEventArgs e)
		{			
			if(Level.levelData != null)
			{
				int zoom = MainForm.zoom;
				
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

				using (Brush enemyBrush = new SolidBrush(Level.enemyPalette[2]))
				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Pen pen = new Pen(Color.White, 1.5f * zoom))
				using (Font font = new Font("Arial", 16 * zoom))
				{
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					
					e.Graphics.FillRectangle(enemyBrush, 0, 0, objectPictureBox.Width, objectPictureBox.Height);
					
					for(int j = 0 ; j < 16 ; j++)
					{
						int index = GUIToObjectIndex(j);
						int x = (j % 4) * 32;
						int y = (j / 4) * 32;
						
						Rectangle destRect = new Rectangle(x * zoom, y * zoom, 32 * zoom, 32 * zoom);							
						if (destRect.IntersectsWith(e.ClipRectangle))
					    {
						   	if(index == 0)
							{							
								e.Graphics.DrawLine(pen, (x + 8) * zoom, (y + 8) * zoom, (x + 24) * zoom, (y + 24) * zoom);
								e.Graphics.DrawLine(pen, (x + 24) * zoom, (y + 8) * zoom, (x + 8) * zoom, (y + 24) * zoom);
							}
							else if(index <= 6)
							{																	
								if(Level.enemiesAvailable[index - 1])
								{
									Rectangle enemyRect = Level.loadedSprites[index - 1];
									if(enemyRect != Rectangle.Empty)
									{									
										e.Graphics.DrawImage(MainForm.tilesEnemies.Bitmap, destRect, new Rectangle(enemyRect.X - 16 + enemyRect.Width / 2, enemyRect.Y - 16 + enemyRect.Height / 2, 32, 32), GraphicsUnit.Pixel);
									}	
									else
									{
										e.Graphics.DrawString(index.ToString(), font,  Brushes.White, (x + 16) * zoom, (y + 16) * zoom, format);
									}
								}
							}
							else
							{
								e.Graphics.DrawImage(MainForm.tilesObjects.Bitmap, new Rectangle((x + 8) * zoom, (y + 8) * zoom, 16 * zoom, 16 * zoom), new Rectangle((index - 7) * 16, 0, 16, 16),  GraphicsUnit.Pixel);
							}
					    }						
					}
					
					if(MainForm.currentObject != -1)
					{
						int index = ObjectIndexToGUI(MainForm.currentObject);
						e.Graphics.FillRectangle(brush, (index % 4) * 32 * zoom, (index / 4) * 32 * zoom, 32 * zoom, 32 * zoom);
					}
				}
			}		
		}
				
		void Tiles16x16FormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing) 
		    {
		        e.Cancel = true;
		        Hide();
		    }
		}
		
		void Tiles16x16FormLoad(object sender, EventArgs e)
		{
			comboBox1.SelectedIndex = 0;
		}
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			tiles16x16PictureBox.Visible = comboBox1.SelectedIndex == 0;
			tiles8x8PictureBox.Visible = comboBox1.SelectedIndex == 1;
			objectPictureBox.Visible = comboBox1.SelectedIndex == 2;
		}
		
		void Tiles8x8PictureBoxPaint(object sender, PaintEventArgs e)
		{
			if(Level.levelData != null)
			{
				int zoom = MainForm.zoom;
				
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(MainForm.tiles8x8.Bitmap, 0, 0, 128 * zoom, 128 * zoom);	
			}
		}
		
		public int GetSelectedPanelIndex()
		{
			return comboBox1.SelectedIndex;
		}
	}
}
