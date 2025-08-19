using System;
using System.Collections.Generic;
using System.Reflection;

namespace WLEditor
{
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

			return (T)CloneObject();

			object CloneArray()
			{
				var type = sourceType.GetElementType();
				var array = source as Array;
				var clonedArray = Array.CreateInstance(type, array.Length);
				instances.Add(source, clonedArray);

				for (int i = 0; i < array.Length; i++)
				{
					var value = array.GetValue(i);
					if (value != null)
					{
						clonedArray.SetValue(CloneField(value, type), i);
					}
				}

				return clonedArray;
			}

			object CloneObject()
			{
				var target = Activator.CreateInstance(sourceType);
				instances.Add(source, target);
				foreach (var item in sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				{
					var value = item.GetValue(source);
					if (value != null)
					{
						item.SetValue(target, CloneField(value, item.FieldType));
					}
				}

				return target;
			}

			object CloneField(object value, Type type)
			{
				if (type.IsValueType || type == typeof(string))
				{
					return value;
				}
				else
				{
					return Clone(value);
				}
			}
		}
	}
}
