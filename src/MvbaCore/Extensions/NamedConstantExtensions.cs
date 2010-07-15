using System.Linq;

using CodeQuery;

using JetBrains.Annotations;

namespace MvbaCore.Extensions
{
	public static class NamedConstantExtensions
	{
		[CanBeNull]
		public static T DefaultValue<T>() where T : NamedConstant<T>
		{
			var fields = typeof(T).GetFields().ThatAreStatic();
			var defaultField = fields.WithAttributeOfType<DefaultKeyAttribute>().FirstOrDefault();
			if (defaultField == null)
			{
				return null;
			}
// ReSharper disable AssignNullToNotNullAttribute
			return (T)defaultField.GetValue(null);
// ReSharper restore AssignNullToNotNullAttribute
		}

		[CanBeNull]
		public static T OrDefault<T>([CanBeNull] this T value) where T : NamedConstant<T>
		{
			if (value != null)
			{
				return value;
			}
			return DefaultValue<T>();
		}
	}
}