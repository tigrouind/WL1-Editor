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
				var cloner = new Cloner();
				dest.Push(cloner.Clone(serialize()));
				deserialize(source.Pop());
			}
		}

		public void Commit()
		{
			var equality = new Equality();
			if (undo.Any() && equality.Equals(serialize(), undo.Peek()))
			{
				undo.Pop();
			}
		}

		public void SaveState()
		{
			var cloner = new Cloner();
			undo.Push(cloner.Clone(serialize()));
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
