using System;
using System.Linq;

namespace CQRS.Sample.Extensions
{
	public static class GenericTypeExtensions
	{
		public static string GetGenericTypeName(this Type type)
		{
			string typeName = string.Empty;

			if (type.IsGenericType)
			{
				string genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
				typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
			}
			else
			{
				typeName = type.Name;
			}

			return typeName;
		}

		public static string GetGenericTypeName(this object @object) => @object.GetType().GetGenericTypeName();
	}
}
