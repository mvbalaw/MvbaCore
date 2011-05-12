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
		private const int MaxResults = 100000;
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
			var analyzer = new SimpleAnalyzer();
			var fieldNames = _fields
				.Where(x => x.IsSearchable)
				.Select(x => x.Name)
				.ToArray();
			var parser = new MultiFieldQueryParser(Version.LUCENE_29,
												   fieldNames,
												   analyzer);
			string lowerQueryString = querystring.ToLower();

			parser.SetDefaultOperator(QueryParser.Operator.AND);
			try
			{
				var fullQuery = parser.Parse(lowerQueryString);
				var rewrittenQueryString = fullQuery.ToString();
				var clauses = rewrittenQueryString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

				string luceneDirectory = _luceneFileSystem.GetLuceneDirectory();

				var fsDirectory = FSDirectory.Open(new DirectoryInfo(luceneDirectory));
				var indexSearcher = new IndexSearcher(fsDirectory, true);


				if (clauses.Length <= 1)
				{
					return FindClauseMatches(fullQuery, indexSearcher);
				}

				// force it to be an 'and' search by keeping only those uniqueIDs
				// that show up in the search result for each independent clause
				parser.SetDefaultOperator(QueryParser.Operator.OR);

				var searchResultList = new List<LuceneSearchResult>();
				foreach (var queryPart in clauses)
				{
					var query = parser.Parse(queryPart);
					var partialResult = FindClauseMatches(query, indexSearcher);
					if (!partialResult.Any())
					{
						// nothing matched this clause. since we are aggregating an AND
						// result across multiple documents, we're done.
						return partialResult;
					}
					searchResultList.AddRange(partialResult);
				}
				var mergedResult = searchResultList
					.GroupBy(x => x.UniqueId)
					.Where(x => x.Count() == clauses.Count())
					.Select(x => x.First())
					.ToList();
				return mergedResult;
			}
			catch (ParseException)
			{
				return new List<LuceneSearchResult>();
			}
		}

		private IList<LuceneSearchResult> FindClauseMatches(Query fullQuery, Searcher indexSearcher)
		{

			try
			{
				var collector = TopScoreDocCollector.create(MaxHits, false);
				indexSearcher.Search(fullQuery, collector);
				var hits = collector.TopDocs();
				if (hits.totalHits == 0)
				{
					return new List<LuceneSearchResult>();
				}

				string uniqueKey = _fields.First(x => x.IsUniqueKey).Name;

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