using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLEditor
{
	public class Clipboard
	{
		public static void CopyToClipboard( int tileSize, int width, int height,List<ClipboardData> data)
		{
			using var memoryStream = new MemoryStream();
			using var writer = new BinaryWriter(memoryStream);
			writer.Write(tileSize);
			writer.Write(width); //width
			writer.Write(height); //height

			foreach(var item in data)
			{
				writer.Write(item.Index);
				writer.Write(item.Tile);
			}

			System.Windows.Forms.Clipboard.SetData("WLEditor", memoryStream);
		}

		public static (int width, int height, ClipboardData[] data) GetDataFromClipboard(int tileSize)
		{
			var memoryStream = (MemoryStream)System.Windows.Forms.Clipboard.GetData("WLEditor");
			if (memoryStream != null)
			{
				using var reader = new BinaryReader(memoryStream);
				if (reader.ReadInt32() == tileSize)
				{
					int selectionWidth = reader.ReadInt32();
					int selectionHeight = reader.ReadInt32();
					ClipboardData[] selectionData = [.. Enumerable.Range(0, selectionWidth * selectionHeight)
							.Select((x, i) => (Order: reader.ReadInt32(), Tile: reader.ReadInt32(), Index: i))
							.OrderBy(x => x.Order)   //used for ordering events
							.Select(x => new ClipboardData { Index = x.Index, Tile = x.Tile })];

					return (selectionWidth, selectionHeight, selectionData);
				}
			}

			return (0, 0, null);
		}
	}
}
