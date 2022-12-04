using System;
using System.Windows.Forms;

namespace WLEditor
{
	public class PanelUnScrollable : Panel
    {       
		const int WM_MOUSEWHEEL = 0x020A;
		const int MK_CONTROL = 0x0008;
		public new event MouseEventHandler MouseWheel;
				
        protected override void WndProc(ref Message m)
        {           
        	if (m.Msg == WM_MOUSEWHEEL && LOWORD((int)m.WParam) == MK_CONTROL)
            {
        		int delta = SignedHIWORD((int)m.WParam);
        		MouseWheel(this, new MouseEventArgs(MouseButtons.None, 0, 0, 0, delta));
            	return;
            }
            
            base.WndProc(ref m);
        }
        
        int SignedHIWORD(int n)
        {
        	return unchecked((short)HIWORD(n));
        }
        
        int HIWORD(int n)
        {
        	return (n >> 16) & 0xFFFF;
        }
        
        int LOWORD(int n) 
        {
            return n & 0xffff;
        }
    }
}
