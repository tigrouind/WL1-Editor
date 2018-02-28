using System;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class ToolboxForm : Form
	{		
		public MainForm MainForm;				
		public Tiles8x8PictureBox tiles8x8PictureBox  = new Tiles8x8PictureBox();	
		public Tiles16x16PictureBox tiles16x16PictureBox = new Tiles16x16PictureBox();
		public ObjectsPictureBox objectsPictureBox = new ObjectsPictureBox();
				
		public ToolboxForm()
		{				
			InitializeComponent();
		}	
		
		public void InitializeControls()
		{
			tiles8x8PictureBox.MainForm = MainForm;
			tiles8x8PictureBox.SetZoom(1);
						
			tiles16x16PictureBox.MainForm = MainForm;
			tiles16x16PictureBox.SetZoom(1);
									
			objectsPictureBox.MainForm = MainForm;			
			objectsPictureBox.SetZoom(1);				
		}
		
		public void SetZoom(int zoom)
		{
			tiles16x16PictureBox.SetZoom(zoom);
			tiles8x8PictureBox.SetZoom(zoom);
			objectsPictureBox.SetZoom(zoom);			
		}
		
		public void RefreshPictureBoxes()
		{
			tiles16x16PictureBox.Refresh();	
			tiles8x8PictureBox.Refresh();
			objectsPictureBox.Refresh();			
		}				
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			SuspendLayout();
			Control controlToRemove = tableLayoutPanel1.GetControlFromPosition(0, 1);
			tableLayoutPanel1.Controls.Remove(controlToRemove);
			
			Control controlToAdd;
			switch(comboBox1.SelectedIndex)
			{
				case 0:
					controlToAdd = tiles16x16PictureBox;
					break;
				case 1:
					controlToAdd = tiles8x8PictureBox;
					break;
				case 2:
					controlToAdd = objectsPictureBox;
					break;
				default:
					throw new NotImplementedException();
			}
			
			tableLayoutPanel1.Controls.Add(controlToAdd, 0, 1);			
			ResumeLayout();
		}		
		
		void ToolBoxFormLoad(object sender, EventArgs e)
		{			
			comboBox1.SelectedIndex = 0;
		}
						
		void ToolBoxFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing) 
		    {
		        e.Cancel = true;
		        Hide();
		    }
		}
		
		public int GetSelectedPanelIndex()
		{
			return comboBox1.SelectedIndex;
		}
	}
}
