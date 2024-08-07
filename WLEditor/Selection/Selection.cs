using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	public class Selection
	{
		int zoom;
		readonly int tileSize;

		bool selection;
		Point selectionStart, selectionEnd;
		public event EventHandler<SelectionEventArgs> InvalidateSelection;

		public Selection(int tileSize)
		{
			this.tileSize = tileSize;
		}

		public void SetZoom(int zoomLevel)
		{
			zoom = zoomLevel;
		}

		public void DrawSelection(Graphics g)
		{
			if (selection)
			{
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 0)))
				{
					var rect = GetSelectionRectangle();
					g.FillRectangle(brush, rect);
				}
			}
		}

		(Point start, Point end) GetSelection()
		{
			int startX = Math.Min(selectionStart.X, selectionEnd.X);
			int startY = Math.Min(selectionStart.Y, selectionEnd.Y);
			int endX = Math.Max(selectionStart.X, selectionEnd.X);
			int endY = Math.Max(selectionStart.Y, selectionEnd.Y);

			var start = new Point(startX, startY);
			var end = new Point(endX, endY);
			return (start, end);
		}

		public int GetCurrentTile(Func<int, int, int> getIndex)
		{
			if (selection && selectionStart.X == selectionEnd.X && selectionStart.Y == selectionEnd.Y)
			{
				return getIndex(selectionStart.X, selectionEnd.Y);
			}
			else
			{
				return -1;
			}
		}

		void Invalidate()
		{
			InvalidateSelection(this, new SelectionEventArgs(GetSelectionRectangle()));
		}

		Rectangle GetSelectionRectangle()
		{
			var (start, end) = GetSelection();
			return new Rectangle(
				start.X * tileSize * zoom,
				start.Y * tileSize * zoom,
				((end.X - start.X + 1) * tileSize) * zoom,
				((end.Y - start.Y + 1) * tileSize) * zoom);
		}

		public bool CopySelection(Func<int, int, ClipboardData> getTileAt)
		{
			if (selection)
			{
				var (start, end) = GetSelection();
				CopyToClipboard(start, end);

				return true;
			}

			return false;

			void CopyToClipboard(Point start, Point end)
			{
				using (var memoryStream = new MemoryStream())
				using (var writer = new BinaryWriter(memoryStream))
				{
					writer.Write(tileSize);
					writer.Write(end.X - start.X + 1); //width
					writer.Write(end.Y - start.Y + 1); //height

					for (int y = start.Y; y <= end.Y; y++)
					{
						for (int x = start.X; x <= end.X; x++)
						{
							var item = getTileAt(x, y);
							writer.Write(item.Index);
							writer.Write(item.Tile);
						}
					}

					Clipboard.SetData("WLEditor", memoryStream);
				}
			}
		}

		public List<SelectionChange> DeleteSelection(Func<int, int, int, int> setTileAt, int emptyTile)
		{
			if (selection)
			{
				var changes = new List<SelectionChange>();

				var (start, end) = GetSelection();

				for (int y = start.Y; y <= end.Y; y++)
				{
					for (int x = start.X; x <= end.X; x++)
					{
						int previous = setTileAt(x, y, emptyTile);
						if (previous != emptyTile)
						{
							changes.Add(new SelectionChange { X = x, Y = y, Data = previous });
						}
					}
				}

				return changes;
			}

			return null;
		}

		public List<SelectionChange> PasteSelection(Func<int, int, int, int> setTileAt)
		{
			if (selection)
			{
				var (selectionWidth, selectionHeight, selectionData) = GetDataFromClipboard();
				var changes = new List<SelectionChange>();

				if (selectionData != null && selectionWidth > 0 && selectionHeight > 0)
				{
					bool invertX = selectionStart.X > selectionEnd.X;
					bool invertY = selectionStart.Y > selectionEnd.Y;

					var (start, end) = GetSelection();

					for (int ty = start.Y; ty <= end.Y; ty += selectionHeight)
					{
						for (int tx = start.X; tx <= end.X; tx += selectionWidth)
						{
							foreach (var data in selectionData)
							{
								Point dest = new Point
								{
									X = (invertX ? start.X - tx + end.X - selectionWidth + 1 : tx) + data.Index % selectionWidth,
									Y = (invertY ? start.Y - ty + end.Y - selectionHeight + 1 : ty) + data.Index / selectionWidth
								};

								if ((dest.X >= start.X && dest.Y >= start.Y && dest.X <= end.X && dest.Y <= end.Y)
									|| (start.X == end.X && start.Y == end.Y))
								{
									int tile = data.Tile;
									int previous = setTileAt(dest.X, dest.Y, tile);
									if (previous != tile)
									{
										changes.Add(new SelectionChange { X = dest.X, Y = dest.Y, Data = previous });
									}
								}
							}
						}
					}
				}

				return changes;
			}

			return null;

			(int selectionWidth, int selectionHeight, ClipboardData[] selectionData) GetDataFromClipboard()
			{
				var memoryStream = (MemoryStream)Clipboard.GetData("WLEditor");
				if (memoryStream != null)
				{
					using (var reader = new BinaryReader(memoryStream))
					{
						if (reader.ReadInt32() == tileSize)
						{
							int selectionWidth = reader.ReadInt32();
							int selectionHeight = reader.ReadInt32();
							ClipboardData[] selectionData = Enumerable.Range(0, selectionWidth * selectionHeight)
								.Select((x, i) => (Order: reader.ReadInt32(), Tile: reader.ReadInt32(), Index: i ))
								.OrderBy(x => x.Order)   //used for ordering events
								.Select(x => new ClipboardData { Index = x.Index, Tile = x.Tile })
								.ToArray();

							return (selectionWidth, selectionHeight, selectionData);
						}
					}
				}

				return (0, 0, null);
			}
		}

		public void StartSelection(int x, int y)
		{
			if (selection)
			{
				Invalidate();
			}

			selectionStart = new Point(x, y);
			selectionEnd = new Point(x, y);
			selection = true;

			if (selection)
			{
				Invalidate();
			}
		}

		public void SetSelection(int x, int y)
		{
			if (selection)
			{
				Invalidate();
			}

			selectionEnd = new Point(x, y);

			if (selection)
			{
				Invalidate();
			}
		}

		public void ClearSelection()
		{
			if (selection)
			{
				selection = false;
				Invalidate();
			}
		}

		public bool HasSelection => selection;
	}
}
