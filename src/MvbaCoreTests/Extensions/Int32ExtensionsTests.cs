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