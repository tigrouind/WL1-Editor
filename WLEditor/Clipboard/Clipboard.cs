using System.Text.Json;

namespace WLEditor
{
	public class Clipboard
	{
		static readonly JsonSerializerOptions options = new() { IncludeFields = true };

		public static void Copy(ClipboardData data, ClipboardType type)
		{
			System.Windows.Forms.Clipboard.SetData($"WLEditor{type}", JsonSerializer.Serialize(data, options));
		}

		public static ClipboardData Paste(ClipboardType type)
		{
			var json = (string)System.Windows.Forms.Clipboard.GetData($"WLEditor{type}");
			if (!string.IsNullOrEmpty(json))
			{
				return JsonSerializer.Deserialize<ClipboardData>(json, options);
			}
			else
			{
				return null;
			}
		}
	}
}
