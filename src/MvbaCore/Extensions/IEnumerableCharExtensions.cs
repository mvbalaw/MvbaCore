//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************
// ReSharper disable CheckNamespace
namespace System.Collections.Generic
// ReSharper restore CheckNamespace
{
	public static class IEnumerableCharExtensions
	{
		public static string AsString(this IEnumerable<char> items, string separator = "")
		{
			if (items == null)
			{
				return String.Empty;
			}
			if (separator == null)
			{
				throw new ArgumentNullException("separator", "The character separator cannot be null.");
			}
			return String.Join(separator, items);
		}
	}
}