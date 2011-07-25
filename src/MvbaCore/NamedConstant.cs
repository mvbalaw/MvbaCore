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
using System.Reflection;
using JetBrains.Annotations;
using MvbaCore.Extensions;

namespace MvbaCore
{

	[Serializable]
	public class NamedConstant : INamedConstant
	{
		/// <summary>
		/// Use Add to set
		/// </summary>
		public string Key { get; internal set; }
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

		protected void Add(string key, T item)
		{
			Key = key;
			NamedConstants.Add(key.ToLower(), item);
		}

		[Obsolete("Use .GetAll()")]
		protected static IEnumerable<T> Values()
		{
			return GetAll();
		}

		public static IEnumerable<T> GetAll()
		{
			EnsureValues();
			return NamedConstants.Values;
		}

		protected static T Get(string key)
		{
			if (key == null)
			{
				return null;
			}
			T t;
			NamedConstants.TryGetValue(key.ToLower(), out t);
			return t;
		}

		public static bool operator ==(NamedConstant<T> a, NamedConstant<T> b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(NamedConstant<T> a, NamedConstant<T> b)
		{
			return !(a == b);
		}

		public static T GetFor(string key)
		{
			EnsureValues();
			return Get(key);
		}

		public static T GetDefault()
		{
			EnsureValues();
			return NamedConstantExtensions.DefaultValue<T>();
		}

		private static void EnsureValues()
		{
			if (NamedConstants.Count == 0)
			{
				var fieldInfos = typeof (T).GetFields(BindingFlags.Static | BindingFlags.Public);
				// ensure its static members get created by triggering the type initializer
				fieldInfos[0].GetValue(null);
			}

		}
	}
}