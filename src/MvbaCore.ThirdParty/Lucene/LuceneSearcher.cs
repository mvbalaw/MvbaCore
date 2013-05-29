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
using System.Text;

using JetBrains.Annotations;

using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

using MvbaCore.Extensions;

using Version = Lucene.Net.Util.Version;

namespace MvbaCore.ThirdParty.Lucene
{
	public class QueryType : NamedConstant<QueryType>
	{
		[DefaultKey]
		public static readonly QueryType Intersection = new QueryType("intersection", "Intersection",
		                                                              (searcher, queryString) =>
		                                                              searcher.FindIntersection(queryString));

		public static readonly QueryType Union = new QueryType("union", "Union",
		                                                       (searcher, queryString) => searcher.FindUnion(queryString));

		private QueryType(string key, string description, Func<LuceneSearcher, string, IList<LuceneSearchResult>> findMatches)
		{
			Description = description;
			FindMatches = findMatches;
			Add(key, this);
		}

		public string Description { get; private set; }
		public Func<LuceneSearcher, string, IList<LuceneSearchResult>> FindMatches { get; private set; }
	}

	public interface ILuceneSearcher
	{
		IList<LuceneSearchResult> FindMatches(string querystring, string queryType);
	}

	[UsedImplicitly]
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

		public IList<LuceneSearchResult> FindMatches(string querystring, string queryType)
		{
			return QueryType.GetFor(queryType).OrDefault().FindMatches(this, querystring);
		}

		private IList<LuceneSearchResult> FindClauseMatches(Query fullQuery, Searcher indexSearcher)
		{
			try
			{
				var collector = TopScoreDocCollector.Create(MaxHits, false);
				indexSearcher.Search(fullQuery, collector);
				var hits = collector.TopDocs();
				if (hits.TotalHits == 0)
				{
					return new List<LuceneSearchResult>();
				}

				var count = Math.Min(hits.TotalHits, MaxHits);
				var mergedResults = Enumerable.Range(0, count)
					.Select(x => indexSearcher.Doc(hits.ScoreDocs[x].Doc))
					.GroupBy(x =>
						{
							var field = _fields
								.Where(y => y.IsUniqueKey)
								.Select(y => x.GetField(y.Name))
								.FirstOrDefault(y => y != null);
							return field == null ? "" : field.StringValue;
						})
					.Where(x => x.Key != "")
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

		public IList<LuceneSearchResult> FindIntersection(string querystring)
		{
			var analyzer = new StandardAnalyzer(Version.LUCENE_30);
			var fieldNames = _fields
				.Where(x => x.IsSearchable)
				.Where(x => !x.IsSystemDescriminator)
				.Select(x => x.Name)
				.ToArray();
			var systemDescriminator = _fields.FirstOrDefault(x => x.IsSystemDescriminator);
			var parser = new MultiFieldQueryParser(Version.LUCENE_30,
			                                       fieldNames,
			                                       analyzer);
			var lowerQueryString = querystring.ToLower();

			parser.DefaultOperator = QueryParser.Operator.AND;
			try
			{
				var escaped = ReplaceDashesWithSpecialString(lowerQueryString, true);
				escaped = EscapeLeadingWildcards(escaped);
				var fullQuery = parser.Parse(escaped);
				var rewrittenQueryString = fullQuery.ToString();
				var clauses = rewrittenQueryString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

				var luceneDirectory = _luceneFileSystem.GetLuceneDirectory();

				var fsDirectory = FSDirectory.Open(new DirectoryInfo(luceneDirectory));
				var indexSearcher = new IndexSearcher(fsDirectory, true);

				var descriminatorClause = "";
				if (systemDescriminator != null)
				{
					descriminatorClause = clauses.FirstOrDefault(y => y.StartsWith(systemDescriminator.Name + ":")) ?? "";
				}

				if (clauses.Length <= 1 || descriminatorClause.Length > 0 && clauses.Length == 2)
				{
					return FindClauseMatches(fullQuery, indexSearcher);
				}

				// force it to be an 'and' search by keeping only those uniqueIDs
				// that show up in the search result for each independent clause
				parser.DefaultOperator = QueryParser.Operator.OR;

				var searchResultList = new List<LuceneSearchResult>();
				foreach (var queryPart in clauses.Except(new[] { descriminatorClause }))
				{
					var query = descriminatorClause.Length > 0
						            ? parser.Parse("+" + queryPart + " +" + descriminatorClause)
						            : parser.Parse(queryPart);
					var partialResult = FindClauseMatches(query, indexSearcher);
					if (!partialResult.Any())
					{
						// nothing matched this clause. since we are aggregating an AND
						// result across multiple documents, we're done.
						return partialResult;
					}
					searchResultList.AddRange(partialResult);
				}
				var expectedMatchingClauses = clauses.Length;
				if (descriminatorClause.Length > 0)
				{
					expectedMatchingClauses--;
				}
				var mergedResult = searchResultList
					.GroupBy(x => x.UniqueId)
					.Where(x => x.Count() == expectedMatchingClauses)
					.Select(x => x.First())
					.ToList();
				return mergedResult;
			}
			catch (ParseException)
			{
				return new List<LuceneSearchResult>();
			}
		}

		public static string EscapeLeadingWildcards(string input)
		{
			if (input.Length == 0)
			{
				return input;
			}
			var escaped = new StringBuilder();

			var parts = input.Split(new[] { ' ' });
			if (parts.Length == 1)
			{
				return input;
			}
			bool gatheringTerm = false;
			foreach(var part in parts)
			{
				escaped.Append(' ');
				if (gatheringTerm)
				{
					if (part.Contains("\""))
					{
						gatheringTerm = false;
					}
				}
				else if (part.Contains("\""))
				{
					if (part.IndexOf('\"') != part.LastIndexOf('\"'))
					{
						gatheringTerm = true;
					}
				}

				if (part.Contains(":"))
				{
					if (part.Contains(":*"))
					{
						escaped.Append(part.Replace(":*", ":" + LuceneConstants.WildcardEndsWithSearchEnabler + "*"));
						continue;
					}
					if (part.Contains(":\"*"))
					{
						escaped.Append(part.Replace(":\"*", ":\""));
						continue;
					}
				}
				else if (part.StartsWith("\"*"))
				{
					escaped.Append(part.Replace("\"*", "\""));
					continue;
				}
				else if (part.StartsWith("*"))
				{
					escaped.Append(LuceneConstants.WildcardEndsWithSearchEnabler);
				}
				escaped.Append(part);
			}

			return escaped.ToString().TrimStart();
		}

		public IList<LuceneSearchResult> FindUnion(string querystring)
		{
			var analyzer = new StandardAnalyzer(Version.LUCENE_30);
			var fieldNames = _fields
				.Where(x => x.IsSearchable)
				.Where(x => !x.IsSystemDescriminator)
				.Where(
					x => querystring.Contains(String.Format("{0}:", x.Name)) || querystring.Contains(String.Format("{0} :", x.Name)))
				.Select(x => x.Name)
				.ToArray();
			var parser = new QueryParser(Version.LUCENE_30,
			                             fieldNames[0],
			                             analyzer)
			             {
				             DefaultOperator = QueryParser.Operator.OR
			             };

			try
			{
				var escaped = ReplaceDashesWithSpecialString(querystring.ToLower(), true);
				escaped = EscapeLeadingWildcards(escaped);
				var fullQuery = parser.Parse(escaped);
				var luceneDirectory = _luceneFileSystem.GetLuceneDirectory();

				var fsDirectory = FSDirectory.Open(new DirectoryInfo(luceneDirectory));
				var indexSearcher = new IndexSearcher(fsDirectory, true);

				var searchResultList = FindClauseMatches(fullQuery, indexSearcher);

				return searchResultList;
			}
			catch (ParseException)
			{
				return new List<LuceneSearchResult>();
			}
		}

		public static string ReplaceDashesWithSpecialString(string input, bool ignoreLeadingDash)
		{
			const string replacement = @"dash";

			var parts = input.Split(' ');
			for (var i = 0; i < parts.Length; i++)
			{
				if (parts[i].Length < 2)
				{
					continue;
				}

				if (ignoreLeadingDash)
				{
					parts[i] = parts[i].Substring(0, 1) + parts[i].Substring(1).Replace("-", replacement);
				}
				else
				{
					parts[i] = parts[i].Replace("-", replacement);
				}
			}
			return String.Join(" ", parts);
		}
	}
}