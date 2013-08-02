//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.Linq;

// ReSharper disable CheckNamespace

namespace System
// ReSharper restore CheckNamespace
{
	public static class CharExtensions
	{
		private static readonly char[] Vowels = { 'a', 'e', 'i', 'o', 'u' };

		public static bool IsVowel(this char ch)
		{
			return Vowels.Contains(Char.ToLower(ch));
		}
	}
}