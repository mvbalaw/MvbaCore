//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using MvbaCore.CodeQuery;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class ReflectionExtensions
	{
		[NotNull]
		public static IEnumerable<T> GetCustomAttribute<T>([NotNull] this Type typeThatHasTheProperty,
														   [NotNull] string propertyName) where T : Attribute
		{
			var attributes = typeThatHasTheProperty.GetProperty(propertyName).CustomAttributesOfType<T>();
			return attributes;
		}

		[CanBeNull]
		public static T GetFirstPropertyCustomAttribute<T>([NotNull] this Type typeThatHasTheProperty,
														   [NotNull] string propertyName) where T : Attribute
		{
			return GetCustomAttribute<T>(typeThatHasTheProperty, propertyName).FirstOrDefault();
		}
	}
}