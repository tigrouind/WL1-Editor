using System;

namespace WLEditor
{
	public class ComboboxItem<T>
	{
		public string Text { get; set; }
		public T Value { get; set; }

		public ComboboxItem(string text, T value)
		{
			Text = text;
			Value = value;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
