using System.Collections.Generic;

namespace WLEditor
{
	public class ComboboxItemCollection<T> : List<ComboboxItem<T>>
	{
		public void Add(T value, string text) => Add(new ComboboxItem<T>(value, text));
	}
}
