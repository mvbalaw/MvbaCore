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
using System.Linq;

namespace MvbaCore.Collections
{
	// ReSharper disable PossibleInterfaceMemberAmbiguity
	public interface IPersistentList<T> : IList<T>, ICollection
		// ReSharper restore PossibleInterfaceMemberAmbiguity
		where T : class
	{
		IList<T> Actual { set; }
		Action<IList<T>> AfterAdd { get; set; }
		Action<IList<T>> AfterRemove { get; set; }
		Func<IList<T>, T, bool> BeforeAdd { get; set; }
		Func<IList<T>, T, bool> BeforeRemove { get; set; }
		IPersistentList<T> Sort(Func<T, T, int> compare);
	}

	public class PersistentList<T> : IPersistentList<T>
		where T : class
	{
		private IList<T> _actual;
		private Action<IList<T>> _afterAdd;
		private Action<IList<T>> _afterRemove;
		private Func<IList<T>, T, bool> _beforeAdd;
		private Func<IList<T>, T, bool> _beforeRemove;

		public PersistentList(IList<T> actual)
		{
			_actual = actual;
		}

		public IList<T> Actual
		{
			set { _actual = value; }
		}

		/// <summary>
		///     perform actions on one or more list items after an item is added.
		/// </summary>
		public Action<IList<T>> AfterAdd
		{
			get { return _afterAdd ?? (_afterAdd = l => { }); }
			set { _afterAdd = value; }
		}

		/// <summary>
		///     perform actions on one or more list items after an item is removed.
		/// </summary>
		public Action<IList<T>> AfterRemove
		{
			get { return _afterRemove ?? (_afterRemove = l => { }); }
			set { _afterRemove = value; }
		}

		/// <summary>
		///     perform a check on the item being added before adding it. Return true
		///     if it should be added, false if it should not be added.
		/// </summary>
		public Func<IList<T>, T, bool> BeforeAdd
		{
			get { return _beforeAdd ?? (_beforeAdd = (l, x) => true); }
			set { _beforeAdd = value; }
		}

		/// <summary>
		///     perform a check on the item being removed before removing it. Return true
		///     if it should be removed, false if it should not be removed.
		/// </summary>
		public Func<IList<T>, T, bool> BeforeRemove
		{
			get { return _beforeRemove ?? (_beforeRemove = (l, x) => true); }
			set { _beforeRemove = value; }
		}

		public IPersistentList<T> Sort(Func<T, T, int> compare)
		{
			int count = _actual.Count;
			if (count < 2)
			{
				return this;
			}
			// in-place insertion sort
			// http://en.wikipedia.org/wiki/Insertion_sort
			for (int i = 1; i < count; i++)
			{
				var current = _actual[i];
				int j = i - 1;
				for (; j >= 0; j--)
				{
					var item = _actual[j];
					if (compare(item, current) <= 0)
					{
						break;
					}
					_actual[j + 1] = item;
				}

				if (j != i - 1)
				{
					_actual[j + 1] = current;
				}
			}
			return this;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _actual.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			if (BeforeAdd(this, item))
			{
				_actual.Add(item);
				AfterAdd(this);
			}
		}

		public void Clear()
		{
			while (_actual.Any())
			{
				RemoveAt(0);
			}
		}

		public bool Contains(T item)
		{
			return _actual.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_actual.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			if (BeforeRemove(this, item))
			{
				bool toReturn = _actual.Remove(item);
				AfterRemove(this);
				return toReturn;
			}
			return true;
		}

		public int Count
		{
			get { return _actual.Count; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			var copy = new T[_actual.Count];
			_actual.CopyTo(copy, 0);
			Array.Copy(copy, 0, array, index, _actual.Count);
		}

		object ICollection.SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		bool ICollection.IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { return _actual.IsReadOnly; }
		}

		public int IndexOf(T item)
		{
			return _actual.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (BeforeAdd(this, item))
			{
				_actual.Insert(index, item);
				AfterAdd(this);
			}
		}

		public void RemoveAt(int index)
		{
			if (BeforeRemove(this, _actual[index]))
			{
				_actual.RemoveAt(index);
				AfterRemove(this);
			}
		}

		public T this[int index]
		{
			get { return _actual[index]; }
			set
			{
				// this is problematic because BeforeAdd has to act on the new item
				// and the existing list MINUS the item at index. 
				var copyWithoutItemAtIndex = new List<T>(_actual);
				copyWithoutItemAtIndex.RemoveAt(index);

				if (BeforeAdd(copyWithoutItemAtIndex, value))
				{
					_actual[index] = value;
					AfterAdd(this);
				}
			}
		}
	}
}