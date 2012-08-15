using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MvbaCore.Extensions;
using Version = Lucene.Net.Util.Version;

namespace MvbaCore.Lucene
{
	public class QueryType : NamedConstant<QueryType>
	{
		[DefaultKey] public static readonly QueryType Intersection = new QueryType("intersection", "Intersection",
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

		public IList<LuceneSearchResult> FindUnion(string querystring)
		{
			var analyzer = new StandardAnalyzer(Version.LUCENE_29, new Hashtable());
			var fieldNames = _fields
				.Where(x => x.IsSearchable)
				.Where(x => !x.IsSystemDescriminator)
				.Where(
					x => querystring.Contains(String.Format("{0}:", x.Name)) || querystring.Contains(String.Format("{0} :", x.Name)))
				.Select(x => x.Name)
				.ToArray();
			var parser = new QueryParser(Version.LUCENE_29,
			                             fieldNames[0],
			                             analyzer);

			parser.SetDefaultOperator(QueryParser.Operator.OR);
			try
			{
				string escaped = ReplaceDashesWithSpecialString(querystring.ToLower(), true);

				var fullQuery = parser.Parse(escaped);
				string luceneDirectory = _luceneFileSystem.GetLuceneDirectory();

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

		public IList<LuceneSearchResult> FindIntersection(string querystring)
		{
			var analyzer = new StandardAnalyzer(Version.LUCENE_29, new Hashtable());
			var fieldNames = _fields
				.Where(x => x.IsSearchable)
				.Where(x => !x.IsSystemDescriminator)
				.Select(x => x.Name)
				.ToArray();
			var systemDescriminator = _fields.FirstOrDefault(x => x.IsSystemDescriminator);
			var parser = new MultiFieldQueryParser(Version.LUCENE_29,
			                                       fieldNames,
			                                       analyzer);
			string lowerQueryString = querystring.ToLower();

			parser.SetDefaultOperator(QueryParser.Operator.AND);
			try
			{
				string escaped = ReplaceDashesWithSpecialString(lowerQueryString, true);
				var fullQuery = parser.Parse(escaped);
				string rewrittenQueryString = fullQuery.ToString();
				var clauses = rewrittenQueryString.Split(new[] {'+'}, StringSplitOptions.RemoveEmptyEntries);

				string luceneDirectory = _luceneFileSystem.GetLuceneDirectory();

				var fsDirectory = FSDirectory.Open(new DirectoryInfo(luceneDirectory));
				var indexSearcher = new IndexSearcher(fsDirectory, true);

				string descriminatorClause = "";
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
				parser.SetDefaultOperator(QueryParser.Operator.OR);

				var searchResultList = new List<LuceneSearchResult>();
				foreach (string queryPart in clauses.Except(new[] {descriminatorClause}))
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
				int expectedMatchingClauses = clauses.Length;
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

				int count = Math.Min(hits.totalHits, MaxHits);
				var mergedResults = Enumerable.Range(0, count)
					.Select(x => indexSearcher.Doc(hits.scoreDocs[x].doc))
					.GroupBy(x =>
					         	{
					         		var field = _fields
					         			.Where(y => y.IsUniqueKey)
					         			.Select(y => x.GetField(y.Name))
					         			.FirstOrDefault(y => y != null);
					         		return field == null ? "" : field.StringValue();
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

		public static string ReplaceDashesWithSpecialString(string input, bool ignoreLeadingDash)
		{
			const string replacement = @"dash";

			var parts = input.Split(' ');
			for (int i = 0; i < parts.Length; i++)
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