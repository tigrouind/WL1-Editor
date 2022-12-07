using System;
using System.Collections.Generic;

namespace WLEditor
{
	public static class Extensions
	{
		public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate) 
		{		
		    int index = 0;
		    foreach (var item in items) 
		    {
		        if (predicate(item))
		        {
		        	return index;
		        }
		        
		        index++;
		    }
		    return -1;
		}
	}
}
