using System;
using System.Collections.Generic;
using System.Linq;

namespace WLEditor
{
	public class HistorySaveState<T>(Func<T> serialize, Action<T> deserialize)
	{
		readonly Stack<T> undo = [];
		readonly Stack<T> redo = [];

		public bool CanUndo => undo.Any();
		public bool CanRedo => redo.Any();
		bool stateSaved;

		public void Clear()
		{
			redo.Clear();
			undo.Clear();
		}

		public void Undo()
		{
			ApplyChanges(undo, redo);
		}

		public void Redo()
		{
			ApplyChanges(redo, undo);
		}

		void ApplyChanges(Stack<T> source, Stack<T> dest)
		{
			if (source.Any())
			{
				dest.Push(serialize());
				deserialize(source.Pop());
			}
		}

		public void SaveState()
		{
			undo.Push(serialize());
			redo.Clear();
		}

		public void SaveStateOnce()
		{
			if (!stateSaved)
			{
				stateSaved = true;
				SaveState();
			}
		}

		public void ClearSaveStateOnce()
		{
			stateSaved = false;
		}
	}
}
