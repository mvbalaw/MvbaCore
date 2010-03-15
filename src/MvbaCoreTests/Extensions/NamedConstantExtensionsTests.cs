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
		public class When_asked_to_get_the_value_for_a_NamedConstant_or_its_default
		{
			[Test]
			public void Should_return_the_field_marked_with__DefaultKeyAttribute__given_null_input()
			{
				const TestNamedConstantWithDefault namedConstantWithDefault = null;

				var actualNamedConstant = namedConstantWithDefault.OrDefault();
				Console.WriteLine(actualNamedConstant);
				ReferenceEquals(actualNamedConstant, TestNamedConstantWithDefault.Foo).ShouldBeTrue();
			}

			[Test]
			public void Should_return_the_input_if_it_is_non_null()
			{
				var namedConstantWithoutDefault = TestNamedConstantWithoutDefault.Foo;

				var actualNamedConstant = namedConstantWithoutDefault.OrDefault();
				ReferenceEquals(actualNamedConstant, namedConstantWithoutDefault).ShouldBeTrue();
			}

			[Test]
			public void Should_throw_an_argument_exception_if_the_input_is_null()
			{
				const TestNamedConstantWithoutDefault namedConstantWithoutDefault = null;

				namedConstantWithoutDefault.ShouldThrowAnException(x => x.OrDefault()).OfType<ArgumentException>();
			}

			public class TestNamedConstantWithDefault : NamedConstant<TestNamedConstantWithDefault>
			{
				public static readonly TestNamedConstantWithDefault Bar = new TestNamedConstantWithDefault();

				[DefaultKey]
				public static readonly TestNamedConstantWithDefault Foo = new TestNamedConstantWithDefault();
			}

			public class TestNamedConstantWithoutDefault : NamedConstant<TestNamedConstantWithoutDefault>
			{
				public static readonly TestNamedConstantWithoutDefault Bar = new TestNamedConstantWithoutDefault();
				public static readonly TestNamedConstantWithoutDefault Foo = new TestNamedConstantWithoutDefault();
			}
		}
	}
}