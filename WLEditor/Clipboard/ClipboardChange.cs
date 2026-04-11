using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WLEditor
{
	public class ClipboardChange : NativeWindow, IDisposable
	{
		#region Native

		const int WM_CREATE = 0x0001;
		const int WM_DESTROY = 0x0002;
		const int WM_DRAWCLIPBOARD = 0x0308;
		const int WM_CHANGECBCHAIN = 0x030D;

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

		#endregion

		IntPtr nextClipboardViewer;

		public EventHandler Change;

		public ClipboardChange()
		{
			CreateHandle(new CreateParams());
		}

		protected override void WndProc(ref Message message)
		{
			switch (message.Msg)
			{
				case WM_CREATE:
					nextClipboardViewer = SetClipboardViewer(message.HWnd);
					break;

				case WM_DESTROY:
					ChangeClipboardChain(message.HWnd, nextClipboardViewer);
					break;

				case WM_DRAWCLIPBOARD:
					Change?.Invoke(this, EventArgs.Empty);
					SendMessage(nextClipboardViewer, message.Msg, message.WParam, message.LParam);
					break;

				case WM_CHANGECBCHAIN:
					if (message.WParam == nextClipboardViewer)
					{
						nextClipboardViewer = message.LParam;
					}
					else if (nextClipboardViewer != IntPtr.Zero)
					{
						SendMessage(nextClipboardViewer, message.Msg, message.WParam, message.LParam);
					}
					break;
			}

			base.WndProc(ref message);
		}

		public void Dispose()
		{
			DestroyHandle();
		}
	}
}
