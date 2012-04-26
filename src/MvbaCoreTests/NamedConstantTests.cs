using System.Linq;

using FluentAssert;

using JetBrains.Annotations;

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	[UsedImplicitly]
	public class NamedConstantTests
	{
		public class DuplicateValue : NamedConstant<DuplicateValue>
		{
			public static readonly DuplicateValue Item = new DuplicateValue("k", "Desc");

			private DuplicateValue(string key, string desc)
			{
				Add(key, this);
				Add(desc, this);
			}
		}

		[TestFixture]
		public class When_asked_to_GetAll
		{
			[Test]
			public void Given_a_named_constant_where_multiple_keys_are_associated_with_the_same_value_should_return_only_one_instance_of_each_value()
			{
				DuplicateValue.GetAll().Count().ShouldBeEqualTo(1);
			}
		}
	}
}