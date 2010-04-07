using System;

using FluentAssert;

using MvbaCore;
using MvbaCore.Extensions;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	public class NamedConstantExtensionsTests
	{
		public class TestNamedConstantWithDefault : NamedConstant<TestNamedConstantWithDefault>
		{
			public static readonly TestNamedConstantWithDefault Bar = new TestNamedConstantWithDefault("bar");

			[DefaultKey]
			public static readonly TestNamedConstantWithDefault Foo = new TestNamedConstantWithDefault("foo");

			public TestNamedConstantWithDefault(string key)
			{
				base.Add(key, this);
			}
		}

		public class TestNamedConstantWithoutDefault : NamedConstant<TestNamedConstantWithoutDefault>
		{
			public static readonly TestNamedConstantWithoutDefault Bar = new TestNamedConstantWithoutDefault("bar");
			public static readonly TestNamedConstantWithoutDefault Foo = new TestNamedConstantWithoutDefault("foo");

			public TestNamedConstantWithoutDefault(string key)
			{
				base.Add(key, this);
			}
		}

		public class TestNamedConstantNotInstantiated : NamedConstant<TestNamedConstantNotInstantiated>
		{
			public static readonly TestNamedConstantNotInstantiated Foo = new TestNamedConstantNotInstantiated("foo");

			public TestNamedConstantNotInstantiated(string key)
			{
				base.Add(key, this);
			}
		}

		[TestFixture]
		public class When_asked_to_get_a_NamedConstant_for_a_specific_key
		{
			[Test]
			public void Should_get_null_given_a_non_existent_key_if_a_default_is_not_defined()
			{
				var result = NamedConstant<TestNamedConstantWithoutDefault>.GetFor("notthere");
				result.ShouldBeNull();
			}

			[Test]
			public void Should_get_the_correct_instance_given_an_existing_key()
			{
				var expected = TestNamedConstantWithDefault.Foo;
				var result = NamedConstant<TestNamedConstantWithDefault>.GetFor(expected.Key);
				result.ShouldBeEqualTo(expected);
			}

			[Test]
			public void Should_get_the_default_instance_given_a_non_existent_key_if_a_default_is_defined()
			{
				const TestNamedConstantWithDefault namedConstantWithDefault = null;
				var expected = namedConstantWithDefault.OrDefault();
				var result = NamedConstant<TestNamedConstantWithDefault>.GetFor(expected.Key + "x");
				result.ShouldBeEqualTo(expected);
			}

			[Test]
			public void Should_Get_the_correct_instance_given_an_existing_key_for_a_type_that_has_not_been_instantiated()
			{
// ReSharper disable AccessToStaticMemberViaDerivedType
				var result = TestNamedConstantNotInstantiated.GetFor("foo");
// ReSharper restore AccessToStaticMemberViaDerivedType
				result.ShouldNotBeNull();
			}


		}

		[TestFixture]
		public class When_asked_to_get_the_value_for_a_NamedConstant_or_its_default
		{
			[Test]
			public void Should_return_null_if_the_input_is_null()
			{
				const TestNamedConstantWithoutDefault namedConstantWithoutDefault = null;

				namedConstantWithoutDefault.OrDefault().ShouldBeNull();
			}

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
		}
	}
}