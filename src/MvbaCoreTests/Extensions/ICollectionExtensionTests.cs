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

using FluentAssert;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	[UsedImplicitly]
	public class ICollectionExtensionsTests
	{
		[TestFixture]
		public class When_asked_if_an_ICollection_IsNullOrEmpty
		{
			[Test]
			public void Should_return_false_if_the_input_contains_items()
			{
				// ReSharper disable UseObjectOrCollectionInitializer
				var input = new ArrayList();
				// ReSharper restore UseObjectOrCollectionInitializer
				input.Add(6);
				input.IsNullOrEmpty().ShouldBeFalse();
			}

			[Test]
			public void Should_return_true_if_the_input_is_empty()
			{
				var input = new ArrayList();
				input.IsNullOrEmpty().ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_null()
			{
				const ArrayList input = null;
				input.IsNullOrEmpty().ShouldBeTrue();
			}
		}

		[TestFixture]
		public class When_asked_if_an_ICollection_contains_Any_items
		{
			[Test]
			public void Should_return_false_if_the_input_is_empty()
			{
				var input = new ArrayList();
				input.Any().ShouldBeFalse();
			}

			[Test]
			public void Should_return_true_if_the_input_contains_items()
			{
				// ReSharper disable UseObjectOrCollectionInitializer
				var input = new ArrayList();
				// ReSharper restore UseObjectOrCollectionInitializer
				input.Add(6);
				input.Any().ShouldBeTrue();
			}

			[Test]
			public void Should_throw_an_exception_if_the_input_is_null()
			{
				const ArrayList input = null;
				Assert.Throws<NullReferenceException>(() => input.Any());
			}
		}
	}
}