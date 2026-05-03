using System.Text.Json;

namespace WLEditor
{
	public class Clipboard
	{
		static readonly JsonSerializerOptions options = new() { IncludeFields = true };

		public static void Copy(ClipboardData data, ClipboardType type)
		{
			System.Windows.Forms.Clipboard.SetData(GetName(type), JsonSerializer.Serialize(data, options));
		}

		public static ClipboardData Paste(ClipboardType type)
		{
			var json = (string)System.Windows.Forms.Clipboard.GetData(GetName(type));
			if (!string.IsNullOrEmpty(json))
			{
				return JsonSerializer.Deserialize<ClipboardData>(json, options);
			}
			else
			{
				return null;
			}
		}

		public static bool ContainsData(ClipboardType type) => System.Windows.Forms.Clipboard.ContainsData(GetName(type));

		static string GetName(ClipboardType type) => $"WLEditor{type}";
	}
}
