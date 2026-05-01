using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace WLEditor
{
	public class Equality
	{
		readonly HashSet<(object A, object B)> alreadyChecked = [];

		public bool Equals<T>(T a, T b)
		{
			if (a == null && b == null)
			{
				return true;
			}

			if (a == null || b == null)
			{
				return false;
			}

			if (!alreadyChecked.Add((a, b)))
			{
				return true;
			}

			var sourceTypeA = a.GetType();
			var sourceTypeB = b.GetType();
			if (sourceTypeA != sourceTypeB)
			{
				return false;
			}

			if (sourceTypeA.IsValueType || sourceTypeA == typeof(string))
			{
				return a.Equals(b);
			}

			if (a is IEnumerable ia && b is IEnumerable ib) //array or List<T>
			{
				if (!EqualsEnumerable(ia, ib))
				{
					return false; //don't bother comparing fields (eg: Capacity might be different for List<T>)
				}
			}

			return EqualsObject();

			bool EqualsEnumerable(IEnumerable a, IEnumerable b)
			{
				var e1 = a.GetEnumerator();
				var e2 = b.GetEnumerator();

				while (e1.MoveNext())
				{
					if (!(e2.MoveNext() && Equals(e1.Current, e2.Current)))
					{
						return false;
					}
				}

				return !e2.MoveNext();
			}

			bool EqualsObject()
			{
				foreach (var item in sourceTypeA.GetFields(BindingFlags.Public | BindingFlags.Instance))
				{
					if (!Equals(item.GetValue(a), item.GetValue(b)))
					{
						return false;
					}
				}

				return true;
			}
		}


	}
}