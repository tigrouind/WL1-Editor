using System;

namespace WLEditor
{
	/// <summary>
	/// Description of ComboboxItem.
	/// </summary>
	public class ComboboxItem
	{
	    public string Text { get; set; }
	    public object Value { get; set; }
	    
	    public ComboboxItem(string text, object value)
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
