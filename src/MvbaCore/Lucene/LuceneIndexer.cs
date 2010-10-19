using System.Linq;

namespace MvbaCore.Lucene
{
	public interface ILuceneIndexer
	{
		void DeleteFromIndex<T>(T entity);
		void UpdateIndex<T>(T entity);
	}

	public class LuceneIndexer : ILuceneIndexer
	{
		private readonly IEntityIndexUpdater[] _indexUpdaters;
		private readonly ILuceneWriter _luceneWriter;

		public LuceneIndexer(ILuceneWriter luceneWriter, IEntityIndexUpdater[] indexUpdaters)
		{
			_luceneWriter = luceneWriter;
			_indexUpdaters = indexUpdaters;
		}

		public void UpdateIndex<T>(T entity)
		{
			var updater = _indexUpdaters.FirstOrDefault(x => x.IsMatch(entity));
			if (updater == null)
			{
				return;
			}

			try
			{
				updater.UpdateIndex(entity, _luceneWriter);
			}
			finally
			{
				_luceneWriter.Close();
			}
		}

		public void DeleteFromIndex<T>(T entity)
		{
			var updater = _indexUpdaters.FirstOrDefault(x => x.IsMatch(entity));
			if (updater == null)
			{
				return;
			}

			try
			{
				updater.DeleteFromIndex(entity, _luceneWriter);
			}
			finally
			{
				_luceneWriter.Close();
			}
		}
	}
}