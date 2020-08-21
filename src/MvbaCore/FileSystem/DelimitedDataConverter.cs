using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace MvbaCore.FileSystem
{
	public abstract class DelimitedDataConverter
	{
		[ItemNotNull]
		[NotNull]
		protected IEnumerable<Dictionary<string, string>> Convert([NotNull] [ItemNotNull] IEnumerable<string> lines, [NotNull] string delimiter, bool joinQuoted = false)
		{
			Dictionary<int, string> headerRow = null;

			var count = 0;
			foreach (var line in lines)
			{
				count++;
				if (headerRow == null)
				{
					headerRow = GetHeaderRow(line, delimiter);
				}
				else
				{
					var strings = line.Split(new[] { delimiter }, StringSplitOptions.None);
					if (strings.Length != headerRow.Count)
					{
						throw new ApplicationException("the file has a problem on line " + count + ": found " + strings.Length + " fields, expected " + headerRow.Count + " -- " + line.Substring(0, Math.Min(30, line.Length)) + (line.Length > 30 ? "..." :""));
					}
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

		[Pure]
		[NotNull]
		private static Dictionary<int, string> GetHeaderRow([NotNull] string line, [NotNull] string delimiter)
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

		[Pure]
		[NotNull]
		[ItemNotNull]
		public static IEnumerable<string> JoinQuoted([NotNull] [ItemNotNull] string[] strings, [NotNull] string delimiter)
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