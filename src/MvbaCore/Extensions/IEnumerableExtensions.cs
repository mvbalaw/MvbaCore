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

// ReSharper disable CheckNamespace

namespace System.Collections
// ReSharper restore CheckNamespace
{
	public static class IEnumerableExtensions
	{
		[NotNull]
		public static IEnumerable<TOut> ConvertAll<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> convert)
		{
			return items.Select(convert);
		}

		[CanBeNull]
		public static T FirstOrDefault<T>([NotNull] this IEnumerable items, [NotNull] Func<T, bool> @delegate)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items", "collection cannot be null");
			}

			foreach (var item in items.Where(@delegate))
			{
				return item;
			}
			return default(T);
		}

		public static IEnumerable<TOut> Select<TIn, TOut>([NotNull] this IEnumerable items,
		                                                  [NotNull] Func<TIn, TOut> @delegate)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items", "collection cannot be null");
			}
			return from TIn item in items
			       select @delegate(item);
		}

		public static IEnumerable<TOut> Select<TIn, TOut>([NotNull] this IEnumerable items,
		                                                  [NotNull] Func<TIn, int, TOut> @delegate)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items", "collection cannot be null");
			}
			var i = 0;
			foreach (TIn item in items)
			{
				yield return @delegate(item, i);
				i++;
			}
		}

		[NotNull]
		public static IEnumerable<T> Where<T>([NotNull] this IEnumerable items, [NotNull] Func<T, bool> @delegate)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items", "collection cannot be null");
			}
			return from T item in items
			       where @delegate(item)
			       select item;
		}
	}
}