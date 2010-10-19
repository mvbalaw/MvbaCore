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

				var queryWords = new HashSet<string>();

				foreach (string key in lowerQueryString
					.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
				{
					int index = key.IndexOf(':');
					queryWords.Add(index != -1 ? key.Substring(index + 1) : key);
				}

				var result = mergedResults
					.Where(x => x.GetMatchCount(queryWords) == queryWords.Count)
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