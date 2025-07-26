using System;
using System.Reflection;

namespace WLEditor
{
	public class Cloner //do not allow cycles
	{
		public static T Clone<T>(T source)
		{
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
				if (type.IsValueType || type.IsEnum || type == typeof(string))
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
