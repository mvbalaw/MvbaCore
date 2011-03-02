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
using System.Linq;

namespace MvbaCore.Lucene
{
	public interface ILuceneIndexer
	{
		void Close();
		void DeleteFromIndex<T>(T entity);
		bool IsIndexable<T>(T entity);
		void UpdateIndex<T>(T entity);
	}

	public class LuceneIndexer : ILuceneIndexer, IDisposable
	{
		private readonly IEntityIndexUpdater[] _indexUpdaters;
		private readonly ILuceneWriter _luceneWriter;
		private bool _disposed;

		public LuceneIndexer(ILuceneWriter luceneWriter, IEntityIndexUpdater[] indexUpdaters)
		{
			_luceneWriter = luceneWriter;
			_indexUpdaters = indexUpdaters;
		}

		public void Dispose()
		{
			Dispose(true);

			// Use SupressFinalize in case a subclass
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
		}

		public void UpdateIndex<T>(T entity)
		{
			var updater = GetUpdater(entity);
			if (updater == null)
			{
				return;
			}

			updater.UpdateIndex(entity, _luceneWriter);
		}

		public bool IsIndexable<T>(T entity)
		{
			return GetUpdater(entity) != null;
		}

		public void DeleteFromIndex<T>(T entity)
		{
			var updater = GetUpdater(entity);
			if (updater == null)
			{
				return;
			}

			updater.DeleteFromIndex(entity, _luceneWriter);
		}

		public void Close()
		{
			_luceneWriter.Close();
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_luceneWriter.Close();
				}
				_disposed = true;
			}
		}

		private IEntityIndexUpdater GetUpdater<T>(T entity)
		{
			return _indexUpdaters.FirstOrDefault(x => x.IsMatch(entity));
		}
	}
}