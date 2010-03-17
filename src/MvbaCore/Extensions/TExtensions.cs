using JetBrains.Annotations;

namespace System
{
	public static class TExtensions
	{
		[NotNull]
		public static T ToNonNull<T>([CanBeNull] this T input) where T : class, new()
		{
			return input ?? new T();
		}

		public static TDesiredType TryCastTo<TDesiredType>(this object item) where TDesiredType : class
		{
			var desiredType = item as TDesiredType;
			if (desiredType == null)
			{
				throw new InvalidOperationException("Cannot convert a " + item.GetType().Name + " to a " + typeof(TDesiredType).Name + ".");
			}
			return desiredType;
		}
	}
}