//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace MvbaCore.Tests
{
	[TestFixture]
	public class MergeSorterTests
	{
		[Test]
		public void Given_empty_list_and_list_5_6_7_should_get_5_6_7()
		{
			var list1 = new int[] {};
			var list2 = new[] {5, 6, 7};

			var sorter = new MergeSorter<int>();
			var result = sorter.Merge(list1, list2, (a, b) => a.CompareTo(b)).ToList();
			result.ShouldContainAllInOrder(new[] {5, 6, 7});
		}

		[Test]
		public void Given_list_1_2_3_4_and_empty_list_should_get_1_2_3_4()
		{
			var list1 = new[] {1, 2, 3, 4};
			var list2 = new int[] {};

			var sorter = new MergeSorter<int>();
			var result = sorter.Merge(list1, list2, (a, b) => a.CompareTo(b)).ToList();
			result.ShouldContainAllInOrder(new[] {1, 2, 3, 4});
		}

		[Test]
		public void Given_lists_1_2_3_4_and_1_4_should_get_1_1_2_3_4_4()
		{
			var list1 = new[] {1, 2, 3, 4};
			var list2 = new[] {1, 4};

			var sorter = new MergeSorter<int>();
			var result = sorter.Merge(list1, list2, (a, b) => a.CompareTo(b)).ToList();
			result.ShouldContainAllInOrder(new[] {1, 1, 2, 3, 4, 4});
		}

		[Test]
		public void Given_lists_1_2_3_4_and_5_6_7_should_get_1_2_3_4_5_6_7()
		{
			var list1 = new[] {1, 2, 3, 4};
			var list2 = new[] {5, 6, 7};

			var sorter = new MergeSorter<int>();
			var result = sorter.Merge(list1, list2, (a, b) => a.CompareTo(b)).ToList();
			result.ShouldContainAllInOrder(new[] {1, 2, 3, 4, 5, 6, 7});
		}

		[Test]
		public void Given_lists_5_6_7_and_1_2_3_4_should_get_1_2_3_4_5_6_7()
		{
			var list1 = new[] {5, 6, 7};
			var list2 = new[] {1, 2, 3, 4};

			var sorter = new MergeSorter<int>();
			var result = sorter.Merge(list1, list2, (a, b) => a.CompareTo(b)).ToList();
			result.ShouldContainAllInOrder(new[] {1, 2, 3, 4, 5, 6, 7});
		}

		[Test]
		public void Given_two_empty_lists_should_get_an_empty_list()
		{
			var list1 = new int[] {};
			var list2 = new int[] {};

			var sorter = new MergeSorter<int>();
			var result = sorter.Merge(list1, list2, (a, b) => a.CompareTo(b)).ToList();
			result.ShouldBeEmpty();
		}
	}
}