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
using System.IO;
using System.Linq;

using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace MvbaCore.Lucene
{
	public interface ILuceneWriter
	{
		void AddDocument(Document document);
		void Close();
		void Commit();
		void DeleteDocuments(Term term);
	}

	public class LuceneWriter : ILuceneWriter, IDisposable
	{
		private readonly ILuceneFileSystem _luceneFileSystem;
		private IndexWriter _writer;

		public LuceneWriter(ILuceneFileSystem luceneFileSystem)
		{
			_luceneFileSystem = luceneFileSystem;
		}

		public void Dispose()
		{
			Close();
		}

		public void Close()
		{
			if (_writer != null)
			{
				_writer.Commit();
				_writer.Optimize();
				_writer.Close();
				_writer = null;
			}
		}

		public void DeleteDocuments(Term term)
		{
			Open();
			_writer.DeleteDocuments(term);
			Commit();
		}

		public void Commit()
		{
			if (_writer == null)
			{
				return;
			}
			_writer.Commit();
		}

		public void AddDocument(Document document)
		{
			Open();
			var fields = document.GetFields();
			foreach (var abstractField in fields)
			{
				if (abstractField is Field)
				{
					var field = (Field)abstractField;
					var value = LuceneSearcher.ReplaceDashesWithSpecialString(field.StringValue(), false);
					if (field.IsTokenized())
					{
						value = value + (" " + value).Replace(" ", " " + LuceneConstants.WildcardEndsWithSearchEnabler); 
					}
					field.SetValue(value);
				}
			}
			_writer.AddDocument(document);
			Commit();
		}

		private void Open()
		{
			if (_writer != null)
			{
				return;
			}
			var analyzer = new StandardAnalyzer(Version.LUCENE_29);
			string luceneDirectory = _luceneFileSystem.GetLuceneDirectory();
			var fsDirectory = FSDirectory.Open(new DirectoryInfo(luceneDirectory));
			bool create = !Directory.GetFiles(luceneDirectory).Any();
			_writer = new IndexWriter(fsDirectory, analyzer, create, new IndexWriter.MaxFieldLength(1000));
		}
	}
}