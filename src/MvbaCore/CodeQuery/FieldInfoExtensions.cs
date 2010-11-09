using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace CodeQuery
{
	public static class FieldInfoExtensions
	{
		[NotNull]
		public static IEnumerable<T> CustomAttributesOfType<T>([NotNull] this FieldInfo input) where T : Attribute
		{
			return ((MemberInfo)input).CustomAttributesOfType<T>();
		}

		public static bool HasAttributeOfType<TAttributeType>([NotNull] this FieldInfo input) where TAttributeType : Attribute
		{
			return input.CustomAttributesOfType<TAttributeType>().Any();
		}

		[NotNull]
		public static IEnumerable<FieldInfo> ThatAreStatic([NotNull] this IEnumerable<FieldInfo> items)
		{
			return items.Where(x => x.IsStatic);
		}

		[NotNull]
		public static IEnumerable<FieldInfo> WithAttributeOfType<TAttributeType>([NotNull] this IEnumerable<FieldInfo> input) where TAttributeType : Attribute
		{
			return input.Where(x => x.HasAttributeOfType<TAttributeType>());
		}
	}
}