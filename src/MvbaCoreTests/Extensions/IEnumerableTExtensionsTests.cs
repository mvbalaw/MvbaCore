using System.Collections.Generic;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	public class IEnumerableTExtensionsTest
	{
		[TestFixture]
		public class When_asked_if_an_IEnumerable_T_IsNullOrEmpty
		{
			[Test]
			public void Should_return_false_if_the_input_contains_items()
			{
				IList<int> input = new List<int>
				{
					6
				};
				input.IsNullOrEmpty().ShouldBeFalse();
			}

			[Test]
			public void Should_return_true_if_the_input_is_empty()
			{
				IList<int> input = new List<int>();
				input.IsNullOrEmpty().ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_null()
			{
				const IList<int> input = null;
				input.IsNullOrEmpty().ShouldBeTrue();
			}
		}

		[TestFixture]
		public class When_asked_to_join_an_enumerable_list_of_items
		{
			[Test]
			public void Should_return_a_string_containing_the_list_items_separated_by_the_delimiter()
			{
				const int one = 1;
				const int item = 3;
				var items = new List<int>
				{
					one,
					item
				};
				const string delimiter = "','";
				string expect = one + delimiter + item;

				Assert.AreEqual(expect, items.Join(delimiter));
			}

			[Test]
			public void Should_return_an_empty_string_if_the_list_is_empty()
			{
				var items = new List<string>();
				Assert.IsEmpty(items.Join("x"));
			}

			[Test]
			public void Should_return_an_empty_string_if_the_list_is_null()
			{
				const List<string> items = null;
				Assert.IsEmpty(items.Join("x"));
			}

			[Test]
			public void Should_use_empty_string_if_the_delimiter_is_null()
			{
				const int one = 1;
				const int item = 3;
				var items = new List<int>
				{
					one,
					item
				};
				const string delimiter = null;
				string expect = one + "" + item;

				Assert.AreEqual(expect, items.Join(delimiter));
			}
		}

		[TestFixture]
		public class When_asked_to_return_items_in_sets
		{
			[Test]
			public void Should_fill_empty_spots_in_the_last_set_with_the_default_value_if_fill_is_requested()
			{
				var input = "abcdefghijklm".Select(x => x.ToString()).ToList();
				var result = input.InSetsOf(5, true, "x");
				result.Count().ShouldBeEqualTo(3);
				result.First().Count.ShouldBeEqualTo(5);
				var last = result.Last();
				last.Count.ShouldBeEqualTo(5);
				last.Join("").ShouldBeEqualTo("klmxx");
			}

			[Test]
			public void Should_not_fill_the_last_set_if_fill_is_not_requested()
			{
				var input = "abcdefghijkm".Select(x => x.ToString()).ToList();
				var result = input.InSetsOf(5, false, "x");
				result.Count().ShouldBeEqualTo(3);
				result.First().Count.ShouldBeEqualTo(5);
				result.Last().Count.ShouldBeEqualTo(2);
			}

			[Test]
			public void Should_return_the_correct_number_of_sets_if_the_input_contains_a_multiple_of_the_setSize()
			{
				var input = "abcdefghij".Select(x => x.ToString()).ToList();
				var result = input.InSetsOf(5);
				result.Count().ShouldBeEqualTo(2);
				result.First().Count.ShouldBeEqualTo(5);
				result.Last().Count.ShouldBeEqualTo(5);
			}

			[Test]
			public void Should_separate_the_input_into_sets_of_size_requested()
			{
				var input = "abcdefghijklm".Select(x => x.ToString()).ToList();
				var result = input.InSetsOf(5);
				result.Count().ShouldBeEqualTo(3);
				result.First().Count.ShouldBeEqualTo(5);
				result.Last().Count.ShouldBeEqualTo(3);
			}
		}
	}
}