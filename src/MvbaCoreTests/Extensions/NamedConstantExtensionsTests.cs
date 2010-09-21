using System;

using FluentAssert;

using MvbaCore;
using MvbaCore.Extensions;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	public class NamedConstantExtensionsTests
	{
		public class TestNamedConstantNotInstantiated : NamedConstant<TestNamedConstantNotInstantiated>
		{
			public static readonly TestNamedConstantNotInstantiated Foo = new TestNamedConstantNotInstantiated("foo");

			public TestNamedConstantNotInstantiated(string key)
			{
				Add(key, this);
			}
		}

		public class TestNamedConstantWithDefault : NamedConstant<TestNamedConstantWithDefault>
		{
			public static readonly TestNamedConstantWithDefault Bar = new TestNamedConstantWithDefault("bar");

			[DefaultKey]
			public static readonly TestNamedConstantWithDefault Foo = new TestNamedConstantWithDefault("foo");

			public TestNamedConstantWithDefault(string key)
			{
				Add(key, this);
			}
		}

		public class TestNamedConstantWithoutDefault : NamedConstant<TestNamedConstantWithoutDefault>
		{
			public static readonly TestNamedConstantWithoutDefault Bar = new TestNamedConstantWithoutDefault("bar");
			public static readonly TestNamedConstantWithoutDefault Foo = new TestNamedConstantWithoutDefault("foo");

			public TestNamedConstantWithoutDefault(string key)
			{
				Add(key, this);
			}
		}

		[TestFixture]
		public class When_asked_to_get_a_NamedConstant_for_a_specific_key
		{
			private object _expected;
			private Func<string, object> _getFor;
			private string _key;
			private object _result;

			[Test]
			public void Given_a_key_for_which_a_NamedConstant_has_been_defined()
			{
				Test.Verify(
					with_a_key_that_exists_for_the_requested_NamedConstant,
					with_a_NamedConstant_that_does_not_have_a_default_value,
					when_asked_to_get_the_NamedConstant_for_the_key,
					should_not_return_null,
					should_get_the_correct_instance
					);
			}

			[Test]
			public void Given_a_key_for_which_a_NamedConstant_has_not_been_defined_and_there_is_a_default_defined()
			{
				Test.Verify(
					with_a_key_that_does_not_exist_for_the_requested_NamedConstant,
					with_a_NamedConstant_that_has_a_default_value,
					when_asked_to_get_the_NamedConstant_for_the_key,
					should_not_return_null,
					should_get_the_default_instance
					);
			}

			[Test]
			public void Given_a_key_for_which_a_NamedConstant_has_not_been_defined_and_there_is_no_default_defined()
			{
				Test.Verify(
					with_a_key_that_does_not_exist_for_the_requested_NamedConstant,
					with_a_NamedConstant_that_does_not_have_a_default_value,
					when_asked_to_get_the_NamedConstant_for_the_key,
					should_return_null
					);
			}

			private void should_get_the_correct_instance()
			{
				_result.ShouldBeSameInstanceAs(_expected);
			}

			private void should_get_the_default_instance()
			{
				_result.ShouldBeEqualTo(TestNamedConstantWithDefault.Foo);
			}

			private void should_not_return_null()
			{
				_result.ShouldNotBeNull();
			}

			private void should_return_null()
			{
				_result.ShouldBeNull();
			}

			private void when_asked_to_get_the_NamedConstant_for_the_key()
			{
				_result = _getFor(_key);
			}

			private void with_a_NamedConstant_that_does_not_have_a_default_value()
			{
				_expected = TestNamedConstantWithoutDefault.GetFor(_key);
				_getFor = key => NamedConstant<TestNamedConstantWithoutDefault>.GetFor(key);
			}

			private void with_a_NamedConstant_that_has_a_default_value()
			{
				_expected = TestNamedConstantWithDefault.GetDefault();
				_getFor = key => NamedConstant<TestNamedConstantWithDefault>.GetDefault();
			}

			private void with_a_key_that_does_not_exist_for_the_requested_NamedConstant()
			{
				_key = "notthere";
			}

			private void with_a_key_that_exists_for_the_requested_NamedConstant()
			{
				_key = "foo";
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