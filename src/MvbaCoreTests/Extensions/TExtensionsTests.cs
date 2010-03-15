using System;

using FluentAssert;

using MvbaCore.Extensions;

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
}