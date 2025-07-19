using System.Text.Json;

namespace WLEditor
{
	public class Clipboard
	{
		static readonly JsonSerializerOptions options = new() { IncludeFields = true };

		public static ClipboardData Data = new();

		public static void Copy()
		{
			System.Windows.Forms.Clipboard.SetData("WLEditor", JsonSerializer.Serialize(Data, options));
			Data = new();
		}

		public static bool Paste()
		{
			var json = (string)System.Windows.Forms.Clipboard.GetData("WLEditor");
			if (!string.IsNullOrEmpty(json))
			{
				Data = JsonSerializer.Deserialize<ClipboardData>(json, options);
				return true;
			}
			else
			{
				Data = new();
				return false;
			}
		}
	}
}
