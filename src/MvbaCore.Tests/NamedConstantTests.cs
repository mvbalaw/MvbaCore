//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System.Linq;

using FluentAssert;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCore.Tests
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