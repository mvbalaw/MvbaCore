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

namespace MvbaCore.Collections
{
	public class CachedEnumerable<T> : IEnumerable<T>
	{
		// original: http://wilsonhut.wordpress.com/tag/memoize/
		private readonly List<T> _cache = new List<T>();
		private readonly IEnumerable<T> _originalEnumerable;
		private IEnumerator<T> _originalEnumerator;

		public CachedEnumerable(IEnumerable<T> enumerable)
		{
			_originalEnumerable = enumerable;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int index = 0; index < _cache.Count; index++)
			{
				var t = _cache[index];
				yield return t;
			}
			if (_originalEnumerator == null)
			{
				_originalEnumerator = _originalEnumerable.GetEnumerator();
			}
			while (_originalEnumerator.MoveNext())
			{
				T item = _originalEnumerator.Current;
				_cache.Add(item);
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}