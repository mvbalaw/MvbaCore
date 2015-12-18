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
using System.Text;

using JetBrains.Annotations;

using MvbaCore.Collections;

namespace System.Linq
{
	public static class IEnumerableTExtensions
	{
		public static int Max<T>([CanBeNull] this IEnumerable<T> items, Func<T, int> selector, int @default)
		{
			if (items == null)
			{
				return @default;
			}
			return items.Select(selector).DefaultIfEmpty(@default).Max();
		}
	}
}

namespace System.Collections.Generic
{
	public static class IEnumerableTExtensions
	{
		[NotNull]
		public static IEnumerable<T> Except<T, TKey>([NotNull][ItemCanBeNull] this IEnumerable<T> a, [CanBeNull][ItemCanBeNull] IEnumerable<T> other, Func<T, TKey> getJoinKey)
		{
			return a.Except(other, getJoinKey, getJoinKey, item => ReferenceEquals(item, null) || item.Equals(default(T)));
		}

		[NotNull]
		public static IEnumerable<T> Except<T, TKey>([NotNull][ItemCanBeNull] this IEnumerable<T> a, [CanBeNull][ItemCanBeNull] IEnumerable<T> other, Func<T, TKey> getJoinKey,
			Func<T, bool> isNullOrEmptyComparer)
		{
			return a.Except(other, getJoinKey, getJoinKey, isNullOrEmptyComparer);
		}

		[NotNull]
		[ContractAnnotation("a:null => halt; other:null => notnull")]
		public static IEnumerable<T> Except<T, TK, TKey>([NotNull][ItemCanBeNull]this IEnumerable<T> a, [CanBeNull][ItemCanBeNull]IEnumerable<TK> other,
			Func<T, TKey> getJoinKeyForItemsInThis,
			Func<TK, TKey> getJoinKeyForItemsInOther,
			Func<TK, bool> isNullOrEmptyComparer)
		{
			if (a == null)
			{
				throw new ArgumentNullException("a", "list being selected from cannot be null");
			}
			if (other == null)
			{
				return a;
			}
			var otherLocal = other.ToList();
			if (!otherLocal.Any())
			{
				return a;
			}
			var aLocal = a.ToList();
			if (!aLocal.Any())
			{
				return aLocal;
			}

			return from itemA in aLocal
				join itemB in otherLocal on getJoinKeyForItemsInThis(itemA) equals getJoinKeyForItemsInOther(itemB) into c
				from itemC in c.DefaultIfEmpty()
				where isNullOrEmptyComparer(itemC)
				select itemA;
		}

		[NotNull]
		public static IEnumerable<KeyValuePair<int, T>> FlattenRanges<T>([NotNull][ItemNotNull] this IEnumerable<Range<T>> items)
		{
			return items.SelectMany(x => Enumerable.Range(x.Start, x.End - x.Start + 1)
				.Select(y => new KeyValuePair<int, T>(y, x.Payload)));
		}

		[NotNull]
		[ContractAnnotation("items:null => halt; action:null => halt")]
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

		[ItemNotNull]
		[NotNull]
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
		public static IEnumerable<List<T>> InSetsOf<T>([NotNull] this IEnumerable<T> items, int setSize, bool fillPartialSetWithDefaultItems, [CanBeNull] T defaultItemToFillGroups)
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

		[NotNull]
		[ContractAnnotation("a:null => halt; other:null => notnull; getJoinKey:null => halt")]
		public static IEnumerable<T> Intersect<T, TKey>([NotNull][ItemCanBeNull] this IEnumerable<T> a, [CanBeNull][ItemCanBeNull] IEnumerable<T> other, [NotNull] Func<T, TKey> getJoinKey)
		{
			if (a == null)
			{
				throw new ArgumentNullException("a", "list being selected from cannot be null");
			}
			var aLocal = a.ToList();
			if (!aLocal.Any())
			{
				return aLocal;
			}
			if (other == null)
			{
				return new List<T>();
			}
			var otherLocal = other.ToList();
			if (!otherLocal.Any())
			{
				return new List<T>();
			}

			return from itemA in aLocal
				join itemB in otherLocal on getJoinKey(itemA) equals getJoinKey(itemB)
				select itemA;
		}

		[ContractAnnotation("list:null => true")]
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
		public static CachedEnumerable<T> Memoize<T>([NotNull] this IEnumerable<T> enumerable, int? max = null)
		{
			return new CachedEnumerable<T>(enumerable, max);
		}

		[NotNull]
		public static string SeparateWith<T>([NotNull] this IEnumerable<T> input, [NotNull] Func<T, string> getValueToSeparate, [NotNull] string separator)
		{
			return input.Select(getValueToSeparate).Join(separator);
		}

		/// <summary>
		///     synchronizes previousItems with newItems and returns the results.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="newItems">all current items, order does not matter</param>
		/// <param name="getItemKeyValue"> e.g. county=>county.Id</param>
		/// <param name="equals">e.g.  (county1,County2)=>county1.Name == county2.Name && county1.Gps == county2.Gps</param>
		/// <param name="previousItems">all previous items, order does not matter</param>
		[NotNull]
		public static IEnumerable<SynchronizationResult<T>> Synchronize<T, TKey>(
			[NotNull] this IEnumerable<T> previousItems, 
			[NotNull] IEnumerable<T> newItems,
			[NotNull] Func<T, TKey> getItemKeyValue,
			[NotNull] Func<T, T, bool> equals)
			where TKey : IComparable
		{
			return SynchronizeInternal(previousItems.OrderBy(getItemKeyValue), newItems.OrderBy(getItemKeyValue), getItemKeyValue, @equals);
		}

		/// <summary>
		///     synchronizes previousItems with newItems and returns the results.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="newItems">all current items, order does not matter</param>
		/// <param name="getItemKeyValue"> e.g. county=>county.Id</param>
		/// <param name="equals">e.g.  (county1,County2)=>county1.Name == county2.Name && county1.Gps == county2.Gps</param>
		/// <param name="previousItems">all previous items, sorted by KeyValue</param>
		[NotNull]
		public static IEnumerable<SynchronizationResult<T>> Synchronize<T, TKey>(
			this IOrderedEnumerable<T> previousItems,
			[NotNull] IEnumerable<T> newItems,
			[NotNull] Func<T, TKey> getItemKeyValue,
			[NotNull] Func<T, T, bool> equals)
			where TKey : IComparable
		{
			return SynchronizeInternal(previousItems, newItems.OrderBy(getItemKeyValue), getItemKeyValue, @equals);
		}

		/// <summary>
		///     synchronizes previousItems with newItems and returns the results.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="newItems">all current items, sorted by KeyValue</param>
		/// <param name="getItemKeyValue"> e.g. county=>county.Id</param>
		/// <param name="equals">e.g.  (county1,County2)=>county1.Name == county2.Name && county1.Gps == county2.Gps</param>
		/// <param name="previousItems">all previous items, sorted by KeyValue</param>
		public static IEnumerable<SynchronizationResult<T>> Synchronize<T, TKey>(
			[NotNull] this IOrderedEnumerable<T> previousItems,
			[NotNull] IOrderedEnumerable<T> newItems,
			[NotNull] Func<T, TKey> getItemKeyValue,
			[NotNull] Func<T, T, bool> equals)
			where TKey : IComparable
		{
			return SynchronizeInternal(previousItems, newItems.OrderBy(getItemKeyValue), getItemKeyValue, @equals);
		}

		/// <summary>
		///     synchronizes previousItems with newItems and returns the results.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="newItems">all current items, sorted by KeyValue</param>
		/// <param name="getItemKeyValue"> e.g. county=>county.Id</param>
		/// <param name="equals">e.g.  (county1,County2)=>county1.Name == county2.Name && county1.Gps == county2.Gps</param>
		/// <param name="previousItems">all previous items, order does not matter</param>
		public static IEnumerable<SynchronizationResult<T>> Synchronize<T, TKey>(
			[NotNull] this IEnumerable<T> previousItems,
			[NotNull] IOrderedEnumerable<T> newItems,
			[NotNull] Func<T, TKey> getItemKeyValue,
			[NotNull] Func<T, T, bool> equals)
			where TKey : IComparable
		{
			return SynchronizeInternal(previousItems.OrderBy(getItemKeyValue), newItems, getItemKeyValue, @equals);
		}

		private static IEnumerable<SynchronizationResult<T>> SynchronizeInternal<T, TKey>(
// ReSharper disable ParameterTypeCanBeEnumerable.Local
			[NotNull] this IEnumerable<T> previousSortedByKey,
			[NotNull] IEnumerable<T> newSortedByKey,
			// ReSharper restore ParameterTypeCanBeEnumerable.Local
			[NotNull] Func<T, TKey> getItemKeyValue,
			[NotNull] Func<T, T, bool> @equals) where TKey : IComparable
		{
			var previousEnumerator = previousSortedByKey.GetEnumerator();
			var newEnumerator = newSortedByKey.GetEnumerator();
			var havePrevious = previousEnumerator.MoveNext();
			var haveNew = newEnumerator.MoveNext();
			var previousItem = default(T);
			var previousItemKey = default(TKey);
			if (havePrevious)
			{
				previousItem = previousEnumerator.Current;
				previousItemKey = getItemKeyValue(previousItem);
			}
			var newItem = default(T);
			var newItemKey = default(TKey);
			if (haveNew)
			{
				newItem = newEnumerator.Current;
				newItemKey = getItemKeyValue(newItem);
			}
			while (havePrevious && haveNew)
			{
				var keyComparisonResult = previousItemKey.CompareTo(newItemKey);
				if (keyComparisonResult == 0)
				{
					// item key matched, check for data changes
					yield return new SynchronizationResult<T>(
						previousItem,
						newItem,
						@equals(previousItem, newItem)
							? SynchronizationStatus.Unchanged
							: SynchronizationStatus.Changed);

					havePrevious = previousEnumerator.MoveNext();
					if (havePrevious)
					{
						previousItem = previousEnumerator.Current;
						previousItemKey = getItemKeyValue(previousItem);
					}

					haveNew = newEnumerator.MoveNext();
					if (haveNew)
					{
						newItem = newEnumerator.Current;
						newItemKey = getItemKeyValue(newItem);
					}
					continue;
				}
				if (keyComparisonResult > 0)
				{
					// newItem was added
					yield return new SynchronizationResult<T>(default(T), newItem, SynchronizationStatus.Added);
					haveNew = newEnumerator.MoveNext();
					if (haveNew)
					{
						newItem = newEnumerator.Current;
						newItemKey = getItemKeyValue(newItem);
						continue;
					}
					break;
				}
				// previousItem was removed
				yield return new SynchronizationResult<T>(previousItem, default(T), SynchronizationStatus.Removed);
				havePrevious = previousEnumerator.MoveNext();
				if (havePrevious)
				{
					previousItem = previousEnumerator.Current;
					previousItemKey = getItemKeyValue(previousItem);
					continue;
				}
				break;
			}

			while (haveNew) // remainder of newSortedByKey were added
			{
				yield return new SynchronizationResult<T>(default(T), newEnumerator.Current, SynchronizationStatus.Added);
				haveNew = newEnumerator.MoveNext();
			}

			while (havePrevious) // remainder of previousSortedByKey were deleted
			{
				yield return new SynchronizationResult<T>(previousEnumerator.Current, default(T), SynchronizationStatus.Removed);
				havePrevious = previousEnumerator.MoveNext();
			}
		}

		/// <summary>
		///     synchronizes previousItems with newItems and returns the results.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="newItems">all current items, sorted by KeyValue</param>
		/// <param name="getItemKeyValue"> e.g. county=>county.Id</param>
		/// <param name="equals">e.g.  (county1,County2)=>county1.Name == county2.Name && county1.Gps == county2.Gps</param>
		/// <param name="previousItems">all previous items, sorted by KeyValue</param>
		public static IEnumerable<SynchronizationResult<T>> SynchronizeOrdered<T, TKey>(
			[NotNull] this IEnumerable<T> previousItems,
			[NotNull] IEnumerable<T> newItems,
			[NotNull] Func<T, TKey> getItemKeyValue,
			[NotNull] Func<T, T, bool> equals)
			where TKey : IComparable
		{
			return SynchronizeInternal(previousItems, newItems, getItemKeyValue, @equals);
		}

		[NotNull]
		public static HashSet<T> ToHashSet<T>([NotNull][ItemNotNull] this IEnumerable<T> items)
		{
			return new HashSet<T>(items);
		}

		[NotNull]
		public static IEnumerable<List<T>> ToPageSets<T>([CanBeNull] this IEnumerable<T> items, int firstPageSize, int nthPageSize)
		{
			if (items != null)
			{
				var toEnumerate = items.ToList();
				yield return new List<T>(toEnumerate.Take(firstPageSize));
				var remainder = toEnumerate.Skip(firstPageSize).ToList();
				if (remainder.Any())
				{
					foreach (var item in remainder.InSetsOf(nthPageSize))
					{
						yield return item;
					}
				}
			}
		}
	}

	public class SynchronizationResult<T>
	{
		public SynchronizationResult(T oldItem, T newItem, SynchronizationStatus status)
		{
			OldItem = oldItem;
			NewItem = newItem;
			Status = status;
		}

		public T NewItem { get; private set; }
		public T OldItem { get; private set; }
		public SynchronizationStatus Status { get; private set; }
	}

	public enum SynchronizationStatus
	{
		Unchanged = 0,
		Added = 10,
		Removed = 20,
		Changed = 30
	}

	public class Range<T>
	{
		public int End { get; set; }
		public T Payload { get; set; }
		public int Start { get; set; }
	}

	public class ContinuingEnumerator<T> : IEnumerable<T>
	{
		public ContinuingEnumerator([NotNull] IEnumerator<T> enumerator, [NotNull] Func<T, T, bool> keepGrouping, [CanBeNull] T current)
		{
			Current = current;
			_enumerator = enumerator;
			_keepGrouping = keepGrouping;
		}

		private readonly IEnumerator<T> _enumerator;
		private readonly Func<T, T, bool> _keepGrouping;

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

		public T Current { get; private set; }

		public bool HasNext { get; private set; }
	}
}