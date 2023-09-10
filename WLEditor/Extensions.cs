using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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

		public static PointF Normalized(this PointF point)
		{
			float length = (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
			return new PointF(point.X / length, point.Y / length);
		}

		public static IEnumerable<TResult> GroupByAdjacent<TSource, TKey, TResult>(this IEnumerable<TSource> source,
				Func<TSource, TKey> selector,
				Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
		{
			var comparer = EqualityComparer<TKey>.Default;
			using (var enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					List<TSource> buffer = new List<TSource>
					{
						enumerator.Current
					};

					TKey lastKey = selector(enumerator.Current);

					while (enumerator.MoveNext())
					{
						TSource currentItem = enumerator.Current;
						TKey currentKey = selector(currentItem);

						if (!comparer.Equals(lastKey, currentKey))
						{
							yield return resultSelector(lastKey, buffer);
							buffer = new List<TSource>();
							lastKey = currentKey;
						}

						buffer.Add(currentItem);
					}

					yield return resultSelector(lastKey, buffer);
				}
			}
		}

		public static IEnumerable<ToolStripMenuItem> GetAllMenuItems(this ToolStripItemCollection items)
		{
			foreach (ToolStripItem item in items)
			{
				if (item is ToolStripMenuItem toolStrip)
				{
					yield return toolStrip;
				}

				if (item is ToolStripDropDownItem toolStripDropDown)
				{
					foreach (var child in GetAllMenuItems(toolStripDropDown.DropDownItems))
					{
						yield return child;
					}
				}
			}
		}
	}
}
