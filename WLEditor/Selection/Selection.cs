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
		int tileSize;

		bool selection;
		Point selectionStart, selectionEnd;
		public event EventHandler<SelectionEventArgs> InvalidateSelection;

		readonly List<List<SelectionChange>> undo = new List<List<SelectionChange>>();
		readonly List<List<SelectionChange>> redo = new List<List<SelectionChange>>();

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

		void GetSelection(out Point start, out Point end)
		{
			int startX = Math.Min(selectionStart.X, selectionEnd.X);
			int startY = Math.Min(selectionStart.Y, selectionEnd.Y);
			int endX = Math.Max(selectionStart.X, selectionEnd.X);
			int endY = Math.Max(selectionStart.Y, selectionEnd.Y);

			start = new Point(startX, startY);
			end = new Point(endX, endY);
		}

		void Invalidate()
		{
			InvalidateSelection(this, new SelectionEventArgs(GetSelectionRectangle()));
		}

		Rectangle GetSelectionRectangle()
		{
			Point start, end;
			GetSelection(out start, out end);
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
				Point start, end;
				GetSelection(out start, out end);

				using (var memoryStream = new MemoryStream())
				using (var writer = new BinaryWriter(memoryStream))
				{
					writer.Write(tileSize);
					writer.Write(end.X - start.X + 1); //width
					writer.Write(end.Y - start.Y + 1); //height

					for (int y = start.Y ; y <= end.Y ; y++)
					{
						for (int x = start.X ; x <= end.X ; x++)
						{
							var item = getTileAt(x, y);
							writer.Write(item.Index);
							writer.Write(item.Tile);
						}
					}

					Clipboard.SetData("WLEditor", memoryStream);
				}

				return true;
			}

			return false;
		}

		public bool DeleteSelection(Func<int, int, int, int> setTileAt, int emptyTile)
		{
			if (selection)
			{
				var changes = new List<SelectionChange>();

				Point start, end;
				GetSelection(out start, out end);

				for (int y = start.Y ; y <= end.Y ; y++)
				{
					for (int x = start.X ; x <= end.X ; x++)
					{
						int previous = setTileAt(x, y, emptyTile);
						if (previous != emptyTile)
						{
							changes.Add(new SelectionChange { X = x, Y =  y, Data = previous });
						}
					}
				}

				AddChanges(changes);
				return changes.Count > 0;
			}

			return false;
		}

		public bool PasteSelection(Func<int, int, int, int> setTileAt)
		{
			if (selection)
			{
				int selectionWidth = 0, selectionHeight = 0;
				ClipboardData[] selectionData = null;

				var memoryStream = (MemoryStream)Clipboard.GetData("WLEditor");
				if (memoryStream != null)
				{
					using (var reader = new BinaryReader(memoryStream))
					{
						if (reader.ReadInt32() == tileSize)
						{
							selectionWidth = reader.ReadInt32();
							selectionHeight = reader.ReadInt32();
							selectionData = Enumerable.Range(0, selectionWidth * selectionHeight)
								.Select((x, i) => new { Order = reader.ReadInt32(), Tile = reader.ReadInt32(), Index = i })
								.OrderBy(x => x.Order)   //used for ordering events
								.Select(x => new ClipboardData { Index = x.Index, Tile = x.Tile })
								.ToArray();
						}
					}
				}

				if (selectionData != null && selectionWidth > 0 && selectionHeight > 0)
				{
					var changes = new List<SelectionChange>();

					bool invertX = selectionStart.X > selectionEnd.X;
					bool invertY = selectionStart.Y > selectionEnd.Y;

					Point start, end;
					GetSelection(out start, out end);

					for (int ty = start.Y ; ty <= end.Y ; ty += selectionHeight)
					for (int tx = start.X ; tx <= end.X ; tx += selectionWidth)
					foreach (var data in selectionData)
					{
						Point dest = new Point
						{
							X = (invertX ? start.X - tx + end.X - selectionWidth  + 1 : tx) + data.Index % selectionWidth,
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

					AddChanges(changes);
					return changes.Any();
				}
			}

			return false;
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

		public bool HasSelection
		{
			get
			{
				return selection;
			}
		}

		#region Undo

		public void ClearUndo()
		{
			redo.Clear();
			undo.Clear();
		}

		public void AddChanges(List<SelectionChange> changes)
		{
			if (changes.Count > 0)
			{
				undo.Add(changes);
				redo.Clear();
			}
		}

		public bool Undo(Func<int, int, int, int> setTileAt, Func<int, int, int> getTileAt)
		{
			return ApplyChanges(setTileAt, getTileAt, undo, redo);
		}

		public bool Redo(Func<int, int, int, int> setTileAt, Func<int, int, int> getTileAt)
		{
			return ApplyChanges(setTileAt, getTileAt, redo, undo);
		}

		bool ApplyChanges(Func<int, int, int, int> setTileAt, Func<int, int, int> getTileAt, List<List<SelectionChange>> source, List<List<SelectionChange>> dest)
		{
			if (source.Count > 0)
			{
				var changes = new List<SelectionChange>();
				foreach (var tile in source.Last())
				{
					changes.Add(new SelectionChange { X = tile.X, Y = tile.Y, Data = getTileAt(tile.X, tile.Y) } );
					setTileAt(tile.X, tile.Y, tile.Data);
				}

				source.RemoveAt(source.Count - 1);
				dest.Add(changes);
				return true;
			}

			return false;
		}

		#endregion
	}
}
