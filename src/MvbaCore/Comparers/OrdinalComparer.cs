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
using System.Text.RegularExpressions;

namespace MvbaCore.Comparers
{
	public class OrdinalNumberComparer : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			int xNumber = Regex.Match(x, @"[^\d]*(\d+)", RegexOptions.Compiled).Groups[1].Value.SafeParseInt32() ?? 0;
			int yNumber = Regex.Match(y, @"[^\d]*(\d+)", RegexOptions.Compiled).Groups[1].Value.SafeParseInt32() ?? 0;
			return xNumber.CompareTo(yNumber);
		}
	}
}