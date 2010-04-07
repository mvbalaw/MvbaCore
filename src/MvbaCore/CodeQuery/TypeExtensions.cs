using System;

namespace CodeQuery
{
	public static class TypeExtensions
	{
		public static bool IsGenericAssignableFrom(this Type target, Type source)
		{
			if (!target.IsGenericType)
			{
				return false;
			}

			if (target.IsAssignableFrom(source))
			{
				return true;
			}

			if (!typeof(Nullable<>).IsAssignableFrom(target))
			{
				return false;
			}

			var genericParameters = target.GetGenericArguments();

			return genericParameters[0].IsAssignableFrom(source);
		}
	}
}