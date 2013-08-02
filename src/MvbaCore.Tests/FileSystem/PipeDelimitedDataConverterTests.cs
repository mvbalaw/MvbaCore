//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.Linq;

using FluentAssert;

using JetBrains.Annotations;

using MvbaCore.Extensions;
using MvbaCore.FileSystem;

using NUnit.Framework;

namespace MvbaCore.Tests.FileSystem
{
	[UsedImplicitly]
	public class PipeDelimitedDataConverterTests
	{
		[TestFixture]
		public class When_asked_to_convert_an_array_of_lines_to_a_Dictionary
		{
			private string[] _input;
			private IPipeDelimitedDataConverter _pipeDelimitedDataConverter;

			[SetUp]
			public void BeforeEachTest()
			{
				_input = new[]
					         {
						         "H1|H2|H3|H4",
						         "Obama|Nobel|Peace|2009",
						         "Bush|No|Nobel|",
						         "Gandhi|Deserved|Nobel|"
					         };

				_pipeDelimitedDataConverter = new PipeDelimitedDataConverter();
			}

			[Test]
			public void Should_have_Dictionary_Key_Value_pair_for_each_header_data_combination_for_each_line()
			{
				var result = _pipeDelimitedDataConverter.Convert(_input);

				var header = _input[0].Split('|');
				var count = 0;
				foreach (var actual in result)
				{
					var expected = _input[count + 1].Split('|');
					var actual1 = actual;
					header.Length.Times(j => actual1[header[j]].ShouldBeEqualTo(expected[j]));
					count++;
				}
			}

			[Test]
			public void Should_return_a_list_of_Dictionary_with_keys_populated_using_the_first_line_of_the_input()
			{
				var result = _pipeDelimitedDataConverter.Convert(_input);
				result.First().Keys.ToList().ShouldBeEqualTo(_input.First().Split('|').ToList());
			}

			[Test]
			public void Should_return_one_Dictionary_item_for_each_line_excluding_the_header_of_the_input()
			{
				var result = _pipeDelimitedDataConverter.Convert(_input);
				result.Count().ShouldBeEqualTo(_input.Skip(1).Count());
			}
		}
	}
}