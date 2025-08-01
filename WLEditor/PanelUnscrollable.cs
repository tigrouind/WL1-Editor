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
			if (m.Msg == WM_MOUSEWHEEL && LOWORD((uint)m.WParam) == MK_CONTROL)
			{
				int delta = SignedHIWORD((uint)m.WParam);
				MouseWheel(this, new MouseEventArgs(MouseButtons.None, 0, 0, 0, delta));
				return;
			}

			base.WndProc(ref m);
		}

		int SignedHIWORD(uint n)
		{
			return unchecked((short)HIWORD(n));
		}

		static uint HIWORD(uint n)
		{
			return (n >> 16) & 0xFFFF;
		}

		static uint LOWORD(uint n)
		{
			return n & 0xffff;
		}
	}
}
