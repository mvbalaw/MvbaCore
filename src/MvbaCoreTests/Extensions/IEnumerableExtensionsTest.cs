using System.Collections.Generic;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace MvbaCoreTests.Extensions
{
	public class IEnumerableExtensionsTest
	{
		[TestFixture]
		public class When_asked_to_get_the_max_number_in_the_sequence
		{
			private const int Default = 0;
			private List<int> _list;
			private int _result;

			[Test]
			public void Given_a_list()
			{
				Test.Static()
					.When(given_a_list)
					.Should(return_the_maximum_number_in_the_list)
					.Verify();
			}

			[Test]
			public void Given_a_null_list()
			{
				Test.Static()
					.When(given_null_list)
					.Should(return_default)
					.Verify();
			}

			[Test]
			public void Given_an_empty_list()
			{
				Test.Static()
					.When(given_empty_list)
					.Should(return_default)
					.Verify();
			}

			private void given_a_list()
			{
				_list = new List<int>
					{
						1,
						5,
						8
					};
				_result = _list.MaxWithDefault(x => x, Default);
			}

			private void given_empty_list()
			{
				_list = new List<int>();
				_result = _list.MaxWithDefault(x => x, Default);
			}

			private void given_null_list()
			{
				_list = null;
				_result = _list.MaxWithDefault(x => x, Default);
			}

			private void return_default()
			{
				_result.ShouldBeEqualTo(Default);
			}

			private void return_the_maximum_number_in_the_list()
			{
				_result.ShouldBeEqualTo(8);
			}
		}
	}
}