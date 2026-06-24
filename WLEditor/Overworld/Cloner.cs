using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace WLEditor;

public class Cloner
{
	readonly Dictionary<object, object> instances = [];

	public T Clone<T>(T source)
	{
		if (instances.TryGetValue(source, out var clone))
		{
			return (T)clone;
		}

		var sourceType = source.GetType();
		if (sourceType.IsArray)
		{
			return (T)CloneArray();
		}

		if (source is IList)
		{
			return (T)CloneList();
		}

		if (sourceType.IsValueType || sourceType == typeof(string))
		{
			return source;
		}

		return (T)CloneObject();

		object CloneArray()
		{
			var type = sourceType.GetElementType();
			var array = source as Array;
			var clonedArray = Array.CreateInstance(type, array.Length);
			instances.Add(source, clonedArray);

			for (int i = 0; i < array.Length; i++)
			{
				var item = array.GetValue(i);
				if (item != null)
				{
					clonedArray.SetValue(Clone(item), i);
				}
			}

			return clonedArray;
		}

		object CloneList()
		{
			var type = sourceType.GetGenericArguments()[0];
			var list = source as IList;
			var clonedList = (IList)Activator.CreateInstance(sourceType, list.Count);
			instances.Add(source, clonedList);

			foreach (var item in list)
			{
				clonedList.Add(item == null ? null : Clone(item));
			}
			return clonedList;
		}

		object CloneObject()
		{
			var target = Activator.CreateInstance(sourceType);
			instances.Add(source, target);

			foreach (var item in sourceType.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				var value = item.GetValue(source);
				if (value != null)
				{
					item.SetValue(target, Clone(value));
				}
			}

			return target;
		}
	}
}
