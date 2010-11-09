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
	[TestFixture]
	public class When_asked_to_try_to_cast_an_object_to_a_specific_type
	{
		[Test]
		public void Should_succeed_if_the_cast_is_valid()
		{
			const string expected = "hello";
			object input = expected;
			string result = input.TryCastTo<string>();
			result.ShouldBeEqualTo(expected);
		}

		[Test]
		public void Should_throw_an_exception_if_the_cast_is_invalid()
		{
			const int expected = 6;
			object input = expected;
			Assert.Throws<InvalidOperationException>(() => input.TryCastTo<string>());
		}
	}

	[TestFixture]
	public class When_asked_to_get_a_non_null_value_for_an_input
	{
		[Test]
		public void Should_return_the_input_given_a_non_null_input()
		{
			var input = new Test();
			var result = input.ToNonNull();
			result.ShouldBeEqualTo(input);
		}

		[Test]
		public void Should_return_the_input_given_a_null_input()
		{
			const Test input = null;
			var result = input.ToNonNull();
			result.ShouldNotBeNull();
		}

		public class Test
		{
		}
	}
}