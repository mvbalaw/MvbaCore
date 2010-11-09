//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using FluentAssert;

using MvbaCore.Extensions;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	public class Int32ExtensionsTests
	{
		[TestFixture]
		public class When_asked_to_repeat_an_action_a_certain_number_of_times
		{
			[Test]
			public void Given_a_negative_value_should_not_perform_the_action()
			{
				int counter = 0;
				(-5).Times(() => counter = counter + 1);
				counter.ShouldBeEqualTo(0);
			}

			[Test]
			public void Given_a_positive_value_should_perform_the_action__value__times()
			{
				int counter = 0;
				(5).Times(() => counter = counter + 1);
				counter.ShouldBeEqualTo(5);
			}

			[Test]
			public void Given_zero_should_not_perform_the_action()
			{
				int counter = 0;
				0.Times(() => counter = counter + 1);
				counter.ShouldBeEqualTo(0);
			}
		}

		[TestFixture]
		public class When_asked_to_repeat_an_integer_action_a_certain_number_of_times
		{
			[Test]
			public void Given_a_negative_value_should_not_perform_the_action()
			{
				int counter = 0;
				(-5).Times(x => counter = counter + x);
				counter.ShouldBeEqualTo(0);
			}

			[Test]
			public void Given_a_positive_value_should_perform_the_action__value__times()
			{
				int counter = 0;
				5.Times(x => counter = counter + x);
				counter.ShouldBeEqualTo(0 + 1 + 2 + 3 + 4);
			}

			[Test]
			public void Given_zero_should_not_perform_the_action()
			{
				int counter = 0;
				0.Times(x => counter = counter + x);
				counter.ShouldBeEqualTo(0);
			}
		}
	}
}