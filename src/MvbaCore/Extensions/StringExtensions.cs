//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************
using JetBrains.Annotations;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string input)
		{
			return input.IsNullOrEmpty(false);
		}

		public static bool IsNullOrEmpty(this string input, bool trim)
		{
			if (String.IsNullOrEmpty(input))
			{
				return true;
			}
			return trim && String.IsNullOrEmpty(input.Trim());
		}

		public static string ToCamelCase([CanBeNull] this string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				return str;
			}
			str = Char.ToLower(str[0]) + str.Substring(1);
			return str;
		}

		public static string ToNonNull(this string input)
		{
			return input ?? "";
		}
	}
}