using System;
using System.ComponentModel;
using System.Linq;

using CodeQuery;

using FluentAssert;

using NUnit.Framework;

namespace CodeQueryTests
{
	public class PropertyInfoExtensionsTests
	{
		[TestFixture]
		public class When_asked_if_there_are_any_custom_Attributes_of_a_specific_type_for_a_PropertyInfo
		{
			[Test]
			public void Should_return_False_if_there_are_no_matching_attributes()
			{
				bool hasAttributeOfType = typeof(TestClass).GetProperty("Id").HasAttributeOfType<TestAttribute>();
				hasAttributeOfType.ShouldBeFalse();
			}

			[Test]
			public void Should_return_True_if_there_are_matching_attributes()
			{
				bool hasAttributeOfType = typeof(TestClass).GetProperty("Id").HasAttributeOfType<ReadOnlyAttribute>();
				hasAttributeOfType.ShouldBeTrue();
			}

			public class TestClass
			{
				[ReadOnly(true)]
				public int Id { get; private set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_PropertyInfos_that_have_a_getter
		{
			[Test]
			public void Should_get_an_empty_container_if_there_are_no_matching_PropertyInfos()
			{
				var propertyInfos = typeof(NoGettersTestClass).GetProperties().ThatHaveAGetter();
				propertyInfos.ShouldNotBeNull();
				propertyInfos.Count().ShouldBeEqualTo(0);
			}

			[Test]
			public void Should_get_the_matching_PropertyInfos()
			{
				var propertyInfos = typeof(TestClass).GetProperties().ThatHaveAGetter();
				propertyInfos.Count().ShouldBeEqualTo(1);
				propertyInfos.First().Name.ShouldBeEqualTo("Id");
			}

			public class NoGettersTestClass
			{
				public int Id
				{
					set { throw new NotImplementedException(); }
				}
			}

			public class TestClass
			{
				public int Id
				{
					get { throw new NotImplementedException(); }
				}

				private string Name { get; set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_PropertyInfos_that_have_a_setter
		{
			[Test]
			public void Should_get_an_empty_container_if_there_are_no_matching_PropertyInfos()
			{
				var propertyInfos = typeof(NoSettersTestClass).GetProperties().ThatHaveASetter();
				propertyInfos.ShouldNotBeNull();
				propertyInfos.Count().ShouldBeEqualTo(0);
			}

			[Test]
			public void Should_get_the_matching_PropertyInfos()
			{
				var propertyInfos = typeof(TestClass).GetProperties().ThatHaveASetter();
				propertyInfos.Count().ShouldBeEqualTo(1);
				propertyInfos.First().Name.ShouldBeEqualTo("Id");
			}

			public class NoSettersTestClass
			{
				public int Id
				{
					get { throw new NotImplementedException(); }
				}
			}

			public class TestClass
			{
				public int Id
				{
					set { throw new NotImplementedException(); }
				}

				private string Name { get; set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_PropertyInfos_that_have_custom_Attributes_of_a_specific_type
		{
			[Test]
			public void Should_get_an_empty_container_if_there_are_no_matching_PropertyInfos()
			{
				var propertyInfos = typeof(TestClass).GetProperties().WithAttributeOfType<TestAttribute>();
				propertyInfos.ShouldNotBeNull();
				propertyInfos.Count().ShouldBeEqualTo(0);
			}

			[Test]
			public void Should_get_the_matching_PropertyInfos()
			{
				var propertyInfos = typeof(TestClass).GetProperties().WithAttributeOfType<ReadOnlyAttribute>();
				propertyInfos.Count().ShouldBeEqualTo(1);
				propertyInfos.First().Name.ShouldBeEqualTo("Id");
			}

			public class TestClass
			{
				[ReadOnly(true)]
				public int Id { get; private set; }

				public string Name { get; set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_custom_Attributes_of_a_specific_type_for_a_PropertyInfo
		{
			[Test]
			public void Should_get_the_matching_Attributes()
			{
				var attributes = typeof(TestClass).GetProperty("Id").CustomAttributesOfType<ReadOnlyAttribute>();
				attributes.Count().ShouldBeEqualTo(1);
			}

			[Test]
			public void Should_return_an_empty_container_if_there_are_no_matching_Attributes()
			{
				var attributes = typeof(TestClass).GetProperty("Id").CustomAttributesOfType<TestAttribute>();
				attributes.ShouldNotBeNull();
				attributes.Count().ShouldBeEqualTo(0);
			}

			public class TestClass
			{
				[ReadOnly(true)]
				public int Id { get; private set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_the_custom_Attributes_for_a_PropertyInfo
		{
			[Test]
			public void Should_get_the_Attributes()
			{
				var attributes = typeof(TestClass).GetProperty("Id").CustomAttributes();
				attributes.Count().ShouldBeEqualTo(1);
				attributes.First().ShouldBeOfType<ReadOnlyAttribute>();
			}

			public class TestClass
			{
				[ReadOnly(true)]
				public int Id { get; private set; }
			}
		}
	}
}