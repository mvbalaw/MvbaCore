//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

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
		public class When_asked_if_a_string_is_null_or_empty_with_optional_trim
		{
			private string _input;
			private bool _result;
			private bool _trim;

			[Test]
			public void Given_a_null_string_and_trim_is_false()
			{
				Test.Verify(
					with_a_null_string,
					with_trim_set_to_false,
					when_asked_if_a_string_is_null_or_empty,
					should_return_true
					);
			}

			[Test]
			public void Given_a_null_string_and_trim_is_true()
			{
				Test.Verify(
					with_a_null_string,
					with_trim_set_to_true,
					when_asked_if_a_string_is_null_or_empty,
					should_return_true
					);
			}

			[Test]
			public void Given_a_string_containing_non_whitespace_and_trim_is_false()
			{
				Test.Verify(
					with_a_string_containing_non_whitespace,
					with_trim_set_to_false,
					when_asked_if_a_string_is_null_or_empty,
					should_return_false
					);
			}

			[Test]
			public void Given_a_string_containing_non_whitespace_and_trim_is_true()
			{
				Test.Verify(
					with_a_string_containing_non_whitespace,
					with_trim_set_to_true,
					when_asked_if_a_string_is_null_or_empty,
					should_return_false
					);
			}

			[Test]
			public void Given_a_string_containing_only_whitespace_and_trim_is_false()
			{
				Test.Verify(
					with_a_string_containing_only_whitespace,
					with_trim_set_to_false,
					when_asked_if_a_string_is_null_or_empty,
					should_return_false
					);
			}

			[Test]
			public void Given_a_string_containing_only_whitespace_and_trim_is_true()
			{
				Test.Verify(
					with_a_string_containing_only_whitespace,
					with_trim_set_to_true,
					when_asked_if_a_string_is_null_or_empty,
					should_return_true
					);
			}

			[Test]
			public void Given_an_empty_string_and_trim_is_false()
			{
				Test.Verify(
					with_an_empty_string,
					with_trim_set_to_false,
					when_asked_if_a_string_is_null_or_empty,
					should_return_true
					);
			}

			[Test]
			public void Given_an_empty_string_and_trim_is_true()
			{
				Test.Verify(
					with_an_empty_string,
					with_trim_set_to_true,
					when_asked_if_a_string_is_null_or_empty,
					should_return_true
					);
			}

			private void should_return_false()
			{
				_result.ShouldBeFalse();
			}

			private void should_return_true()
			{
				_result.ShouldBeTrue();
			}

			private void when_asked_if_a_string_is_null_or_empty()
			{
				_result = _input.IsNullOrEmpty(_trim);
			}

			private void with_a_null_string()
			{
				_input = null;
			}

			private void with_a_string_containing_non_whitespace()
			{
				_input = "aa";
			}

			private void with_a_string_containing_only_whitespace()
			{
				_input = "\n\r\t ";
			}

			private void with_an_empty_string()
			{
				_input = "";
			}

			private void with_trim_set_to_false()
			{
				_trim = false;
			}

			private void with_trim_set_to_true()
			{
				_trim = true;
			}
		}

		[TestFixture]
		public class When_asked_to_change_a_string_ToCamelCase
		{
			[Test]
			public void Should_make_the_first_character_lower_case_but_not_change_the_case_of_the_rest()
			{
				const string test = "TEST";
				test.ToCamelCase().ShouldBeEqualTo("tEST");
			}

			[Test]
			public void Should_make_the_first_character_lower_case_if_the_string_is_only_one_character_long()
			{
				const string test = "A";
				test.ToCamelCase().ShouldBeEqualTo("a");
			}

			[Test]
			public void Should_return_empty_if_the_input_is_empty()
			{
				const string test = "";
				test.ToCamelCase().ShouldBeEqualTo("");
			}

			[Test]
			public void Should_return_null_if_the_input_is_null()
			{
				const string test = null;
				test.ToCamelCase().ShouldBeNull();
			}
		}

		[TestFixture]
		public class When_asked_to_get_a_non_null_value_for_a_string
		{
			[Test]
			public void Should_return_the_string_given_a_non_null_input()
			{
				const string input = "hello";
				string result = input.ToNonNull();
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Should_return_the_string_given_a_null_input()
			{
				const string input = null;
				string result = input.ToNonNull();
				result.ShouldNotBeNull();
			}
		}
	}
}