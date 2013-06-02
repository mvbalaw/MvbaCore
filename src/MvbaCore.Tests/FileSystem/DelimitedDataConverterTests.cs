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
using System.Linq;

using FluentAssert;

using MvbaCore.FileSystem;

using NUnit.Framework;

namespace MvbaCore.Tests.FileSystem
{
	[TestFixture]
	public class DelimitedDataConverterTests
	{
		[Test]
		public void Given_a_quoted_item()
		{
			const string input = "a,\"b\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("b");
		}

		[Test]
		public void Given_a_quoted_item_containing_an_escaped_quote_followed_by_the_delimiter()
		{
			const string input = "a,\"b\"\",b\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("b\",b");
		}

		[Test]
		public void Given_a_quoted_item_containing_the_delimiter()
		{
			const string input = "a,\"b,b\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("b,b");
		}

		[Test]
		public void Given_a_quoted_item_containing_the_delimiter_followed_by_an_escaped_quote()
		{
			const string input = "a,\"b,\"\"b\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("b,\"b");
		}

		[Test]
		public void Given_a_quoted_item_ending_with_an_escaped_quote()
		{
			//"\"JENKINS A C \"\"CY\"\"\""
			const string input = "a,\"b\"\"\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("b\"");
		}

		[Test]
		public void Given_a_quoted_item_ending_with_the_delimiter()
		{
			const string input = "a,\"b,\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("b,");
		}

		[Test]
		public void Given_a_quoted_item_starting_with_an_escaped_quote()
		{
			const string input = "a,\"\"\"b\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("\"b");
		}

		[Test]
		public void Given_a_quoted_item_starting_with_the_delimiter()
		{
			const string input = "a,\",b\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo(",b");
		}

		[Test]
		public void Given_an_empty_quoted_item()
		{
			const string input = "a,\"\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("");
		}

		[Test]
		public void Given_an_item_ending_with_an_escaped_quote()
		{
			const string input = "a,b\"\",c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("b\"");
		}

		[Test]
		public void Given_an_item_starting_with_an_escaped_quote()
		{
			const string input = "a,\"\"b,c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("\"b");
		}

		[Test]
		public void Given_an_item_with_an_embedded_escaped_quote()
		{
			const string input = "a,b\"\"b,c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
			result[1].ShouldBeEqualTo("b\"b");
		}

		[Test]
		public void Given_string_with_only_non_quoted_items()
		{
			const string input = "a,b,c";
			const string delimiter = ",";
			var result = DelimitedDataConverter.JoinQuoted(input.Split(new[] { delimiter }, StringSplitOptions.None), delimiter).ToList();
			result.Count.ShouldBeEqualTo(3);
		}
	}
}