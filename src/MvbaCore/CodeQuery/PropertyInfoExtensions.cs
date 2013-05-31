//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace MvbaCore.CodeQuery
{
	public static class PropertyInfoExtensions
	{
		[NotNull]
		[Pure]
		public static IEnumerable<Attribute> CustomAttributes([NotNull] this PropertyInfo input)
		{
			return input.GetCustomAttributes(true).Cast<Attribute>();
		}

		[NotNull]
		[Pure]
		public static IEnumerable<T> CustomAttributesOfType<T>([NotNull] this PropertyInfo input) where T : Attribute
		{
			return ((MemberInfo)input).CustomAttributesOfType<T>();
		}

		[Pure]
		public static bool HasAttributeOfType<TAttributeType>([NotNull] this PropertyInfo input) where TAttributeType : Attribute
		{
			return input.CustomAttributesOfType<TAttributeType>().Any();
		}

		[NotNull]
		[Pure]
		public static IEnumerable<PropertyInfo> ThatHaveAGetter([NotNull] this IEnumerable<PropertyInfo> input)
		{
			return input.Where(x => x.CanRead);
		}

		[NotNull]
		[Pure]
		public static IEnumerable<PropertyInfo> ThatHaveASetter([NotNull] this IEnumerable<PropertyInfo> input)
		{
			return input.Where(x => x.CanWrite);
		}

		[NotNull]
		[Pure]
		public static IEnumerable<PropertyInfo> WithAttributeOfType<TAttributeType>([NotNull] this IEnumerable<PropertyInfo> input) where TAttributeType : Attribute
		{
			return input.Where(x => x.HasAttributeOfType<TAttributeType>());
		}
	}
}