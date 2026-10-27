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
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

using MvbaCore.Extensions;

namespace MvbaCore
{
	[Serializable]
	public class NamedConstant : INamedConstant
	{
		/// <summary>
		///   Use Add to set
		/// </summary>
		// ReSharper disable once NotNullMemberIsNotInitialized
		public string Key { get; internal set; }

		public override string ToString()
		{
			return Key;
		}
	}

#pragma warning disable 661,660
	[Serializable]
	public class NamedConstant<T> : NamedConstant
#pragma warning restore 661,660
		where T : NamedConstant<T>
	{
// ReSharper disable StaticFieldInGenericType
		private static readonly Dictionary<string, T> NamedConstants = new Dictionary<string, T>();
// ReSharper restore StaticFieldInGenericType

		protected void Add([NotNull] string key, [NotNull] T item)
		{
			Key = key;
			var storageKey = key.ToLower();
			if (NamedConstants.ContainsKey(storageKey))
			{
				if (ReferenceEquals(NamedConstants[storageKey], item))
				{
					return;
				}
				throw new ArgumentException("NamedConstant storage key '"+storageKey+"' is assigned to "+NamedConstants[storageKey].Key+" - cannot assign it to "+key);
			}
			NamedConstants.Add(storageKey, item);
		}

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
		[NotNull]
		[Pure]
		public static IEnumerable<T> GetAll()
// ReSharper restore UnusedMember.Global
// ReSharper restore MemberCanBePrivate.Global
		{
			EnsureValues();
			return NamedConstants.Values.Distinct();
		}

// ReSharper disable MemberCanBePrivate.Global
		[CanBeNull]
		[Pure]
		protected static T Get([CanBeNull] string key)
// ReSharper restore MemberCanBePrivate.Global
		{
			if (key == null)
			{
				return null;
			}
			EnsureValues();
			T t;
			NamedConstants.TryGetValue(key.ToLower(), out t);
			return t;
		}

		[Pure]
		public static bool operator ==([CanBeNull] NamedConstant<T> a, [CanBeNull] NamedConstant<T> b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			if ((object)a == null || (object)b == null)
			{
				return false;
			}

			return a.Equals(b);
		}

		[Pure]
		public static bool operator !=([CanBeNull] NamedConstant<T> a, [CanBeNull] NamedConstant<T> b)
		{
			return !(a == b);
		}

		[CanBeNull]
		[Pure]
		public static T GetFor([CanBeNull] string key)
		{
			return Get(key);
		}

		[CanBeNull]
		[Pure]
		public static T GetDefault()
		{
			EnsureValues();
			return NamedConstantExtensions.DefaultValue<T>();
		}

		private static void EnsureValues()
		{
			// the values are added by T's type initializer, so make sure it has finished. A non-zero Count is not
			// enough: it is also what another thread sees while that initializer is still adding values. The
			// runtime returns at once when the initializer has run, waits when another thread is running it, and
			// returns at once when this thread is the one running it.
			RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
		}
	}
}