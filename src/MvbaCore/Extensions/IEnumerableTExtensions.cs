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

// ReSharper disable PossibleMultipleEnumeration
			foreach (var item in items)
// ReSharper restore PossibleMultipleEnumeration
			{
				action(item);
			}
// ReSharper disable PossibleMultipleEnumeration
			return items;
// ReSharper restore PossibleMultipleEnumeration
		}

		public static IEnumerable<IEnumerable<T>> Group<T>([NotNull] this IEnumerable<T> input, [NotNull] Func<T, T, bool> keepGrouping)
		{
			using (var enumerator = input.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					yield break;
				}

				bool hasMore;
				do
				{
					var @group = new ContinuingEnumerator<T>(enumerator, keepGrouping, enumerator.Current);
					yield return @group;
					hasMore = @group.HasNext;
				} while (hasMore);
			}
		}

		[NotNull]
		public static IEnumerable<List<T>> InSetsOf<T>([NotNull] this IEnumerable<T> items, int setSize)
		{
			return items.InSetsOf(setSize, false, default(T));
		}

		[NotNull]
		public static IEnumerable<List<T>> InSetsOf<T>([NotNull] this IEnumerable<T> items, int setSize, bool fillPartialSetWithDefaultItems, T defaultItemToFillGroups)
		{
			var counter = 0;
			Func<T, T, bool> keepGoing = (current, previous) =>
				{
					if (++counter > setSize)
					{
						counter = 0;
					}
					return counter != 0;
				};
			foreach (var list in items.Group(keepGoing).Select(set => set.ToList()))
			{
				if (list.Count < setSize && fillPartialSetWithDefaultItems)
				{
					list.AddRange(Enumerable.Repeat(defaultItemToFillGroups, setSize - list.Count));
				}
				yield return list;
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
			if (items != null)
			{
				delimiter = delimiter ?? "";
				foreach (var item in items)
				{
					result.Append(item);
					result.Append(delimiter);
				}
				if (result.Length > 0)
				{
					result.Length -= delimiter.Length;
				}
			}
			return result.ToString();
		}

		[NotNull]
		public static HashSet<T> ToHashSet<T>([NotNull] this IEnumerable<T> items)
		{
			return new HashSet<T>(items);
		}
	}

	public class ContinuingEnumerator<T> : IEnumerable<T>
	{
		private readonly IEnumerator<T> _enumerator;
		private readonly Func<T, T, bool> _keepGrouping;

		public ContinuingEnumerator(IEnumerator<T> enumerator, Func<T, T, bool> keepGrouping, T current)
		{
			Current = current;
			_enumerator = enumerator;
			_keepGrouping = keepGrouping;
		}

		public T Current { get; private set; }

		public bool HasNext { get; private set; }

		public IEnumerator<T> GetEnumerator()
		{
			var previous = Current;
			do
			{
				if (_keepGrouping(_enumerator.Current, previous))
				{
					previous = _enumerator.Current;
					yield return _enumerator.Current;
					continue;
				}
				HasNext = true;
				Current = _enumerator.Current;
				yield break;
			} while (_enumerator.MoveNext());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}