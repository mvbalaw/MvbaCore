//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MvbaCore
{
	public class MergeSorter<T>
	{
		[ItemNotNull]
		[NotNull]
		public IEnumerable<T> Merge([NotNull][ItemNotNull] IEnumerable<T> list1, [NotNull][ItemNotNull] IEnumerable<T> list2, [NotNull] Func<T, T, int> compare)
		{
			var list1Pointer = list1.GetEnumerator();
			var list2Pointer = list2.GetEnumerator();
			var list1HasItems = list1Pointer.MoveNext();
			var list2HasItems = list2Pointer.MoveNext();

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

				var comparisonResult = compare(list1Item, list2Item);
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