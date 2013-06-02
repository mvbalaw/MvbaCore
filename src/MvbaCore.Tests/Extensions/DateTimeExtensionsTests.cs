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

using MvbaCore.Extensions;

using NUnit.Framework;

namespace MvbaCore.Tests.Extensions
{
	[UsedImplicitly]
	public class DateTimeExtensionsTests
	{
		[TestFixture]
		public class When_asked_to_get_the_number_of_months_since_other_date
		{
			private DateTime _currentDate;
			private int _expected;
			private DateTime _otherDate;
			private int _result;

			[SetUp]
			public void BeforeEachTest()
			{
				_currentDate = new DateTime(2010, 9, 3);
				_expected = -1;
			}

			[Test]
			public void Given_current_and_other_are_the_same_day()
			{
				_expected = 1;

				Test.Verify(
					with_other_date_same_as_current_date,
					when_asked_to_get_the_number_of_months_since_other_date,
					should_return_expected_number
					);
			}

			[Test]
			public void Given_other_earlier_in_same_month_as_current_date()
			{
				_expected = 1;

				Test.Verify(
					with_other_date_earlier_in_same_month_as_current_date,
					when_asked_to_get_the_number_of_months_since_other_date,
					should_return_expected_number
					);
			}

			[Test]
			public void Given_other_in_previous_year_from_current_year()
			{
				_expected = 10;

				Test.Verify(
					with_other_date_in_previous_year_from_current_year,
					when_asked_to_get_the_number_of_months_since_other_date,
					should_return_expected_number
					);
			}

			[Test]
			public void Given_other_is_later_than_current_date()
			{
				_expected = 0;

				Test.Verify(
					with_other_date_later_than_current_date,
					when_asked_to_get_the_number_of_months_since_other_date,
					should_return_expected_number
					);
			}

			[Test]
			public void Given_other_is_one_month_before_current()
			{
				_expected = 2;

				Test.Verify(
					with_other_date_one_month_before_current_date,
					when_asked_to_get_the_number_of_months_since_other_date,
					should_return_expected_number
					);
			}

			[Test]
			public void Given_other_more_than_year_from_before_current_year()
			{
				_expected = 22;

				Test.Verify(
					with_other_date_more_than_year_from_current_year,
					when_asked_to_get_the_number_of_months_since_other_date,
					should_return_expected_number
					);
			}

			private void should_return_expected_number()
			{
				_result.ShouldBeEqualTo(_expected);
			}

			private void when_asked_to_get_the_number_of_months_since_other_date()
			{
				_result = _currentDate.MonthsSince(_otherDate);
			}

			private void with_other_date_earlier_in_same_month_as_current_date()
			{
				if (_currentDate.Day == 1)
				{
					_currentDate.AddDays(1);
				}
				_otherDate = _currentDate.AddDays(-1);
			}

			private void with_other_date_in_previous_year_from_current_year()
			{
				_otherDate = _currentDate.AddMonths(-_currentDate.Month);
			}

			private void with_other_date_later_than_current_date()
			{
				_otherDate = _currentDate.AddDays(1);
			}

			private void with_other_date_more_than_year_from_current_year()
			{
				_otherDate = _currentDate.AddMonths(-_currentDate.Month - 12);
			}

			private void with_other_date_one_month_before_current_date()
			{
				_otherDate = _currentDate.AddMonths(-1);
			}

			private void with_other_date_same_as_current_date()
			{
				_otherDate = _currentDate;
			}
		}
	}
}