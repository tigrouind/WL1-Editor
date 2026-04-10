using System;
using System.Collections.Generic;
using System.Linq;

namespace WLEditor
{
	public class History
	{
		readonly Stack<List<SelectionChange>> undo = [];
		readonly Stack<List<SelectionChange>> redo = [];

		public bool CanUndo => undo.Any();
		public bool CanRedo => redo.Any();
		public EventHandler Change;

		public void ClearUndo()
		{
			redo.Clear();
			undo.Clear();
			HistoryChange();
		}

		public void AddChanges(List<SelectionChange> changes)
		{
			if (changes.Count > 0)
			{
				undo.Push(changes);
				redo.Clear();
				HistoryChange();
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

		bool ApplyChanges(Func<int, int, int, int> setTileAt, Func<int, int, int> getTileAt, Stack<List<SelectionChange>> source, Stack<List<SelectionChange>> dest)
		{
			if (source.Count > 0)
			{
				var changes = new List<SelectionChange>();
				foreach (var tile in source.Peek())
				{
					changes.Add(new SelectionChange { X = tile.X, Y = tile.Y, Data = getTileAt(tile.X, tile.Y) });
					setTileAt(tile.X, tile.Y, tile.Data);
				}

				source.Pop();
				dest.Push(changes);
				HistoryChange();
				return true;
			}

			return false;
		}

		void HistoryChange()
		{
			Change?.Invoke(this, EventArgs.Empty);
		}
	}
}
