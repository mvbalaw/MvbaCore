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

using FluentAssert;

using JetBrains.Annotations;

using MvbaCore.Collections;

using NUnit.Framework;

namespace MvbaCoreTests.Collections
{
	[UsedImplicitly]
	public class PersistentListTests
	{
		[TestFixture]
		public class When_asked_to_Sort
		{
			[Test]
			public void Given__a_b()
			{
				var input = new[] { "a", "b" };
				var persistentList = new PersistentList<string>(input);
				persistentList.Sort((x, y) => x.CompareTo(y));
				persistentList[0].ShouldBeEqualTo("a");
				persistentList[1].ShouldBeEqualTo("b");
			}

			[Test]
			public void Given__a_b_c()
			{
				var input = new[] { "a", "b", "c" };
				var persistentList = new PersistentList<string>(input);
				persistentList.Sort((x, y) => x.CompareTo(y));
				persistentList[0].ShouldBeEqualTo("a");
				persistentList[1].ShouldBeEqualTo("b");
				persistentList[2].ShouldBeEqualTo("c");
			}

			[Test]
			public void Given__a_c_b()
			{
				var input = new[] { "a", "c", "b" };
				var persistentList = new PersistentList<string>(input);
				persistentList.Sort((x, y) => x.CompareTo(y));
				persistentList[0].ShouldBeEqualTo("a");
				persistentList[1].ShouldBeEqualTo("b");
				persistentList[2].ShouldBeEqualTo("c");
			}

			[Test]
			public void Given__b_a()
			{
				var input = new[] { "b", "a" };
				var persistentList = new PersistentList<string>(input);
				persistentList.Sort((x, y) => x.CompareTo(y));
				persistentList[0].ShouldBeEqualTo("a");
				persistentList[1].ShouldBeEqualTo("b");
			}

			[Test]
			public void Given__b_a_c()
			{
				var input = new[] { "b", "a", "c" };
				var persistentList = new PersistentList<string>(input);
				persistentList.Sort((x, y) => x.CompareTo(y));
				persistentList[0].ShouldBeEqualTo("a");
				persistentList[1].ShouldBeEqualTo("b");
				persistentList[2].ShouldBeEqualTo("c");
			}

			[Test]
			public void Given__b_c_a()
			{
				var input = new[] { "b", "c", "a" };
				var persistentList = new PersistentList<string>(input);
				persistentList.Sort((x, y) => x.CompareTo(y));
				persistentList[0].ShouldBeEqualTo("a");
				persistentList[1].ShouldBeEqualTo("b");
				persistentList[2].ShouldBeEqualTo("c");
			}

			[Test]
			public void Given__c_a_b()
			{
				var input = new[] { "c", "a", "b" };
				var persistentList = new PersistentList<string>(input);
				persistentList.Sort((x, y) => x.CompareTo(y));
				persistentList[0].ShouldBeEqualTo("a");
				persistentList[1].ShouldBeEqualTo("b");
				persistentList[2].ShouldBeEqualTo("c");
			}

			[Test]
			public void Given__c_b_a()
			{
				var input = new[] { "c", "b", "a" };
				var persistentList = new PersistentList<string>(input);
				persistentList.Sort((x, y) => x.CompareTo(y));
				persistentList[0].ShouldBeEqualTo("a");
				persistentList[1].ShouldBeEqualTo("b");
				persistentList[2].ShouldBeEqualTo("c");
			}

			[Test]
			public void Given_one_item()
			{
				var input = new[] { "a" };
				var persistentList = new PersistentList<string>(input);
				// should not throw if only one item
				persistentList.Sort((x, y) => x.CompareTo(y));
			}

			[Test]
			public void Given_zero_items()
			{
				var input = new List<string>();
				var persistentList = new PersistentList<string>(input);
				// should not throw if no items
				persistentList.Sort((x, y) => x.CompareTo(y));
			}
		}
	}
}