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

namespace MvbaCore.FileSystem
{
	public abstract class DelimitedDataConverter
	{
		protected IEnumerable<Dictionary<string, string>> Convert(IEnumerable<string> lines, string delimiter, bool joinQuoted = false)
		{
			Dictionary<int, string> headerRow = null;

			foreach (var line in lines)
			{
				if (headerRow == null)
				{
					headerRow = GetHeaderRow(line, delimiter);
				}
				else
				{
					var strings = line.Split(new[] { delimiter }, StringSplitOptions.None);
					if (joinQuoted)
					{
						strings = JoinQuoted(strings, delimiter).ToArray();
					}
					var header = headerRow;
					var values = strings
						.Select((x, i) => new
						{
							Key = header[i],
							Value = x
						})
						.ToDictionary(x => x.Key, x => x.Value);
					yield return values;
				}
			}
		}

		private static Dictionary<int, string> GetHeaderRow(string line, string delimiter)
		{
			var headerRow = line
				.Split(new[] { delimiter }, StringSplitOptions.None)
				.Select((x, i) => new
				{
					Index = i,
					Key = x
				})
				.ToDictionary(x => x.Index, x => x.Key);
			return headerRow;
		}

// ReSharper disable once ParameterTypeCanBeEnumerable.Global
		public static IEnumerable<string> JoinQuoted(string[] strings, string delimiter)
		{
			var output = new List<string>();
			var combining = false;
			var combined = "";
			foreach (var item in strings)
			{
				var startDoubleQuote = item.StartsWith("\"\"");
				var startSingleQuote = !combining && item.StartsWith("\"") && (!startDoubleQuote || item.StartsWith("\"\"\""));
				var endDoubleQuote = item.EndsWith("\"\"");
				var endSingleQuote = item.EndsWith("\"") && (!endDoubleQuote || item.EndsWith("\"\"\""));

				if (combining)
				{
					combined += delimiter + item;
					if (endSingleQuote)
					{
						output.Add(combined.Substring(1, combined.Length - 2));
						combined = "";
						combining = false;
					}
					continue;
				}

				if (startSingleQuote)
				{
					if (endSingleQuote && item.Length > 1)
					{
						output.Add(item.Substring(1, item.Length - 2));
					}
					else
					{
						combining = true;
						combined = item;
					}
					continue;
				}

				if (startDoubleQuote && item.Length == 2)
				{
					output.Add("");
					continue;
				}
				output.Add(item);
			}
			return output.Select(x => x.Replace("\"\"", "\""));
		}
	}
}