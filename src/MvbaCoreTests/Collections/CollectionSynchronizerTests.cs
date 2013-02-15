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

using MvbaCore.Collections;

using NUnit.Framework;

namespace MvbaCoreTests.Collections
{
	[UsedImplicitly]
	public class CollectionSynchronizerTests
	{
		public static void AssertCorrectContents<T>(IEnumerable<T> expected, CollectionSynchronizer<T> synchronizer)
		{
			var removed = synchronizer.Removed.ToList();
			Assert.IsEmpty(removed, "lists should be the same but items were removed : " + removed);
			var added = synchronizer.Added.ToList();
			Assert.IsEmpty(added, "lists should be the same but items were added : " + added);
			Assert.AreEqual(expected.Count(), synchronizer.Unchanged.Count(),
							"lists should be the same but not all expected unchanged items exist");
		}

		public class TestItem
		{
			public int KeyId { get; set; }
			public string Name { get; set; }
		}

		[TestFixture]
		public class When_asked_to_construct_a_CollectionSynchronizer
		{
			[Test]
			public void Should_throw_exception_if_the_list_being_synchronized_is_null()
			{
				const List<TestItem> listA = null;
				var listB = new List<TestItem>();
				// ReSharper disable AssignNullToNotNullAttribute
				Assert.Throws<ArgumentNullException>(() => new CollectionSynchronizer<TestItem>(listA, listB));
				// ReSharper restore AssignNullToNotNullAttribute
			}
		}

		[TestFixture]
		public class When_asked_to_synchronize_two_lists
		{
			private List<TestItem> _listA;
			private List<TestItem> _listB;
			private CollectionSynchronizer<TestItem> _synchronizer;

			[TestFixtureSetUp]
			public void BeforeFirstTest()
			{
				var itemA1 = new TestItem
				{
					KeyId = 1,
					Name = "A"
				};
				var itemA2 = new TestItem
				{
					KeyId = 2,
					Name = "A & B"
				};
				var itemA3 = new TestItem
				{
					KeyId = 3,
					Name = "A & B"
				};
				var itemB2 = new TestItem
				{
					KeyId = 2,
					Name = "A & B"
				};
				var itemB3 = new TestItem
				{
					KeyId = 3,
					Name = "A & B"
				};
				var itemB4 = new TestItem
				{
					KeyId = 4,
					Name = "B"
				};
				_listA = new List<TestItem>
					{
						itemA1,
						itemA2,
						itemA3
					};
				_listB = new List<TestItem>
					{
						itemB2,
						itemB3,
						itemB4
					};
			}

			[SetUp]
			public void BeforeEachTest()
			{
				_synchronizer = new CollectionSynchronizer<TestItem>(_listA, _listB);
				_synchronizer.Synchronize(item => item.KeyId, item => item == null);
			}

			[Test]
			public void Should_populate_Added_with_items_that_are_in_listA_that_are_not_in_listB()
			{
				AssertCorrectContents(_synchronizer.Added, "A");
			}

			[Test]
			public void Should_populate_Removed_with_items_that_are_in_listB_that_are_not_in_listA()
			{
				AssertCorrectContents(_synchronizer.Removed, "B");
			}

			[Test]
			public void Should_populate_Unchanged_with_items_that_are_in_both_listA_and_listB()
			{
				AssertCorrectContents(_synchronizer.Unchanged, "A & B");
			}

			private static void AssertCorrectContents(IEnumerable<TestItem> result, string expectedName)
			{
				var list = result.ToList();
				list.ShouldNotBeNull();
				list.Any().ShouldBeTrue();
				list.All(item => item.Name == expectedName).ShouldBeTrue();
			}
		}
	}
}