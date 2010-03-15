using System;

namespace MvbaCore.Extensions
{
	public static class TExtensions
	{
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