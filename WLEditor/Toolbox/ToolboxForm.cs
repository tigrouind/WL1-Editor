using System;
using System.Drawing;
using System.Windows.Forms;

namespace WLEditor
{
	public partial class ToolboxForm : Form
	{				
		public event EventHandler<KeyEventArgs> ProcessCommandKey;		
		
		public ToolboxForm()
		{				
			InitializeComponent();			
		}	
		
		public void SetZoom(int zoom)
		{
			tiles16x16PictureBox.SetZoom(zoom);
			tiles8x8PictureBox.SetZoom(zoom);
			objectsPictureBox.SetZoom(zoom);			
		}
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			SuspendLayout();
			
			Control controlToShow = new Control[] { tiles16x16PictureBox, tiles8x8PictureBox, objectsPictureBox }[comboBox1.SelectedIndex];
			controlToShow.Visible = true;
			controlToShow.Location = new Point(controlToShow.Margin.Left, controlToShow.Margin.Top);
			
			foreach(Control control in panel1.Controls)
			{
				if(control != controlToShow)
				{
					control.Visible = false;
				}
			}
						
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
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			KeyEventArgs args = new KeyEventArgs(keyData);
			
			ProcessCommandKey(this, args);
			if(args.Handled)
			{
				return true;
			}
			
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		public int CurrentObject
		{
			get
			{
				return objectsPictureBox.CurrentObject;
			}
			
			set
			{
				objectsPictureBox.CurrentObject = value;
			}
		}
		
		public int CurrentTile
		{
			get
			{
				return tiles16x16PictureBox.CurrentTile;
			}			
		}
		
		public int SelectedPanelIndex
		{
			get
			{
				return comboBox1.SelectedIndex;	
			}			
		}
		
		public bool ShowColliders
		{
			set
			{
				tiles16x16PictureBox.ShowColliders = value;
			}
		}
		
		public int SwitchMode
		{
			set
			{
				tiles16x16PictureBox.SwitchMode = value;
			}
		}
		
		public bool ShowTileNumbers
		{
			set
			{
				tiles16x16PictureBox.ShowTileNumbers = value;
			}
		}
	}
}
