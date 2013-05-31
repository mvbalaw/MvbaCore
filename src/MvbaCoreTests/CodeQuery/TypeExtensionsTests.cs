//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System;
using System.Collections.Generic;

using FluentAssert;

using MvbaCore.CodeQuery;

using NUnit.Framework;

// ReSharper disable CheckNamespace
namespace CodeQueryTests
// ReSharper restore CheckNamespace
{
	public class TypeExtensionsTests
	{
		[TestFixture]
		public class When_asked_if_a_type_is_generic_and_assignable_from_a_specific_type
		{
			[Test]
			public void Should_return_false_if_the_target_has_more_than_one_generic_type_specifier()
			{
				bool result = typeof(Dictionary<,>).IsGenericAssignableFrom(typeof(int));
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_false_if_the_target_has_one_generic_type_specifier_but_not_assignable_from_the_source_type()
			{
				bool result = typeof(HashSet<>).IsGenericAssignableFrom(typeof(int));
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_false_if_the_target_is_Nullable_and_its_generic_type_is_assignable_from_the_source_type()
			{
				bool result = typeof(int?).IsGenericAssignableFrom(typeof(int));
				result.ShouldBeTrue();
			}

			[Test]
			public void Should_return_false_if_the_target_is_Nullable_but_its_generic_type_is_not_assignable_from_the_source_type()
			{
				bool result = typeof(int?).IsGenericAssignableFrom(typeof(Guid?));
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_false_if_the_target_is_generic_but_not_directly_assignable_from_the_source_type_and_not_Nullable()
			{
				bool result = typeof(IEnumerable<>).IsGenericAssignableFrom(typeof(int));
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_false_if_the_target_is_not_a_generic_type()
			{
				bool result = typeof(int).IsGenericAssignableFrom(typeof(int));
				result.ShouldBeFalse();
			}

			[Test]
			public void Should_return_true_if_the_target_is_generic_and_directly_assignable_from_the_source_type()
			{
				bool result = typeof(IEnumerable<int>).IsGenericAssignableFrom(typeof(HashSet<int>));
				result.ShouldBeTrue();
			}
		}
	}
}