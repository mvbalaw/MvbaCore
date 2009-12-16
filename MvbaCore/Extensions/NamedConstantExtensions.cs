using System;
using System.Linq;

using CodeQuery;

using JetBrains.Annotations;

namespace MvbaCore.Extensions
{
	public static class NamedConstantExtensions
	{
		[NotNull]
		public static NamedConstant<T> ToNonNull<T>([CanBeNull] this NamedConstant<T> value) where T : class
		{
			if (value != null)
			{
				return value;
			}
			var fields = typeof(T).GetFields().ThatAreStatic();
			var defaultField = fields.WithAttributeOfType<DefaultKeyAttribute>().FirstOrDefault();
			if (defaultField == null)
			{
				throw new ArgumentException(typeof(T).Name + " must have a field marked with attribute [DefaultKey]");
			}
			return (NamedConstant<T>)defaultField.GetValue(null);
		}
	}
}