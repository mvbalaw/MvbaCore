namespace MvbaCore.Lucene
{
	public interface ILuceneIndexedField
	{
		bool IsSearchable { get; }
		bool IsUniqueKey { get; }
		string Name { get; }
	}
}