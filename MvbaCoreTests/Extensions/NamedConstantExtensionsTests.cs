using System;

using FluentAssert;

using MvbaCore;
using MvbaCore.Extensions;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	public class NamedConstantExtensionsTests
	{
		[TestFixture]
		public class When_asked_to_get_the_non_null_value_for_a_NamedConstant
		{
			[Test]
			public void Should_return_the_field_marked_with__DefaultKeyAttribute__given_null_input()
			{
				const TestNamedConstantWithDefault namedConstantWithDefault = null;

				var actualNamedConstant = namedConstantWithDefault.ToNonNull();
				Console.WriteLine(actualNamedConstant);
				ReferenceEquals(actualNamedConstant, TestNamedConstantWithDefault.Foo).ShouldBeTrue();
			}

			[Test]
			public void Should_return_the_input_if_it_is_non_null()
			{
				TestNamedConstantWithoutDefault namedConstantWithoutDefault = TestNamedConstantWithoutDefault.Foo;

				var actualNamedConstant = namedConstantWithoutDefault.ToNonNull();
				ReferenceEquals(actualNamedConstant, namedConstantWithoutDefault).ShouldBeTrue();
			}

			[Test]
			public void Should_throw_an_argument_exception_if_the_input_is_null()
			{
				const TestNamedConstantWithoutDefault namedConstantWithoutDefault = null;

				namedConstantWithoutDefault.ShouldThrowAnException(x => x.ToNonNull()).OfType<ArgumentException>();
			}

			public class TestNamedConstantWithoutDefault : NamedConstant<TestNamedConstantWithoutDefault>
			{
				public static readonly TestNamedConstantWithoutDefault Bar = new TestNamedConstantWithoutDefault();
				public static readonly TestNamedConstantWithoutDefault Foo = new TestNamedConstantWithoutDefault();
			}

			public class TestNamedConstantWithDefault : NamedConstant<TestNamedConstantWithDefault>
			{
				public static readonly TestNamedConstantWithDefault Bar = new TestNamedConstantWithDefault();

				[DefaultKey]
				public static readonly TestNamedConstantWithDefault Foo = new TestNamedConstantWithDefault();
			}
		}
	}
}