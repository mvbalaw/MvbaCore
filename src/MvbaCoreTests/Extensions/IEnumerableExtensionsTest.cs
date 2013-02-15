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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FluentAssert;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	[UsedImplicitly]
	public class IEnumerableExtensionsTests
	{
		private static Func<string, bool> MatchNotNullOrEmptyDelegate()
		{
			return str => !str.IsNullOrEmpty();
		}

		[TestFixture]
		public class When_asked_to_Select_items_from_an_IEnumerable
		{
			[Test]
			public void Should_not_find_any_matches_in_an_empty_list()
			{
				var items = new ArrayList();
				var result =
					new List<string>(items.Select<int, string>(item => item > 0 ? item.ToString() : Math.Abs(item).ToString()));
				Assert.IsEmpty(result);
			}

			[Test]
			public void Should_select_items_in_a_populated_list()
			{
				var items = new ArrayList
					            {
						            -1,
						            2,
						            -3
					            };
				var result =
					new List<string>(items.Select<int, string>(item => item > 0 ? item.ToString() : Math.Abs(item).ToString()));
				Assert.IsNotNull(result);
				Assert.AreEqual(items.Count, result.Count, "counts differ");
				Assert.AreEqual(Math.Abs((int)items[0]).ToString(), result[0], "item 0");
				Assert.AreEqual(Math.Abs((int)items[1]).ToString(), result[1], "item 1");
				Assert.AreEqual(Math.Abs((int)items[2]).ToString(), result[2], "item 2");
			}

			[Test]
			public void Should_throw_an_exception_if_the_collection_is_null()
			{
				const ArrayList items = null;
				// ReSharper disable AssignNullToNotNullAttribute
				Assert.Throws<ArgumentNullException>(() => new List<string>(items.Select<int, string>(item => item > 0 ? item.ToString() : Math.Abs(item).ToString())));
				// ReSharper restore AssignNullToNotNullAttribute
			}
		}

		[TestFixture]
		public class When_asked_to_find_all_matches_on_an_IEnumerable
		{
			[Test]
			public void Should_find_all_matches_in_a_populated_list()
			{
				var items = new ArrayList
					            {
						            "Food",
						            "",
						            "Bar",
						            null,
						            "Base"
					            };

				var matches = new List<string>(items.Where(MatchNotNullOrEmptyDelegate()));
				Assert.AreEqual(3, matches.Count, "count is wrong");
				Assert.AreEqual(items[0], matches[0], "first item");
				Assert.AreEqual(items[2], matches[1], "second item");
				Assert.AreEqual(items[4], matches[2], "third item");
			}

			[Test]
			public void Should_not_find_any_matches_in_an_empty_list()
			{
				var items = new ArrayList();
				Assert.IsFalse(items.Where(MatchNotNullOrEmptyDelegate()).Any(), "should not have returned any items");
			}

			[Test]
			public void Should_throw_an_exception_if_the_collection_is_null()
			{
				const ArrayList items = null;
				// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
				Assert.Throws<ArgumentNullException>(() => items.Where(MatchNotNullOrEmptyDelegate()).Any());
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
				// ReSharper restore AssignNullToNotNullAttribute
			}
		}

		[TestFixture]
		public class When_asked_to_find_the_first_match_for_an_IEnumerable
		{
			[Test]
			public void Should_find_the_first_match_in_a_populated_list()
			{
				var items = new ArrayList
					            {
						            "",
						            null,
						            "Food",
						            "Bar",
						            "Base"
					            };

				var item = items.FirstOrDefault(MatchNotNullOrEmptyDelegate());
				Assert.AreEqual(items[2], item);
			}

			[Test]
			public void Should_not_find_any_matches_in_an_empty_list()
			{
				var items = new ArrayList();
				var item = items.FirstOrDefault(MatchNotNullOrEmptyDelegate());
				Assert.IsNull(item);
			}

			[Test]
			public void Should_throw_an_exception_if_the_collection_is_null()
			{
				const ArrayList items = null;
				// ReSharper disable AssignNullToNotNullAttribute
				Assert.Throws<ArgumentNullException>(() => items.FirstOrDefault(MatchNotNullOrEmptyDelegate()));
				// ReSharper restore AssignNullToNotNullAttribute
			}
		}

		[TestFixture]
		public class When_asked_to_get_the_max_number_in_the_sequence
		{
			private const int Default = 0;
			private List<int> _list;
			private int _result;

			[Test]
			public void Given_a_list()
			{
				Test.Static()
				    .When(asked_to_get_the_max_number_in_the_sequence)
				    .With(a_list)
				    .Should(return_the_maximum_number_in_the_list)
				    .Verify();
			}

			[Test]
			public void Given_a_null_list()
			{
				Test.Static()
				    .When(asked_to_get_the_max_number_in_the_sequence)
				    .With(a_null_list)
				    .Should(return_default)
				    .Verify();
			}

			[Test]
			public void Given_an_empty_list()
			{
				Test.Static()
				    .When(asked_to_get_the_max_number_in_the_sequence)
				    .With(an_empty_list)
				    .Should(return_default)
				    .Verify();
			}

			private void a_list()
			{
				_list = new List<int>
					        {
						        1,
						        5,
						        8
					        };
			}

			private void a_null_list()
			{
				_list = null;
			}

			private void an_empty_list()
			{
				_list = new List<int>();
			}

			private void asked_to_get_the_max_number_in_the_sequence()
			{
				_result = _list.Max(x => x, Default);
			}

			private void return_default()
			{
				_result.ShouldBeEqualTo(Default);
			}

			private void return_the_maximum_number_in_the_list()
			{
				_result.ShouldBeEqualTo(8);
			}
		}
	}
}