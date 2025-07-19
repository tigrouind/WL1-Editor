using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WLEditor
{
	public class Selection(int tileSize)
	{
		int zoom;
		bool selection;
		Point selectionStart, selectionEnd;
		public event EventHandler<SelectionEventArgs> InvalidateSelection;

		public void SetZoom(int zoomLevel)
		{
			zoom = zoomLevel;
		}

		public void DrawSelection(Graphics g)
		{
			if (selection)
			{
				using SolidBrush brush = new(Color.FromArgb(128, 255, 255, 0));
				var rect = GetSelectionRectangle();
				g.FillRectangle(brush, rect);
			}
		}

		Rectangle GetSelection()
		{
			int startX = Math.Min(selectionStart.X, selectionEnd.X);
			int startY = Math.Min(selectionStart.Y, selectionEnd.Y);
			int endX = Math.Max(selectionStart.X, selectionEnd.X);
			int endY = Math.Max(selectionStart.Y, selectionEnd.Y);

			return new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);
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
			var selection = GetSelection();
			return new Rectangle(
				selection.X * tileSize * zoom,
				selection.Y * tileSize * zoom,
				selection.Width * tileSize * zoom,
				selection.Height * tileSize * zoom);
		}

		public ClipboardData CopySelection(Func<int, int, ClipboardTile> getTileAt, ClipboardType type)
		{
			if (selection)
			{
				var selection = GetSelection();
				var items = new List<ClipboardTile>();
				for (int y = selection.Top; y < selection.Bottom; y++)
				{
					for (int x = selection.Left; x < selection.Right; x++)
					{
						var item = getTileAt(x, y);
						items.Add(item);
					}
				}

				var data = new ClipboardData
				{
					TileWidth = selection.Width,
					TileHeight = selection.Height,
					Tiles = [.. items.Select(x => new ClipboardTile { Tile = x.Tile, Order = x.Order })]
				};

				Clipboard.Copy(data, type);
				return data;
			}

			return null;
		}

		public List<SelectionChange> DeleteSelection(Func<int, int, int, int> setTileAt, int emptyTile)
		{
			if (selection)
			{
				var changes = new List<SelectionChange>();

				var selection = GetSelection();

				for (int y = selection.Top; y < selection.Bottom; y++)
				{
					for (int x = selection.Left; x < selection.Right; x++)
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

		public List<SelectionChange> PasteSelection(Func<int, int, int, int> setTileAt, ClipboardType type)
		{
			if (selection)
			{
				var changes = new List<SelectionChange>();

				var data = Clipboard.Paste(type);
				if (data != null && data.TileWidth > 0 && data.TileHeight > 0)
				{
					var items = data.Tiles
							.Select((x, i) => (x.Tile, x.Order, Index: i))
							.OrderBy(x => x.Order)   //used for ordering events
							.Select(x => (x.Tile, x.Index))
							.ToArray();

					bool invertX = selectionStart.X > selectionEnd.X;
					bool invertY = selectionStart.Y > selectionEnd.Y;

					var selection = GetSelection();

					for (int ty = selection.Top; ty < selection.Bottom; ty += data.TileHeight)
					{
						for (int tx = selection.Left; tx < selection.Right; tx += data.TileWidth)
						{
							foreach (var item in items)
							{
								Point dest = new()
								{
									X = (invertX ? selection.Left - tx + selection.Right - data.TileWidth : tx) + item.Index % data.TileWidth,
									Y = (invertY ? selection.Top - ty + selection.Bottom - data.TileHeight : ty) + item.Index / data.TileWidth
								};

								if (selection.Contains(dest) || selection.Size == new Size(1, 1))
								{
									int tile = item.Tile;
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
