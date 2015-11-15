using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{		
		public Rom rom = new Rom();
		public Bitmap tiles8x8 = new Bitmap(16 * 8, 8 * 8);
		public Bitmap tiles16x16 = new Bitmap(16 * 8, 16 * 16);		
		public int currentSector = -1;
		public int currentTile = -1;		
		public int currentCourseId = -1;
		public string romFilePath;		
		public bool hasChanges;
				
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//								
		}
		
		void LoadLevel(bool reloadAll)
		{			
			if(rom.IsLoaded && comboBox1.SelectedItem != null)
			{				
				Level.DumpLevel(rom, currentCourseId, currentSector, tiles8x8, tiles16x16, reloadAll, aToolStripMenuItem.Checked, bToolStripMenuItem.Checked);
				
				pictureBox1.Refresh();	
				pictureBox3.Refresh();
			}			
		}
		
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)				
		{								
			if(comboBox1.SelectedItem != null)
			{
				if(AskForSavingChanges())
				{
					currentSector = -1;
					ComboboxItem item = (ComboboxItem)comboBox1.SelectedItem;				
					currentCourseId = (int)item.Value;
					LoadLevel(true);
				}
				else
				{
					//restore previous item
					comboBox1.SelectedIndexChanged -= ComboBox1SelectedIndexChanged;
					comboBox1.SelectedItem = comboBox1.Items.Cast<ComboboxItem>().First(x => (int)x.Value == currentCourseId);
					comboBox1.SelectedIndexChanged += ComboBox1SelectedIndexChanged;
				}
			}
		}			
			
		void LoadToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				rom.Load(openFileDialog1.FileName);				
				LoadCombobox1();
				romFilePath = openFileDialog1.FileName;
											
				if(comboBox1.SelectedIndex == 0)			
					ComboBox1SelectedIndexChanged(sender, e);
				else 
					comboBox1.SelectedIndex = 0;										
			}
		}
		
		void LoadCombobox1()
		{
			//init combobox				
			comboBox1.SelectedItem = null;
			comboBox1.Items.Clear();
			
			//convert course id => course no using data in ROM
			rom.SetBank(0);													
			List<ComboboxItem> items = new List<ComboboxItem>();
			for(int i = 0 ; i <= 0x2A ; i++)
			{
				int levelpointer = rom.ReadWord(0x0534 + i * 2);
				int courseNo = (levelpointer - 0x0587) / 3;
				ComboboxItem item = new ComboboxItem(string.Format("Course {0:D2}", courseNo), i);												
				items.Add(item);
			}		
			
			items = items.OrderBy(x => x.Text).ToList();
			string oldItem = null;
			foreach(var item in items)
			{
				if(oldItem != null && item.Text == oldItem)
				{
					item.Text += " (revisited)";
				}
				oldItem = item.Text;
			}
			
			comboBox1.Items.AddRange(items.ToArray());			
		}
		
		void RegionsToolStripMenuItemClick(object sender, EventArgs e)
		{
			pictureBox1.Refresh();
		}
		
		void ObjectsToolStripMenuItemClick(object sender, EventArgs e)
		{
			pictureBox1.Refresh();
		}

		void ScrollRegionToolStripMenuItemClick(object sender, EventArgs e)
		{
			pictureBox1.Refresh();
		}
	
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Application.Exit();
		}						
				
		void PictureBox1Paint(object sender, PaintEventArgs e)
		{							
			bool viewObjects = objectsToolStripMenuItem.Checked;
			bool viewSectors = regionsToolStripMenuItem.Checked;
			bool viewScroll = scrollRegionToolStripMenuItem.Checked;	
			bool viewColliders = collidersToolStripMenuItem.Checked;	
			bool switchA = aToolStripMenuItem.Checked;
			bool switchB = bToolStripMenuItem.Checked;
			
			if(Level.levelData != null)
			{										
				StringFormat format = new StringFormat();
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;		
				
				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				using (Font font = new Font("Arial", 8))
				{								
					for(int j = 0 ; j < 32 ; j++)
					{
						for(int i = 0 ; i < 256 ; i++)
						{					
							Rectangle destRect = new Rectangle(i * 16, j * 16, 16, 16);
							
							if(destRect.IntersectsWith(e.ClipRectangle))
							{
								//tile blocks														
								byte tileIndex = Level.levelData[i + j * 256 + 0x1000];
								e.Graphics.DrawImage(tiles16x16,
						    		        destRect,
						    		        new Rectangle((tileIndex % 8) * 16, (tileIndex / 8) * 16, 16, 16),
						    		        GraphicsUnit.Pixel);
								
								if(viewColliders)
								{
									if(Level.IsCollidable(Level.Switch(tileIndex, switchA, switchB)))
								   	{
										e.Graphics.FillRectangle(brush, destRect);
								   }
								}
								
								if(viewSectors)
								{								
									//if tile is a door, display destination
									if(tileIndex == 72 || tileIndex == 75 || tileIndex == 46 || tileIndex == 84)
									{
										e.Graphics.FillRectangle(Brushes.Brown, destRect);
										e.Graphics.DrawString(Level.warps[i/16 + (j/16)*16].ToString("X"), font, Brushes.White, i *16 +8, j * 16 +8, format);
									}
								}
								
								//objects						
								if(viewObjects)
								{
									byte data = Level.objectsData[i + j * 256];
									if(data != 0)
									{
										e.Graphics.FillRectangle(Brushes.Purple, destRect);
										e.Graphics.DrawString(data.ToString("X"), font, Brushes.White, i * 16 + 8, j * 16 + 8, format);
									}
								}
							}
					 	}
					}					
				}
				
				using(Font font = new Font("Arial", 8))
				using(Pen penBlue = new Pen(Color.DarkBlue, 2.0f))								
				{
					for(int j = 0 ; j < 2 ; j++)
					{
						for(int i = 0 ; i < 16 ; i++)
						{
							Rectangle destRect = new Rectangle(i * 256, j * 256, 256, 256);
							
							if(destRect.IntersectsWith(e.ClipRectangle))
							{						
								int drawSector = i + j * 16;							
							
								//scroll
								if(viewScroll)
								{
									byte scroll = Level.scrollData[drawSector];
									if ((scroll & 2) == 2)
										e.Graphics.FillRectangle(Brushes.Yellow, i * 256, j * 256, 6, 256);
									
									if ((scroll & 1) == 1)
										e.Graphics.FillRectangle(Brushes.Yellow, (i+1) * 256 - 6, j*256, 6, 256);
								}
								
								//sectors
								if(viewSectors)
								{
									e.Graphics.DrawRectangle(penBlue, i * 256 , j * 256, 256, 256);																								
									
									e.Graphics.FillRectangle(Brushes.DarkBlue, i * 256 , j * 256, 16.0f, 16.0f);
									e.Graphics.DrawString(drawSector.ToString("X"), font, Brushes.White, i * 256 + 8, j * 256 + 8, format);									
								}
							}
						}
					}											
				}
				
				if(viewSectors)
				{
					//wario position
					Rectangle destRect = new Rectangle(Level.warioPosition % 8192 - 8, Level.warioPosition / 8192 - 8, 16, 16);
					if(destRect.IntersectsWith(e.ClipRectangle))
					{	
						e.Graphics.FillRectangle(Brushes.Green, destRect);
						using(Font font = new Font("Arial", 8))
						{
							e.Graphics.DrawString("W", font, Brushes.White, destRect, format);	
						}
					}
				}	

				//current sector
				if(viewSectors && currentSector != -1)
				{
					Rectangle destRect = new Rectangle((currentSector % 16) * 256, (currentSector / 16) * 256, 256, 256);
					if(destRect.IntersectsWith(e.ClipRectangle))
					{						
						using(Brush blue = new SolidBrush(Color.FromArgb(64, 0, 0, 255)))
						{
							e.Graphics.FillRectangle(blue, destRect);						
						}						
					}
				}																					
			}		
		}
						
		void CollidersToolStripMenuItemClick(object sender, EventArgs e)
		{
			pictureBox1.Refresh();	
		}

		void PictureBox3Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(tiles16x16, 0, 0);
			
			if(currentTile != -1)
			{
				using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
				{
					e.Graphics.FillRectangle(brush, (currentTile % 8) * 16, (currentTile / 8) * 16, 16, 16);	
				}
			}
		}
		
		void NoneToolStripMenuItemClick(object sender, EventArgs e)
		{			
			SetSwitchMode(noneToolStripMenuItem);
			LoadLevel(false);
		}
		
		void AToolStripMenuItemClick(object sender, EventArgs e)
		{			
			SetSwitchMode(aToolStripMenuItem);
			LoadLevel(false);			
		}
		
		void BToolStripMenuItemClick(object sender, EventArgs e)
		{			
			SetSwitchMode(bToolStripMenuItem);
			LoadLevel(false);
		}
		
		void SetSwitchMode(ToolStripItem toolStrip)
		{
			noneToolStripMenuItem.Checked = toolStrip == noneToolStripMenuItem;
			aToolStripMenuItem.Checked = toolStrip == aToolStripMenuItem;
			bToolStripMenuItem.Checked = toolStrip == bToolStripMenuItem;			
		}
		
		void PictureBox1Click(object sender, EventArgs e)
		{
			MouseEventArgs me = (MouseEventArgs)e;
			if(me.Button == MouseButtons.Right)
			{
				Point coordinates = me.Location;
	    		int sector = me.Location.X / 256 + (me.Location.Y / 256) * 16;
	    		if(sector != currentSector)
	    		{
	    			currentSector = sector;
	    			LoadLevel(false);
	    		}
			}
		}
		
		void SaveChanges()
		{
			Level.SaveChanges(rom, currentCourseId, romFilePath);
		}
		
		bool AskForSavingChanges()
		{
			if(hasChanges)
			{			
				DialogResult result = MessageBox.Show("Save pending changes ?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch(result)
				{
					case DialogResult.Yes:
						SaveChanges();
						hasChanges = false;
						return true;
						
					case DialogResult.No:
						hasChanges = false;
						return true;
						
					case DialogResult.Cancel:
						return false;
						
					default: 
						throw new NotImplementedException();
				}
			}
			else
			{
				return true;
			}
		}
		
		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveChanges();
		}
		
		void PictureBox3MouseDown(object sender, MouseEventArgs e)
		{			
			currentTile = e.Location.X / 16 + (e.Location.Y / 16) * 8;						
			pictureBox3.Refresh();			
		}
		
		void PictureBox1MouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left && currentTile != -1)
			{	
				int tileIndex = e.Location.X / 16 + (e.Location.Y / 16) * 256;
									
				Level.levelData[tileIndex + 0x1000] = (byte)currentTile;
				hasChanges = true;

				Region r = new Region(new Rectangle((tileIndex % 256) * 16, (tileIndex / 256) * 16, 16, 16));
				pictureBox1.Invalidate(r);							
			}
		}
		
		void PictureBox1MouseMove(object sender, MouseEventArgs e)
		{
			if (pictureBox1.ClientRectangle.Contains(e.Location))
			{
				PictureBox1MouseDown(sender, e);	
			}
		}
		
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{			
			e.Cancel = !AskForSavingChanges();
		}	
	}
}
