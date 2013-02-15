//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.Collections.Generic;
using System.Linq;

namespace MvbaCore.FileSystem
{
	public interface IQuotedCommaDelimitedDataConverter
	{
		IEnumerable<Dictionary<string, string>> Convert(IEnumerable<string> lines);
	}

	public class QuotedCommaDelimitedDataConverter : DelimitedDataConverter, IQuotedCommaDelimitedDataConverter
	{
		public IEnumerable<Dictionary<string, string>> Convert(IEnumerable<string> lines)
		{
			var result = Convert(lines.Select(x => x.Substring(1, x.Length - 2)), "\",\"");
			return result;
		}
	}
}