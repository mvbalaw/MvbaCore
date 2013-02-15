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
using System.Collections;
using System.Collections.Generic;

namespace MvbaCore.Comparers
{
	public class FuncComparer<T> : IComparer<T>, IComparer
	{
		private readonly Func<T, T, int> _compare;

		public FuncComparer(Func<T, T, int> compare)
		{
			_compare = compare;
		}

		public int Compare(object x, object y)
		{
			return _compare((T)x, (T)y);
		}

		public int Compare(T x, T y)
		{
			return _compare(x, y);
		}
	}
}