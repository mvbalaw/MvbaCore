using System;
using System.Collections.Generic;
using System.Reflection;

using MvbaCore.Extensions;

namespace MvbaCore
{
	[Serializable]
	public class NamedConstant : INamedConstant
	{
		public string Key { get; protected set; }
	}

#pragma warning disable 661,660
	[Serializable]
	public class NamedConstant<T> : NamedConstant
#pragma warning restore 661,660
		where T : NamedConstant<T>
	{
		private static readonly Dictionary<string, T> NamedConstants = new Dictionary<string, T>();

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
			return Get(key).OrDefault();
		}

		private static void EnsureValues()
		{
			if (NamedConstants.Count == 0)
			{
				try
				{
					var fieldInfos = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
					// ensure its static members get created by triggering the type initializer
					fieldInfos[0].GetValue(null);
				}
				catch
				{
				}
			}
		}
	}
}