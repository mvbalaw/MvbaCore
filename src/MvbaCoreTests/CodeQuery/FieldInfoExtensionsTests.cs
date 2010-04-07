using System.ComponentModel;
using System.Linq;

using CodeQuery;

using FluentAssert;

using NUnit.Framework;

namespace CodeQueryTests
{
	public class FieldInfoExtensionsTests
	{
		[TestFixture]
		public class When_asked_for_the_static_FieldInfos
		{
			[Test]
			public void Should_return_only_the_static_ones()
			{
				var fields = typeof(TestClass).GetFields();
				fields.Length.ShouldBeEqualTo(2);
				var fieldInfos = fields.ThatAreStatic();
				fieldInfos.Count().ShouldBeEqualTo(1);
			}

			public class TestClass
			{
				public static int Foo = 7;
				public int Bar = 6;
			}
		}

		[TestFixture]
		public class When_asked_if_there_are_any_custom_Attributes_of_a_specific_type_for_a_FieldInfo
		{
			[Test]
			public void Should_return_False_if_there_are_no_matching_attributes()
			{
				var hasAttributeOfType = typeof(TestClass).GetField("Id").HasAttributeOfType<TestAttribute>();
				hasAttributeOfType.ShouldBeFalse();
			}

			[Test]
			public void Should_return_True_if_there_are_matching_attributes()
			{
				var hasAttributeOfType = typeof(TestClass).GetField("Id").HasAttributeOfType<ReadOnlyAttribute>();
				hasAttributeOfType.ShouldBeTrue();
			}

			public class TestClass
			{
				[ReadOnly(true)]
				public int Id;
			}
		}

		[TestFixture]
		public class When_asked_to_get_custom_Attributes_of_a_specific_type_for_a_FieldInfo
		{
			[Test]
			public void Should_get_the_matching_Attributes()
			{
				var attributes = typeof(TestClass).GetField("Id").CustomAttributesOfType<ReadOnlyAttribute>();
				attributes.Count().ShouldBeEqualTo(1);
			}

			[Test]
			public void Should_return_an_empty_container_if_there_are_no_matching_Attributes()
			{
				var attributes = typeof(TestClass).GetField("Id").CustomAttributesOfType<TestAttribute>();
				attributes.ShouldNotBeNull();
				attributes.Count().ShouldBeEqualTo(0);
			}

			public class TestClass
			{
				[ReadOnly(true)]
				public int Id;
			}
		}

		[TestFixture]
		public class When_asked_to_get_FieldInfos_that_have_custom_Attributes_of_a_specific_type
		{
			[Test]
			public void Should_get_an_empty_container_if_there_are_no_matching_Fields()
			{
				var fieldInfos = typeof(TestClass).GetFields().WithAttributeOfType<TestAttribute>();
				fieldInfos.ShouldNotBeNull();
				fieldInfos.Count().ShouldBeEqualTo(0);
			}

			[Test]
			public void Should_get_the_matching_FieldInfos()
			{
				var fieldInfos = typeof(TestClass).GetFields().WithAttributeOfType<ReadOnlyAttribute>();
				fieldInfos.Count().ShouldBeEqualTo(1);
				fieldInfos.First().Name.ShouldBeEqualTo("Id");
			}

			public class TestClass
			{
				[ReadOnly(true)]
				public int Id;

				public string Name;
			}
		}
	}
}