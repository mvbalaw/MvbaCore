//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

namespace System.Linq
{
	public static class IEnumerableExtensions
	{
		[NotNull]
		public static int Max<T>([NotNull] this IEnumerable<T> items, Func<T, int> selector, int @default)
		{
			if (items.IsNullOrEmpty())
			{
				return @default;
			}
			return items.Max(selector);
		}
	}
}

namespace System.Collections.Generic
{
	public static class IEnumerableTExtensions
	{
		[NotNull]
		public static IEnumerable<T> ForEach<T>([NotNull] this IEnumerable<T> items, [NotNull] Action<T> action)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items", "collection cannot be null");
			}

			foreach (var item in items)
			{
				action(item);
			}
			return items;
		}

		[NotNull]
		public static IEnumerable<List<T>> InSetsOf<T>([NotNull] this IEnumerable<T> items, int setSize)
		{
			return items.InSetsOf(setSize, false, default(T));
		}

		[NotNull]
		public static IEnumerable<List<T>> InSetsOf<T>([NotNull] this IEnumerable<T> items, int setSize, bool fillPartialSetWithDefaultItems, T defaultItemToFillGroups)
		{
			var set = new List<T>(setSize);
			foreach (var item in items)
			{
				set.Add(item);
				if (set.Count == setSize)
				{
					yield return set.ToList();
					set.Clear();
				}
			}

			if (set.Count > 0)
			{
				if (fillPartialSetWithDefaultItems)
				{
					while (set.Count < setSize)
					{
						set.Add(defaultItemToFillGroups);
					}
				}
				yield return set;
			}
		}

		public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> list)
		{
			return list == null || !list.Any();
		}

		[NotNull]
		public static string Join<T>([CanBeNull] this IEnumerable<T> items, [CanBeNull] string delimiter)
		{
			var result = new StringBuilder();
			if (items != null && items.Any())
			{
				delimiter = delimiter ?? "";
				foreach (var item in items)
				{
					result.Append(item);
					result.Append(delimiter);
				}
				result.Length = result.Length - delimiter.Length;
			}
			return result.ToString();
		}

		[NotNull]
		public static HashSet<T> ToHashSet<T>([NotNull] this IEnumerable<T> items)
		{
			return new HashSet<T>(items);
		}
	}
}