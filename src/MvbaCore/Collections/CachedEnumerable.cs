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
using System.IO;

using JetBrains.Annotations;

namespace MvbaCore.Collections
{
	public class CachedEnumerable<T> : IEnumerable<T>
	{
		// original: http://wilsonhut.wordpress.com/tag/memoize/
		private readonly List<T> _cache;
		private readonly int? _max;
		private readonly IEnumerable<T> _originalEnumerable;
		private IEnumerator<T> _originalEnumerator;
		private bool _readBeyondCache;

		public CachedEnumerable([NotNull] IEnumerable<T> enumerable, int? max)
		{
			_originalEnumerable = enumerable;
			_max = max;
			_cache = max != null ? new List<T>(max.Value) : new List<T>();
		}

		[ItemCanBeNull]
		public IEnumerator<T> GetEnumerator()
		{
// ReSharper disable ForCanBeConvertedToForeach
			for (var index = 0; index < _cache.Count; index++)
// ReSharper restore ForCanBeConvertedToForeach
			{
				var t = _cache[index];
				yield return t;
			}
			if (_originalEnumerator == null)
			{
				_originalEnumerator = _originalEnumerable.GetEnumerator();
			}
			if (_readBeyondCache)
			{
				throw new InternalBufferOverflowException("Cannot re-stream unmemoized data.");
			}
			if (_max == null)
			{
				while (_originalEnumerator.MoveNext())
				{
					var item = _originalEnumerator.Current;
					_cache.Add(item);
					yield return item;
				}
			}
			else
			{
				for (var i = 0; i < _max.Value; i++)
				{
					if (!_originalEnumerator.MoveNext())
					{
						yield break;
					}
					var item = _originalEnumerator.Current;
					_cache.Add(item);
					yield return item;
				}

				if (_originalEnumerator.MoveNext())
				{
					_readBeyondCache = true;
					var item = _originalEnumerator.Current;
					yield return item;

					while (_originalEnumerator.MoveNext())
					{
						item = _originalEnumerator.Current;
						yield return item;
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}