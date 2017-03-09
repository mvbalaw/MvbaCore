//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssert;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCore.Tests.Extensions
{
	[UsedImplicitly]
	public class StringExtensionsTests
	{
		[TestFixture]
		public class When_asked_if_a_string_IsNullOrEmpty
		{
			[Test]
			public void Should_return_false_if_the_input_is_not_empty()
			{
				const string input = "\r\n";
				var result = input.IsNullOrEmpty();
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_true_if_the_input_is_empty()
			{
				var input = String.Empty;
				var result = input.IsNullOrEmpty();
				result.ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_null()
			{
				const string input = null;
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse
				var result = input.IsNullOrEmpty();
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse
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
		public class When_asked_to_add_an_item_to_a_list
		{
			private List<string> _list;

			[SetUp]
			public void BeforeEachTest()
			{
				_list = new List<string>();
			}

			[Test]
			public void Should_add_the_item_if_it_is_not_null_and_not_empty()
			{
				const string item = "Test";
				_list.AddIfNotNullOrEmpty(item);
				_list.Count.ShouldBeEqualTo(1);
				_list.First().ShouldBeEqualTo(item);
			}

			[Test]
			public void Should_not_add_the_item_if_it_is_empty()
			{
				_list.AddIfNotNullOrEmpty("");
				_list.Count.ShouldBeEqualTo(0);
			}

			[Test]
			public void Should_not_add_the_item_if_it_is_null()
			{
				_list.AddIfNotNullOrEmpty(null);
				_list.Count.ShouldBeEqualTo(0);
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
			public void Should_return_empty_string_if_the_input_is_null()
			{
				const string test = null;
// ReSharper disable once ExpressionIsAlwaysNull
				test.ToCamelCase().ShouldBeEqualTo("");
			}
		}

		[TestFixture]
		public class When_asked_to_convert_newlines_to_BR
		{
			[Test]
			public void Should_replace_the_newlines()
			{
				var input = "food" + Environment.NewLine + "bar";
				var result = input.NewlinesToBr();
				Assert.AreEqual("food<br />bar", result);
			}

			[Test]
			public void Should_return_null_if_the_input_is_null()
			{
				const string input = null;
// ReSharper disable once ExpressionIsAlwaysNull
				var result = input.NewlinesToBr();
				Assert.IsNull(result);
			}
		}

		[TestFixture]
		public class When_asked_to_convert_tabs_to_Nbsp
		{
			[Test]
			public void Should_replace_the_tabs()
			{
				const string input = "food" + "\t" + "bar";
				var result = input.TabsToNbsp();
				Assert.AreEqual("food&nbsp;&nbsp;&nbsp;&nbsp;bar", result);
			}

			[Test]
			public void Should_return_null_if_the_input_is_null()
			{
				const string input = null;
// ReSharper disable once ExpressionIsAlwaysNull
				var result = input.NewlinesToBr();
				Assert.IsNull(result);
			}
		}

		[TestFixture]
		public class When_asked_to_convert_to_title_case
		{
			[Test]
			public void Should_return_the_empty_string_for_string_of_length_zero()
			{
				Assert.AreEqual("", "".ToTitleCase());
			}

			[Test]
			public void Should_return_the_letter_in_caps_for_string_of_length_equal_to_one()
			{
				Assert.AreEqual("V", "v".ToTitleCase());
			}

			[Test]
			public void Should_return_the_string_in_title_case_for_string_of_length_greater_than_one()
			{
				Assert.AreEqual("Value", "value".ToTitleCase());
			}
		}

		[TestFixture]
		public class When_asked_to_get_a_non_null_value_for_a_string
		{
			[Test]
			public void Should_return_the_string_given_a_non_null_input()
			{
				const string input = "hello";
				var result = input.ToNonNull();
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Should_return_the_string_given_a_null_input()
			{
				const string input = null;
				var result = input.ToNonNull();
				result.ShouldNotBeNull();
			}
		}

		[TestFixture]
		public class When_asked_to_get_an_MD5_hash
		{
			[Test]
			public void Given__The_quick_brown_fox_jumps_over_the_lazy_dog()
			{
				// http://en.wikipedia.org/wiki/Md5
				const string input = "The quick brown fox jumps over the lazy dog";
				var result = input.GetMD5Hash();
				result.ShouldBeEqualTo("9e107d9d372bb6826bd81d3542a419d6");
			}

			[Test]
			public void Given_an_empty_string()
			{
				// http://en.wikipedia.org/wiki/Md5
				const string input = "";
				var result = input.GetMD5Hash();
				result.ShouldBeEqualTo("d41d8cd98f00b204e9800998ecf8427e");
			}
		}

		[UsedImplicitly]
		public class When_asked_to_group_items_in_a_string_array
		{
			[TestFixture]
			public class Given_a_null_separator
			{
				[Test]
				[ExpectedException(typeof(ArgumentException))]
				public void Should_throw_an_exception()
				{
					var lines = new string[0];
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
					lines.GroupBy(null);
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
				}
			}

			[TestFixture]
			public class Given_an_array_containing_data_and_a_trailing_separator
			{
				[Test]
				public void Should_return_the_data_in_one_group()
				{
					var lines = new[] { "a", "b", "c" };
					var grouped = lines.GroupBy("c");
					grouped.Count.ShouldBeEqualTo(1);
					grouped.First().ShouldContainAllInOrder(new[] { "a", "b" });
				}
			}

			[TestFixture]
			public class Given_an_array_containing_data_then_a_separator_then_more_data
			{
				[Test]
				public void Should_return_the_data_in_one_group()
				{
					var lines = new[] { "a", "b", "c", "d", "e" };
					var grouped = lines.GroupBy("c");
					grouped.Count.ShouldBeEqualTo(2);
					grouped.First().ShouldContainAllInOrder(new[] { "a", "b" });
					grouped.Last().ShouldContainAllInOrder(new[] { "d", "e" });
				}
			}

			[TestFixture]
			public class Given_an_array_containing_only_multiple_copies_of_the_separator
			{
				[Test]
				public void Should_return_an_empty_group_for_each_instance_of_the_separator()
				{
					var lines = new[] { "a", "a", "a" };
					var grouped = lines.GroupBy("a");
					grouped.Count.ShouldBeEqualTo(lines.Length);
					grouped.First().Count.ShouldBeEqualTo(0);
					grouped.Skip(1).First().Count.ShouldBeEqualTo(0);
					grouped.Skip(2).First().Count.ShouldBeEqualTo(0);
				}
			}

			[TestFixture]
			public class Given_an_array_containing_only_one_copy_of_the_separator
			{
				[Test]
				public void Should_return_an_empty_group()
				{
					var lines = new[] { "a" };
					var grouped = lines.GroupBy("a");
					grouped.Count.ShouldBeEqualTo(lines.Length);
					grouped.First().Count.ShouldBeEqualTo(0);
				}
			}

			[TestFixture]
			public class Given_an_array_that_does_not_contain_the_separator
			{
				[Test]
				public void Should_return_the_entire_input_in_a_single_group()
				{
					var lines = new[] { "a", "b", "c" };
					var grouped = lines.GroupBy("d");
					grouped.Count.ShouldBeEqualTo(1);
					grouped.First().ShouldContainAllInOrder(lines);
				}
			}

			[TestFixture]
			public class Given_an_empty_array
			{
				[Test]
				public void Should_return_an_empty_list()
				{
					var lines = new string[0];
					var grouped = lines.GroupBy("");
					grouped.ShouldNotBeNull();
					grouped.Count.ShouldBeEqualTo(0);
				}
			}

			[TestFixture]
			public class Given_data_lines_containing_only_whitespace
			{
				[Test]
				public void Should_remove_the_whitespace_lines()
				{
					var lines = new[] { " ", "\t", "a", "\r", "\n" };
					var grouped = lines.GroupBy("d");
					grouped.Count.ShouldBeEqualTo(1);
					grouped.First().Count.ShouldBeEqualTo(1);
					grouped.First().ShouldContainAllInOrder(new[] { "a" });
				}
			}
		}

		[TestFixture]
		public class When_asked_to_make_a_string_plural
		{
			[Test]
			public void Should_add__es__if_the_input_ends_with__ch()
			{
				const string input = "church";
				Assert.AreEqual("churches", input.ToPlural());
			}

			[Test]
			public void Should_add__es__if_the_input_ends_with__s()
			{
				const string input = "atlas";
				Assert.AreEqual("atlases", input.ToPlural());
			}

			[Test]
			public void Should_add__es__if_the_input_ends_with__sh()
			{
				const string input = "bush";
				Assert.AreEqual("bushes", input.ToPlural());
			}

			[Test]
			public void Should_add__es__if_the_input_ends_with__x()
			{
				const string input = "Box";
				Assert.AreEqual("Boxes", input.ToPlural());
			}

			[Test]
			public void Should_add__s__if_the_input_does_not_end_with__y_x_sh_ch__or__s()
			{
				const string input = "cat";
				Assert.AreEqual(input + "s", input.ToPlural());
			}

			[Test]
			public void Should_add__s__if_the_input_ends_with_a_vowel_followed_by__y()
			{
				const string input = "day";
				Assert.AreEqual("days", input.ToPlural());
			}

			[Test]
			public void Should_replace__y__with__ies__if_the_input_ends_with_a_consonant_followed_by__y()
			{
				const string input = "pony";
				Assert.AreEqual("ponies", input.ToPlural());
			}
		}

		[TestFixture]
		public class When_asked_to_prefix_a_string_with_a_or_an
		{
			[Test]
			public void Should_prefix_with__a__if_the_input_does_not_start_with__a_e_i_o_u__or_h()
			{
				const string input = "cat";
				input.PrefixWithAOrAn().ShouldBeEqualTo("a " + input);
			}

			[Test]
			public void Should_prefix_with__a__if_the_input_starts_with__ha()
			{
				const string input = "Hawaii";
				input.PrefixWithAOrAn().ShouldBeEqualTo("a " + input);
			}

			[Test]
			public void Should_prefix_with__a__if_the_input_starts_with__hot()
			{
				const string input = "Hotel";
				input.PrefixWithAOrAn().ShouldBeEqualTo("a " + input);
			}

			[Test]
			public void Should_prefix_with__a__if_the_input_starts_with__uni()
			{
				const string input = "Unicorn";
				input.PrefixWithAOrAn().ShouldBeEqualTo("a " + input);
			}

			[Test]
			public void Should_prefix_with__a__if_the_input_starts_with__ut_but_not_with_utt()
			{
				const string input = "Utah";
				input.PrefixWithAOrAn().ShouldBeEqualTo("a " + input);
			}

			[Test]
			public void Should_prefix_with__an__if_the_input_starts_with__a()
			{
				const string input = "Aardvark";
				input.PrefixWithAOrAn().ShouldBeEqualTo("an " + input);
			}

			[Test]
			public void Should_prefix_with__an__if_the_input_starts_with__e()
			{
				const string input = "Egret";
				input.PrefixWithAOrAn().ShouldBeEqualTo("an " + input);
			}

			[Test]
			public void Should_prefix_with__an__if_the_input_starts_with__h_but_not_with_hot()
			{
				const string input = "Hour";
				input.PrefixWithAOrAn().ShouldBeEqualTo("an " + input);
			}

			[Test]
			public void Should_prefix_with__an__if_the_input_starts_with__i()
			{
				const string input = "Ibex";
				input.PrefixWithAOrAn().ShouldBeEqualTo("an " + input);
			}

			[Test]
			public void Should_prefix_with__an__if_the_input_starts_with__o()
			{
				const string input = "Ocelot";
				input.PrefixWithAOrAn().ShouldBeEqualTo("an " + input);
			}

			[Test]
			public void Should_prefix_with__an__if_the_input_starts_with__u_but_not_with_uni()
			{
				const string input = "Uncle";
				input.PrefixWithAOrAn().ShouldBeEqualTo("an " + input);
			}

			[Test]
			public void Should_prefix_with__an__if_the_input_starts_with__uni()
			{
				const string input = "Unicorn";
				input.PrefixWithAOrAn().ShouldBeEqualTo("a " + input);
			}

			[Test]
			public void Should_prefix_with__an__if_the_input_starts_with__utt()
			{
				const string input = "Utter";
				input.PrefixWithAOrAn().ShouldBeEqualTo("an " + input);
			}
		}

		[TestFixture]
		public class When_asked_to_replace_old_values_with_new_values_in_a_string
		{
			private const string NewValue = "New";
			private const string OldValue = "Old";

			[Test]
			public void Should_replace_the_old_values_with_new_values_if_the_string_is_not_null()
			{
				const string input = "Old Value";
				var result = input.ReplaceIfExists(OldValue, NewValue);
				Assert.AreEqual("New Value", result);
			}

			[Test]
			public void Should_return_null_if_the_input_is_null()
			{
				const string input = null;
// ReSharper disable once ExpressionIsAlwaysNull
				var result = input.ReplaceIfExists(OldValue, NewValue);
				Assert.IsNull(result);
			}

			[Test]
			public void Should_return_the_input_as_is_if_the_old_value_is_null_or_empty()
			{
				const string input = "Old Value";
				var result = input.ReplaceIfExists("", NewValue);
				Assert.AreEqual(input, result);
			}
		}

		[TestFixture]
		public class When_asked_to_trim_if_longer_than_certain_length
		{
			private const int MaxLength = 10;

			[Test]
			public void Should_return_the_string_as_is_if_it_is_shorter_than_specified_length()
			{
				const string item = "Test";
				item.TrimIfLongerThan(MaxLength).ShouldBeEqualTo(item);
			}

			[Test]
			public void Should_return_the_trimmed_string_if_it_is_longer_than_specified_length()
			{
				const string item = "Test on the horizon";
				item.TrimIfLongerThan(MaxLength).ShouldBeEqualTo(item.Substring(0, MaxLength));
			}
		}
	}
}