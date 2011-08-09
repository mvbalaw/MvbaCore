using System;
using System.Collections.Generic;

namespace MvbaCore
{
	public class MergeSorter<T>
	{
		public IEnumerable<T> Merge(IEnumerable<T> list1, IEnumerable<T> list2, Func<T, T, int> compare)
		{
			var list1Pointer = list1.GetEnumerator();
			var list2Pointer = list2.GetEnumerator();
			bool list1HasItems = list1Pointer.MoveNext();
			bool list2HasItems = list2Pointer.MoveNext();

			while (list1HasItems || list2HasItems)
			{
				T list2Item;
				if (!list1HasItems)
				{
					do
					{
						list2Item = list2Pointer.Current;
						yield return list2Item;
					} while (list2Pointer.MoveNext());
					yield break;
				}
				T list1Item;
				if (!list2HasItems)
				{
					do
					{
						list1Item = list1Pointer.Current;
						yield return list1Item;
					} while (list1Pointer.MoveNext());
					yield break;
				}

				list2Item = list2Pointer.Current;
				list1Item = list1Pointer.Current;

				int comparisonResult = compare(list1Item, list2Item);
				if (comparisonResult <= 0)
				{
					yield return list1Item;
					list1HasItems = list1Pointer.MoveNext();
					continue;
				}

				yield return list2Item;
				list2HasItems = list2Pointer.MoveNext();
			}
		}
	}
}