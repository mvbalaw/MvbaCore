using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace CodeQuery
{
	public static class PropertyInfoExtensions
	{
		[NotNull]
		public static IEnumerable<Attribute> CustomAttributes([NotNull] this PropertyInfo input)
		{
			return input.GetCustomAttributes(true).Cast<Attribute>();
		}

		[NotNull]
		public static IEnumerable<T> CustomAttributesOfType<T>([NotNull] this PropertyInfo input) where T : Attribute
		{
			return ((MemberInfo)input).CustomAttributesOfType<T>();
		}

		public static bool HasAttributeOfType<TAttributeType>([NotNull] this PropertyInfo input) where TAttributeType : Attribute
		{
			return input.CustomAttributesOfType<TAttributeType>().Any();
		}

		[NotNull]
		public static IEnumerable<PropertyInfo> ThatHaveAGetter([NotNull] this IEnumerable<PropertyInfo> input)
		{
			return input.Where(x => x.CanRead);
		}

		[NotNull]
		public static IEnumerable<PropertyInfo> ThatHaveASetter([NotNull] this IEnumerable<PropertyInfo> input)
		{
			return input.Where(x => x.CanWrite);
		}

		[NotNull]
		public static IEnumerable<PropertyInfo> WithAttributeOfType<TAttributeType>([NotNull] this IEnumerable<PropertyInfo> input) where TAttributeType : Attribute
		{
			return input.Where(x => x.HasAttributeOfType<TAttributeType>());
		}
	}
}