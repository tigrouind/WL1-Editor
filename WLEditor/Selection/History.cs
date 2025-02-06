using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLEditor
{
	public class History
	{
		readonly List<List<SelectionChange>> undo = new List<List<SelectionChange>>();
		readonly List<List<SelectionChange>> redo = new List<List<SelectionChange>>();

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
					changes.Add(new SelectionChange { X = tile.X, Y = tile.Y, Data = getTileAt(tile.X, tile.Y) });
					setTileAt(tile.X, tile.Y, tile.Data);
				}

				source.RemoveAt(source.Count - 1);
				dest.Add(changes);
				return true;
			}

			return false;
		}
	}
}
