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
using System.Linq;

using JetBrains.Annotations;

namespace MvbaCore.Collections
{
	public class CollectionSynchronizer<T>
	{
		private readonly IEnumerable<T> _newState;
		private readonly IEnumerable<T> _previousState;
		private IEnumerable<T> _added = new List<T>();
		private IEnumerable<T> _removed = new List<T>();
		private IEnumerable<T> _unchanged = new List<T>();

		public CollectionSynchronizer([NotNull] IEnumerable<T> newState, [CanBeNull] IEnumerable<T> previousState)
		{
			if (newState == null)
			{
				throw new ArgumentNullException("newState", "list being synchronized cannot be null");
			}
			_newState = newState.ToList();
			_previousState = previousState == null ? null : previousState.ToList();
		}

		/// <summary>
		///     items in listA that are not in listB
		/// </summary>
		[NotNull]
		public IEnumerable<T> Added
		{
			get { return _added; }
		}

		/// <summary>
		///     items in listB that are not in listA
		/// </summary>
		[NotNull]
		public IEnumerable<T> Removed
		{
			get { return _removed; }
		}

		/// <summary>
		///     items in both listA and listB
		/// </summary>
		[NotNull]
		public IEnumerable<T> Unchanged
		{
			get { return _unchanged; }
		}

		/// <summary>
		///     synchronizes listA with listB.
		/// </summary>
		public void Synchronize()
		{
			_removed = _previousState.Except(_newState);
			_added = _newState.Except(_previousState);
			_unchanged = _newState.Intersect(_previousState);
		}

		/// <summary>
		///     synchronizes newState with previousState.
		/// </summary>
		/// <typeparam name = "TKey"></typeparam>
		/// <param name = "getComparisonKey"> e.g. county=>county.CountyId</param>
		/// <param name = "isNullOrEmptyComparer">e.g.  county=>county.CountyId &lt;= 0</param>
		public void Synchronize<TKey>(Func<T, TKey> getComparisonKey, Func<T, bool> isNullOrEmptyComparer)
		{
			var previousStateKeyLookup = _previousState.Where(x => !isNullOrEmptyComparer(x)).ToDictionary(getComparisonKey);
			var newStateKeyLookup = _newState.Where(x => !isNullOrEmptyComparer(x)).ToDictionary(getComparisonKey);

			_removed = previousStateKeyLookup.Where(x => !newStateKeyLookup.ContainsKey(x.Key)).Select(x => x.Value).ToList();
			var added = new List<T>();
			var unchanged = new List<T>();
			foreach (var newItem in newStateKeyLookup)
			{
				T previousItem;
				if (previousStateKeyLookup.TryGetValue(newItem.Key, out previousItem))
				{
					unchanged.Add(previousItem);
				}
				else
				{
					added.Add(newItem.Value);
				}
			}
			_added = added;
			_unchanged = unchanged;
		}
	}
}