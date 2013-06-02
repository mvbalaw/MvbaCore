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
using System.Linq;

using FluentAssert;

using JetBrains.Annotations;

using MvbaCore.Extensions;
using MvbaCore.FileSystem;

using NUnit.Framework;

namespace MvbaCore.Tests.FileSystem
{
	[UsedImplicitly]
	public class QuotedCommaDelimitedDataConverterTests
	{
		[TestFixture]
		public class When_asked_to_convert_an_array_of_lines_to_a_Dictionary
		{
			private string[] _input;
			private IQuotedCommaDelimitedDataConverter _quotedCommaDelimitedDataConverter;

			[SetUp]
			public void BeforeEachTest()
			{
				_input = new[]
					         {
						         "\"H1\",\"H2\",\"H3\",\"H4\"",
						         "\"Obama\",\"Nobel\",\"Peace\",\"2009\"",
						         "\"Bush\",\"No\",\"Nobel\",\"\"",
						         "\"Gandhi\",\"Deserved\",\"Nobel\",\"\""
					         };

				_quotedCommaDelimitedDataConverter = new QuotedCommaDelimitedDataConverter();
			}

			[Test]
			public void Should_have_Dictionary_Key_Value_pair_for_each_header_data_combination_for_each_line()
			{
				var result = _quotedCommaDelimitedDataConverter.Convert(_input);

				var header = SplitOnDelimiter(_input[0]);
				int count = 0;
				foreach (var actual in result)
				{
					var expected = SplitOnDelimiter(_input[count + 1]);
					header.Length.Times(j => { actual[header[j]].ShouldBeEqualTo(expected[j]); });
					count++;
				}
			}

			[Test]
			public void Should_return_a_list_of_Dictionary_with_keys_populated_using_the_first_line_of_the_input()
			{
				var result = _quotedCommaDelimitedDataConverter.Convert(_input);
				result.First().Keys.ToList().ShouldBeEqualTo(SplitOnDelimiter(_input.First()).ToList());
			}

			[Test]
			public void Should_return_one_Dictionary_item_for_each_line_excluding_the_header_of_the_input()
			{
				var result = _quotedCommaDelimitedDataConverter.Convert(_input);
				result.Count().ShouldBeEqualTo(_input.Skip(1).Count());
			}

			private static string[] SplitOnDelimiter(string input)
			{
				return input.Substring(1, input.Length - 2).Split(new[] { "\",\"" }, StringSplitOptions.None);
			}
		}
	}
}