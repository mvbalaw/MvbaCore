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
using System.Text.RegularExpressions;

using Lucene.Net.Documents;

namespace MvbaCore.Lucene
{
	public class LuceneSearchResult
	{
		private readonly Dictionary<string, string> _matches = new Dictionary<string, string>();

		private int? _matchCount;

		public LuceneSearchResult(string uniqueId, IEnumerable<Document> documents)
		{
			UniqueId = uniqueId;
			foreach (var doc in documents)
			{
				foreach (Field field in doc.GetFields())
				{
					string stringValue = field.StringValue();
					if (String.IsNullOrEmpty(stringValue))
					{
						continue;
					}
					string value;
					string key = field.Name();
					if (!_matches.TryGetValue(key, out value))
					{
						_matches.Add(key, stringValue);
					}
					else
					{
						_matches[key] = value + Environment.NewLine + stringValue;
					}
				}
			}
		}

		public string UniqueId { get; private set; }

		public int GetMatchCount(IEnumerable<string> queryWords)
		{
			if (_matchCount != null)
			{
				return _matchCount.Value;
			}
			int count = (from queryWord in queryWords
			             from value in _matches.Values
			             where Regex.IsMatch(value, ".*"+queryWord.Replace("*",".*")+".*", RegexOptions.IgnoreCase)
			             select queryWord).Count();

			_matchCount = count;
			return count;
		}
	}
}