using System.Collections.Generic;

using JetBrains.Annotations;

namespace MvbaCore
{
	public class LruCache<TKey, TValue>
		where TValue : class
	{
		private readonly int _capacity;
		private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _lookup;
		private readonly LinkedList<KeyValuePair<TKey, TValue>> _orderedItems;

		public LruCache(int capacity)
		{
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
				if (_orderedItems.First == node)
				{
					_orderedItems.RemoveFirst();
					_orderedItems.AddLast(node);
				}
				else if (_orderedItems.Last != node)
				{
					_orderedItems.Remove(node);
					_orderedItems.AddLast(node);
				}
				return node.Value.Value;
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (Contains(key))
			{
				return;
			}
			if (_lookup.Count >= _capacity)
			{
				_lookup.Remove(_orderedItems.First.Value.Key);
				_orderedItems.RemoveFirst();
			}
			var node = new KeyValuePair<TKey, TValue>(key, value);
			_lookup.Add(key, _orderedItems.AddLast(node));
		}

		public bool Contains(TKey key)
		{
			return _lookup.ContainsKey(key);
		}
	}
}