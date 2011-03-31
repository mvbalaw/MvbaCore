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
using System.IO;
using System.Linq;

using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

using Version = Lucene.Net.Util.Version;

namespace MvbaCore.Lucene
{
	public interface ILuceneSearcher
	{
		IList<LuceneSearchResult> FindMatches(string querystring);
	}

	public class LuceneSearcher : ILuceneSearcher
	{
		private const int MaxHits = 100000;
		private const int MaxResults = 100;
		private readonly ILuceneIndexedField[] _fields;
		private readonly ILuceneFileSystem _luceneFileSystem;

		public LuceneSearcher(
			ILuceneFileSystem luceneFileSystem,
			ILuceneIndexedField[] luceneIndexedFields)
		{
			_fields = luceneIndexedFields;
			_luceneFileSystem = luceneFileSystem;
		}

		public IList<LuceneSearchResult> FindMatches(string querystring)
		{
			var analyzer = new KeywordAnalyzer();
			var fieldNames = _fields
				.Where(x => x.IsSearchable)
				.Select(x => x.Name)
				.ToArray();
			var parser = new MultiFieldQueryParser(Version.LUCENE_29,
			                                       fieldNames,
			                                       analyzer);
			string lowerQueryString = querystring.ToLower();

			try
			{
				var query = parser.Parse(lowerQueryString);
				string luceneDirectory = _luceneFileSystem.GetLuceneDirectory();

				var fsDirectory = FSDirectory.Open(new DirectoryInfo(luceneDirectory));
				var indexSearcher = new IndexSearcher(fsDirectory, true);
				var collector = TopScoreDocCollector.create(MaxHits, false);
				indexSearcher.Search(query, collector);
				var hits = collector.TopDocs();
				if (hits.totalHits == 0)
				{
					return new List<LuceneSearchResult>();
				}

				string uniqueKey = _fields.FirstOrDefault(x => x.IsUniqueKey).Name;

				int count = Math.Min(hits.totalHits, MaxHits);
				var mergedResults = Enumerable.Range(0, count)
					.Select(x => indexSearcher.Doc(hits.scoreDocs[x].doc))
					.GroupBy(x => x.GetField(uniqueKey).StringValue())
					.OrderByDescending(x => x.Count())
					.Select(x => new LuceneSearchResult(x.Key, x));

				var result = mergedResults
					.Take(MaxResults)
					.ToList();
				return result;
			}
			catch (ParseException)
			{
				return new List<LuceneSearchResult>();
			}
		}
	}
}