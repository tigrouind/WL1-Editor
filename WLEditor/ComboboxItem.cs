using System;

namespace WLEditor
{
	public class ComboboxItem<T>
	{
		public string Text { get; set; }
		public T Value { get; set; }
		public string AdditionalText { get; set; }
		
		public ComboboxItem(T value)
		{
			Value = value;
		}

		public ComboboxItem(T value, string text)
		{
			Text = text;
			Value = value;
		}

		public override string ToString()
		{
			if (AdditionalText != null)
			{
				return Text + " " + AdditionalText;
			}
			
			return Text;
		}
	}
}
