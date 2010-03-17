using JetBrains.Annotations;

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

		public static string ToCamelCase([CanBeNull] this string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				return str;
			}
			str = Char.ToLower(str[0]) + str.Substring(1);
			return str;
		}

		public static string ToNonNull(this string input)
		{
			if (input == null)
			{
				return "";
			}
			return input;
		}
	}
}