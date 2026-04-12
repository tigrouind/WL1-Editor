using System;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	internal sealed class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var mainForm = new MainForm(args.FirstOrDefault());
			Application.Idle += mainForm.ApplicationIdle;
			Application.Run(mainForm);
		}
	}
}
