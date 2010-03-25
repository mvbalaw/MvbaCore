using System;
using System.Collections.Generic;

using MvbaCore.Extensions;

namespace MvbaCore
{
	[Serializable]
#pragma warning disable 661,660
	public class NamedConstant<T> : INamedConstant
#pragma warning restore 661,660
		where T : NamedConstant<T>
	{
		private static readonly Dictionary<string, T> NamedConstants = new Dictionary<string, T>();

		protected void Add(string key, T item)
		{
			NamedConstants.Add(key.ToLower(), item);
		}

		protected static IEnumerable<T> Values()
		{
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

		public string Key { get; protected set; }

		public static T GetFor(string key)
		{
			return Get(key).OrDefault();
		}
	}
}