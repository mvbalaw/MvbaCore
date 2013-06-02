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

using FluentAssert;

using JetBrains.Annotations;

using MvbaCore.Services;

using NUnit.Framework;

namespace MvbaCore.Tests.Services
{
	[UsedImplicitly]
	public class SystemServiceTests
	{
		[TestFixture]
		public class When_asked_for_the_current_DateTime
		{
			private DateTime _current;
			private DateTime _expected;
			private SystemService _systemService;

			[SetUp]
			public void BeforeEachTest()
			{
				_systemService = new SystemService();
				_expected = DateTime.Now;
				_current = _systemService.CurrentDateTime;
			}

			[Test]
			public void Should_return_the_correct_day()
			{
				_current.Day.ShouldBeEqualTo(_expected.Day);
			}

			[Test]
			public void Should_return_the_correct_hour()
			{
				_current.Hour.ShouldBeEqualTo(_expected.Hour);
			}

			[Test]
			public void Should_return_the_correct_kind()
			{
				_current.Kind.ShouldBeEqualTo(_expected.Kind);
			}

			[Test]
			public void Should_return_the_correct_millisecond()
			{
				_current.Millisecond.ShouldBeEqualTo(_expected.Millisecond);
			}

			[Test]
			public void Should_return_the_correct_minute()
			{
				_current.Minute.ShouldBeEqualTo(_expected.Minute);
			}

			[Test]
			public void Should_return_the_correct_month()
			{
				_current.Month.ShouldBeEqualTo(_expected.Month);
			}

			[Test]
			public void Should_return_the_correct_second()
			{
				_current.Second.ShouldBeEqualTo(_expected.Second);
			}

			[Test]
			public void Should_return_the_correct_year()
			{
				_current.Year.ShouldBeEqualTo(_expected.Year);
			}

		}
	}
}