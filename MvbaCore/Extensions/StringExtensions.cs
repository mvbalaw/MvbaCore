namespace System
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string input)
		{
			return input.IsNullOrEmpty(false);
		}

		public static bool IsNullOrEmpty(this string input, bool trim)
		{
			if (String.IsNullOrEmpty(input))
			{
				return true;
			}
			return trim && input.Trim() == "";
		}
	}
}