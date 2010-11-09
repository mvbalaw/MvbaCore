using System;

namespace MvbaCore.Extensions
{
	public static class Int32Extensions
	{
		public static void Times(this int count, Action action)
		{
			for (int i = 0; i < count; i++)
			{
				action();
			}
		}

		public static void Times(this int count, Action<int> action)
		{
			for (int i = 0; i < count; i++)
			{
				action(i);
			}
		}
	}
}