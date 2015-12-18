//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace MvbaCore.Collections
{
	public class LruCache<TKey, TValue>
		where TValue : class
	{
		private readonly int _capacity;
		private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _lookup;
		private readonly LinkedList<KeyValuePair<TKey, TValue>> _orderedItems;

		public LruCache(int capacity)
		{
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException("capacity", "Capacity must be at least 1");
			}
			_lookup = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity);
			_orderedItems = new LinkedList<KeyValuePair<TKey, TValue>>();
			_capacity = capacity;
		}

		[CanBeNull]
		public TValue this[TKey key]
		{
			get
			{
				LinkedListNode<KeyValuePair<TKey, TValue>> node;
				if (!_lookup.TryGetValue(key, out node))
				{
					return null;
				}
				lock (_orderedItems)
				{
					if (!ReferenceEquals(_orderedItems.Last, node))
					{
						_orderedItems.Remove(node);
						_orderedItems.AddLast(node);
					}
				}
				return node.Value.Value;
			}
		}

		public void Add([NotNull] TKey key, TValue value)
		{
			if (Contains(key))
			{
				return;
			}
			lock (_orderedItems)
			{
				if (Contains(key))
				{
					return;
				}
				while (_lookup.Count >= _capacity)
				{
					_lookup.Remove(_orderedItems.First.Value.Key);
					_orderedItems.RemoveFirst();
				}
				var node = new KeyValuePair<TKey, TValue>(key, value);
				_lookup.Add(key, _orderedItems.AddLast(node));
			}
		}

		[Pure]
		public bool Contains([NotNull] TKey key)
		{
			return _lookup.ContainsKey(key);
		}
	}
}