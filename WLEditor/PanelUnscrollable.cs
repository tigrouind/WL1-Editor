using System;
using System.Windows.Forms;

namespace WLEditor
{
	public class PanelUnScrollable : Panel
    {       
		const int WM_MOUSEWHEEL = 0x020A;
		const int MK_CONTROL = 0x0008;
				
        protected override void WndProc(ref Message m)
        {           
        	if (m.Msg == WM_MOUSEWHEEL && ((int)m.WParam & 0xFFFF) == MK_CONTROL)
            {
            	return;
            }
            
            base.WndProc(ref m);
        }
    }
}
