using System;

using FluentAssert;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	public class StringExtensionsTests
	{
		[TestFixture]
		public class When_asked_if_a_string_IsNullOrEmpty
		{
			[Test]
			public void Should_return_false_if_the_input_is_not_empty()
			{
				const string input = "\r\n";
				bool result = input.IsNullOrEmpty();
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_true_if_the_input_is_empty()
			{
				string input = String.Empty;
				bool result = input.IsNullOrEmpty();
				result.ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_null()
			{
				const string input = null;
				bool result = input.IsNullOrEmpty();
				result.ShouldBeTrue();
			}
		}

		[TestFixture]
		public class When_asked_if_a_string_IsNullOrEmpty_with_trim
		{
			[Test]
			public void Should_return_false_if_the_input_contains_only_non_whitespace_and_trim_is_false()
			{
				const string input = "ab";
				bool result = input.IsNullOrEmpty(false);
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_false_if_the_input_contains_only_non_whitespace_and_trim_is_true()
			{
				const string input = "ab";
				bool result = input.IsNullOrEmpty(true);
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_false_if_the_input_contains_only_whitespace_and_trim_is_false()
			{
				const string input = "\r\n";
				bool result = input.IsNullOrEmpty(false);
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_true_if_the_input_contains_only_whitespace_and_trim_is_true()
			{
				const string input = "\r\n";
				bool result = input.IsNullOrEmpty(true);
				result.ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_empty_and_trim_is_false()
			{
				string input = String.Empty;
				bool result = input.IsNullOrEmpty(false);
				result.ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_empty_and_trim_is_true()
			{
				string input = String.Empty;
				bool result = input.IsNullOrEmpty(true);
				result.ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_null_and_trim_is_false()
			{
				const string input = null;
				bool result = input.IsNullOrEmpty(false);
				result.ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_null_and_trim_is_true()
			{
				const string input = null;
				bool result = input.IsNullOrEmpty(true);
				result.ShouldBeTrue();
			}
		}
	}
}