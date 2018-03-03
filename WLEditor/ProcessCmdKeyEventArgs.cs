
using System;
using System.Windows.Forms;

namespace WLEditor
{
	public class ProcessCmdKeyEventArgs : EventArgs
	{
		public Keys KeyData;
		
		public bool Processed;
		
		public ProcessCmdKeyEventArgs(Keys keyData)
		{			
			KeyData = keyData;
		}
	}
}