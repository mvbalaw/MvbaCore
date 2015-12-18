//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MvbaCore.Collections
{
	public interface IReadOnlyList<out T> : IEnumerable<T>
	{
		int Count { get; }
		T this[int index] { get; }
	}

	public class ReadOnlyList<T> : IReadOnlyList<T>
	{
		private readonly IList<T> _items;

		public ReadOnlyList(IList<T> items)
		{
			_items = items;
		}

		[Pure]
		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		[Pure]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		public int Count
		{
			get { return _items.Count; }
		}

		public T this[int index]
		{
			get { return _items[index]; }
		}
	}
}