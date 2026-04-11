using System.Text.Json;

namespace WLEditor
{
	public class Clipboard
	{
		static readonly JsonSerializerOptions options = new() { IncludeFields = true };

		public static void Copy(ClipboardData data, ClipboardType type)
		{
			SetData(JsonSerializer.Serialize(data, options), type);
		}

		public static ClipboardData Paste(ClipboardType type)
		{
			var json = GetData(type);
			if (!string.IsNullOrEmpty(json))
			{
				return JsonSerializer.Deserialize<ClipboardData>(json, options);
			}
			else
			{
				return null;
			}
		}

		public static bool HasData(ClipboardType type) => !string.IsNullOrEmpty(GetData(type));

		static string GetData(ClipboardType type) => (string)System.Windows.Forms.Clipboard.GetData($"WLEditor{type}");

		static void SetData(string data, ClipboardType type) => System.Windows.Forms.Clipboard.SetData($"WLEditor{type}", data);
	}
}
