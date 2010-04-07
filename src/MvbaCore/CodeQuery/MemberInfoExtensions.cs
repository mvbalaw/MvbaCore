using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace CodeQuery
{
	internal static class MemberInfoExtensions
	{
		[NotNull]
		internal static IEnumerable<T> CustomAttributesOfType<T>([NotNull] this MemberInfo input) where T : Attribute
		{
			return input.GetCustomAttributes(typeof(T), true).Cast<T>();
		}
	}
}