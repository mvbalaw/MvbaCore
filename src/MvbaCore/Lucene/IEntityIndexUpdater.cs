namespace MvbaCore.Lucene
{
	public interface IEntityIndexUpdater
	{
		void DeleteFromIndex<T>(T entity, ILuceneWriter luceneWriter);
		bool IsMatch<T>(T entity);
		void UpdateIndex<T>(T entity, ILuceneWriter luceneWriter);
	}
}