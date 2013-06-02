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

namespace MvbaCore.Tests.Extensions
{
	public static class ListExtensions
	{
		public static List<T> GetRange<T>(this List<T> records, int? firstRow, int? count)
		{
			firstRow = firstRow ?? 0;
			count = count ?? records.Count;

			int recordsCount = records.Count - firstRow.Value;

			if (count > recordsCount)
			{
				count = recordsCount;
			}

			return records.GetRange(firstRow.Value, count.Value);
		}
	}
}