//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.Collections.Generic;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCore.Tests.Extensions
{
	[UsedImplicitly]
	public class ListExtensionsTests
	{
		public abstract class ListExtensionTestBase
		{
			protected readonly List<int> Items = new List<int>
				                                     {
					                                     0,
					                                     1,
					                                     2,
					                                     3,
					                                     4,
					                                     5,
					                                     6,
					                                     7,
					                                     8,
					                                     9
				                                     };

			protected abstract int? Count { get; }

			protected abstract int CountOfItemsFromListToCompareWithRangeItemsCount { get; }
			protected abstract int? FirstRow { get; }
			protected abstract int ItemFromListToCompareWithFirstRangeItem { get; }
			protected abstract int ItemFromListToCompareWithLastRangeItem { get; }

			protected void Test()
			{
				var range = Items.GetRange(FirstRow, Count);
				var rangeCount = range.Count;
				Assert.AreEqual(CountOfItemsFromListToCompareWithRangeItemsCount, rangeCount);
				Assert.AreEqual(ItemFromListToCompareWithFirstRangeItem, range[0]);
				Assert.AreEqual(ItemFromListToCompareWithLastRangeItem, range[rangeCount - 1]);
			}
		}

		[TestFixture]
		public class When_count_is_a_number_less_than_the_list_count : ListExtensionTestBase
		{
			[Test]
			public void Should_return_the_entire_list()
			{
				Test();
			}

			protected override int? Count
			{
				get { return 6; }
			}

			protected override int CountOfItemsFromListToCompareWithRangeItemsCount
			{
				get { return 6; }
			}
			protected override int? FirstRow
			{
				get { return null; }
			}

			protected override int ItemFromListToCompareWithFirstRangeItem
			{
				get { return Items[0]; }
			}

			protected override int ItemFromListToCompareWithLastRangeItem
			{
				get { return Items[5]; }
			}
		}

		[TestFixture]
		public class When_first_row_and_count_are_null : ListExtensionTestBase
		{
			[Test]
			public void Should_return_the_entire_list()
			{
				Test();
			}

			protected override int? Count
			{
				get { return null; }
			}

			protected override int CountOfItemsFromListToCompareWithRangeItemsCount
			{
				get { return Items.Count; }
			}
			protected override int? FirstRow
			{
				get { return null; }
			}

			protected override int ItemFromListToCompareWithFirstRangeItem
			{
				get { return Items[0]; }
			}

			protected override int ItemFromListToCompareWithLastRangeItem
			{
				get { return Items[Items.Count - 1]; }
			}
		}

		[TestFixture]
		public class When_first_row_is_an_index_within_the_list : ListExtensionTestBase
		{
			[Test]
			public void Should_return_the_entire_list()
			{
				Test();
			}

			protected override int? Count
			{
				get { return null; }
			}

			protected override int CountOfItemsFromListToCompareWithRangeItemsCount
			{
				get { return 5; }
			}
			protected override int? FirstRow
			{
				get { return 5; }
			}

			protected override int ItemFromListToCompareWithFirstRangeItem
			{
				get { return Items[5]; }
			}

			protected override int ItemFromListToCompareWithLastRangeItem
			{
				get { return Items[Items.Count - 1]; }
			}
		}
	}
}