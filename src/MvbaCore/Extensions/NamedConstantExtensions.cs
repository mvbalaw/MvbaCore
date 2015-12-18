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
using System.Linq;

using JetBrains.Annotations;

using MvbaCore.CodeQuery;

namespace MvbaCore.Extensions
{
	public static class NamedConstantExtensions
	{
		private static readonly Dictionary<Type, object> Defaults = new Dictionary<Type, object>();
		private static readonly HashSet<Type> NoDefaults = new HashSet<Type>();

		[CanBeNull]
		[Pure]
		public static T DefaultValue<T>() where T : NamedConstant<T>
		{
			var type = typeof(T);
			lock (NoDefaults)
			{
				if (NoDefaults.Contains(type))
				{
					return null;
				}
			}
			object defaultValue;
			lock (Defaults)
			{
				if (Defaults.TryGetValue(type, out defaultValue))
				{
					return (T)defaultValue;
				}
			}
			var fields = type.GetFields().ThatAreStatic();
			var defaultField = fields.WithAttributeOfType<DefaultKeyAttribute>().FirstOrDefault();
			if (defaultField == null)
			{
				lock (NoDefaults)
				{
					if (!NoDefaults.Contains(type))
					{
						NoDefaults.Add(type);
					}
				}
				return null;
			}
			defaultValue = defaultField.GetValue(null);
			if (defaultValue == null)
			{
				lock (NoDefaults)
				{
					if (!NoDefaults.Contains(type))
					{
						NoDefaults.Add(type);
					}
				}
				return null;
			}
			lock (Defaults)
			{
				if (!Defaults.ContainsKey(type))
				{
					Defaults.Add(type, defaultValue);
				}
			}
			return (T)defaultValue;
		}

		[NotNull]
		[Pure]
		public static T OrDefault<T>([CanBeNull] this T value) where T : NamedConstant<T>
		{
			if (value != null)
			{
				return value;
			}
			var defaultValue = DefaultValue<T>();
			if (defaultValue == null)
			{
				throw new ArgumentException("No default value defined for Named Constant type " + typeof(T));
			}
			return defaultValue;
		}
	}
}