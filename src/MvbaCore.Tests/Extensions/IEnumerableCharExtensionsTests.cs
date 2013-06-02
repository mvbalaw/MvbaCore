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
using System.Collections.Generic;

using FluentAssert;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCore.Tests.Extensions
{
	[UsedImplicitly]
	public class IEnumerableCharExtensionsTests
	{
		[TestFixture]
		public class When_AsString_is_called_without_a_separator_argument
		{
			[Test]
			public void Given_a_null_input_should_return_an_empty_string()
			{
				char[] input = null;
				string result = input.AsString();
				result.ShouldBeEqualTo("");
			}

			[Test]
			public void Given_an_empty_input_should_return_an_empty_string()
			{
				var input = new char[] { };
				string result = input.AsString();
				result.ShouldBeEqualTo("");
			}

			[Test]
			public void Given_characters__a_b_c__should_return_the_string__abc()
			{
				var input = new[] { 'a', 'b', 'c' };
				string result = input.AsString();
				result.ShouldBeEqualTo("abc");
			}
		}
		[TestFixture]
		public class When_AsString_is_called_with_a_separator_argument
		{
			[Test]
			public void Given_a_null_input_and_a_null_separator_should_return_an_empty_string()
			{
				char[] input = null;
				string result = input.AsString(null);
				result.ShouldBeEqualTo("");
			}

			[Test, ExpectedException(typeof(ArgumentNullException))]
			public void Given_an_empty_input_and_a_null_separator_should_throw_a_ArgumentNullException()
			{
				var input = new char[] { };
				string result = input.AsString(null);
				result.ShouldBeEqualTo("");
			}

			[Test]
			public void Given_an_empty_input_and_a_non_empty_separator_should_return_an_empty_string()
			{
				var input = new char[] { };
				string result = input.AsString("-");
				result.ShouldBeEqualTo("");
			}

			[Test]
			public void Given_characters__a_b_c___and_single_character_separator__X__should_return_the_string__aXbXc()
			{
				var input = new[] { 'a', 'b', 'c' };
				string result = input.AsString("X");
				result.ShouldBeEqualTo("aXbXc");
			}

			[Test]
			public void Given_characters__a_b_c___and_multi_character_separator___SPACE_X_SPACE__should_return_the_string__a_SPACE_X_SPACE_b_SPACE_X_SPACE_c()
			{
				var input = new[] { 'a', 'b', 'c' };
				string result = input.AsString(" X ");
				result.ShouldBeEqualTo("a X b X c");
			}
		}
	}
}