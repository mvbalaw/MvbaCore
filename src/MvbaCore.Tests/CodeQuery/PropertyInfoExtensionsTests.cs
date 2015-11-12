//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.ComponentModel;
using System.Linq;

using FluentAssert;

using JetBrains.Annotations;

using MvbaCore.CodeQuery;

using NUnit.Framework;

namespace MvbaCore.Tests.CodeQuery
{
	[UsedImplicitly]
	public class PropertyInfoExtensionsTests
	{
		[TestFixture]
		public class When_asked_if_there_are_any_custom_Attributes_of_a_specific_type_for_a_PropertyInfo
		{
			[Test]
			public void Should_return_False_if_there_are_no_matching_attributes()
			{
				var hasAttributeOfType = typeof(TestClass).GetProperty("Id").HasAttributeOfType<TestAttribute>();
				hasAttributeOfType.ShouldBeFalse();
			}

			[Test]
			public void Should_return_True_if_there_are_matching_attributes()
			{
				var hasAttributeOfType = typeof(TestClass).GetProperty("Id").HasAttributeOfType<ReadOnlyAttribute>();
				hasAttributeOfType.ShouldBeTrue();
			}

			public class TestClass
			{
				[ReadOnly(true)]
// ReSharper disable once UnusedAutoPropertyAccessor.Local
				public int Id { get; private set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_PropertyInfos_that_have_a_getter
		{
			[Test]
			public void Should_get_an_empty_container_if_there_are_no_matching_PropertyInfos()
			{
				var propertyInfos = typeof(NoGettersTestClass).GetProperties().ThatHaveAGetter().ToList();
				propertyInfos.ShouldNotBeNull();
				propertyInfos.Count.ShouldBeEqualTo(0);
			}

			[Test]
			public void Should_get_the_matching_PropertyInfos()
			{
				var propertyInfos = typeof(TestClass).GetProperties().ThatHaveAGetter().ToList();
				propertyInfos.Count.ShouldBeEqualTo(1);
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

// ReSharper disable once UnusedMember.Local
				private string Name { get; set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_PropertyInfos_that_have_a_setter
		{
			[Test]
			public void Should_get_an_empty_container_if_there_are_no_matching_PropertyInfos()
			{
				var propertyInfos = typeof(NoSettersTestClass).GetProperties().ThatHaveASetter().ToList();
				propertyInfos.ShouldNotBeNull();
				propertyInfos.Count.ShouldBeEqualTo(0);
			}

			[Test]
			public void Should_get_the_matching_PropertyInfos()
			{
				var propertyInfos = typeof(TestClass).GetProperties().ThatHaveASetter().ToList();
				propertyInfos.Count.ShouldBeEqualTo(1);
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

// ReSharper disable once UnusedMember.Local
				private string Name { get; set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_PropertyInfos_that_have_custom_Attributes_of_a_specific_type
		{
			[Test]
			public void Should_get_an_empty_container_if_there_are_no_matching_PropertyInfos()
			{
				var propertyInfos = typeof(TestClass).GetProperties().WithAttributeOfType<TestAttribute>().ToList();
				propertyInfos.ShouldNotBeNull();
				propertyInfos.Count.ShouldBeEqualTo(0);
			}

			[Test]
			public void Should_get_the_matching_PropertyInfos()
			{
				var propertyInfos = typeof(TestClass).GetProperties().WithAttributeOfType<ReadOnlyAttribute>().ToList();
				propertyInfos.Count.ShouldBeEqualTo(1);
				propertyInfos.First().Name.ShouldBeEqualTo("Id");
			}

			public class TestClass
			{
				[ReadOnly(true)]
// ReSharper disable once UnusedAutoPropertyAccessor.Local
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
				var attributes = typeof(TestClass).GetProperty("Id").CustomAttributesOfType<TestAttribute>().ToList();
				attributes.ShouldNotBeNull();
				attributes.Count.ShouldBeEqualTo(0);
			}

			public class TestClass
			{
				[ReadOnly(true)]
// ReSharper disable once UnusedAutoPropertyAccessor.Local
				public int Id { get; private set; }
			}
		}

		[TestFixture]
		public class When_asked_to_get_the_custom_Attributes_for_a_PropertyInfo
		{
			[Test]
			public void Should_get_the_Attributes()
			{
				var attributes = typeof(TestClass).GetProperty("Id").CustomAttributes().ToList();
				attributes.Count.ShouldBeEqualTo(1);
				attributes.First().ShouldBeOfType<ReadOnlyAttribute>();
			}

			public class TestClass
			{
				[ReadOnly(true)]
// ReSharper disable once UnusedAutoPropertyAccessor.Local
				public int Id { get; private set; }
			}
		}
	}
}