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

		public void CopySelection(Func<int, int, int> getTileAt)
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

					for(int y = start.Y ; y <= end.Y ; y++)
					{
						for(int x = start.X ; x <= end.X ; x++)
						{
							writer.Write(getTileAt(x, y));
						}
					}

					Clipboard.SetData("WLEditor", memoryStream);
				}
			}
		}

		public bool CutSelection(Func<int, int, int> getTileAt, Action<int, int> clearTileAt)
		{
			if (selection)
			{
				CopySelection(getTileAt);
				DeleteSelection(getTileAt, clearTileAt);
				return true;
			}

			return false;
		}

		public bool DeleteSelection(Func<int, int, int> getTileAt, Action<int, int> clearTileAt)
		{
			if (selection)
			{
				var changes = new List<SelectionChange>();

				Point start, end;
				GetSelection(out start, out end);

				for(int y = start.Y ; y <= end.Y ; y++)
				{
					for(int x = start.X ; x <= end.X ; x++)
					{
						changes.Add(new SelectionChange { X = x, Y =  y, Data = getTileAt(x, y) });
						clearTileAt(x, y);
					}
				}

				AddChanges(changes);
				return true;
			}

			return false;
		}

		public bool PasteSelection(Func<int, int, int, int> setTileAt)
		{
			if (selection)
			{
				int selectionWidth = 0, selectionHeight = 0;
				int[] selectionData = null;

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
								.Select((x, i) => new { Tile = reader.ReadInt32(), Index = i })
								.OrderBy(x => x.Tile >> 16) //used for events
								.Select(x =>  x.Index << 16 | x.Tile & 0xFFFF)
								.ToArray();
						}
					}
				}

				if (selectionData != null && selectionWidth > 0 && selectionHeight > 0)
				{
					var changes = new List<SelectionChange>();

					Point start, end;
					GetSelection(out start, out end);

					for (int ty = start.Y ; ty <= end.Y ; ty += selectionHeight)
					for (int tx = start.X ; tx <= end.X ; tx += selectionWidth)
					foreach (int data in selectionData)
					{
						int destX = tx + ((data >> 16) % selectionWidth);
						int destY = ty + ((data >> 16) / selectionWidth);

						if ((destX <= end.X && destY <= end.Y) || (start.X == end.X && start.Y == end.Y))
						{
							int track = setTileAt(destX, destY, data & 0xFFFF);
							if (track != -1)
							{
								changes.Add(new SelectionChange { X = destX, Y = destY, Data = track });
							}
						}
					}

					AddChanges(changes);
					return true;
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

		public bool Undo(Action<int, int, int> setTileAt, Func<int, int, int> getTileAt)
		{
			return ApplyChanges(setTileAt, getTileAt, undo, redo);
		}

		public bool Redo(Action<int, int, int> setTileAt, Func<int, int, int> getTileAt)
		{
			return ApplyChanges(setTileAt, getTileAt, redo, undo);
		}

		bool ApplyChanges(Action<int, int, int> setTileAt, Func<int, int, int> getTileAt, List<List<SelectionChange>> source, List<List<SelectionChange>> dest)
		{
			if (source.Count > 0)
			{
				var changes = new List<SelectionChange>();
				foreach(var tile in source.Last())
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
