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
using System.Globalization;

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

		// SystemService used to add one UTC offset, worked out when the process first used it, to the UTC clock for as
		// long as the process ran, so a service nobody restarted across a daylight saving time change answered "now" an
		// hour off from then on. Each fixture below creates the clock BEFORE the change and reads it after, which is the
		// state such a process is in.
		[TestFixture]
		public class When_asked_for_the_current_DateTime_after_a_daylight_saving_change
		{
			private static readonly TimeZoneInfo Central = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

			private DateTime _utcNow;

			private SystemService CreatedAt(string utc)
			{
				_utcNow = Utc(utc);
				var clock = new SystemService(() => _utcNow, () => Central);
				// a first read before the change, so anything worked out on first use is worked out with the old offset
				clock.CurrentDateTime.ShouldBeEqualTo(TimeZoneInfo.ConvertTimeFromUtc(_utcNow, Central));
				return clock;
			}

			[Test]
			public void Given_the_fall_change__it_should_answer_standard_time_rather_than_an_hour_late()
			{
				var clock = CreatedAt("2026-10-31T12:00:00Z");

				_utcNow = Utc("2026-11-02T05:30:00Z");

				clock.CurrentDateTime.ShouldBeEqualTo(new DateTime(2026, 11, 1, 23, 30, 0),
					"11:30 PM Sunday, UTC-6 - the offset from before the change answers 12:30 AM Monday");
			}

			[Test]
			public void Given_the_first_hour_after_the_spring_change__it_should_answer_a_time_that_exists()
			{
				var clock = CreatedAt("2027-03-12T18:00:00Z");

				_utcNow = Utc("2027-03-14T08:30:00Z");

				clock.CurrentDateTime.ShouldBeEqualTo(new DateTime(2027, 3, 14, 3, 30, 0),
					"3:30 AM, UTC-5 - the offset from before the change answers 2:30 AM, an hour the clocks skipped");
			}

			[Test]
			public void Given_later_on_the_day_of_the_spring_change__it_should_answer_daylight_time_rather_than_an_hour_early()
			{
				var clock = CreatedAt("2027-03-12T18:00:00Z");

				_utcNow = Utc("2027-03-14T12:00:00Z");

				clock.CurrentDateTime.ShouldBeEqualTo(new DateTime(2027, 3, 14, 7, 0, 0),
					"7:00 AM, UTC-5 - the offset from before the change answers 6:00 AM");
			}
		}

		// Callers store and compare "now" as UTC, so its answer has to convert back to the instant it was read at. These
		// convert in this machine's own time zone, as callers do, so they hold in any zone; in a zone with daylight saving
		// time they fail against an offset worked out before the change, the spring case by throwing "The supplied
		// DateTime represents an invalid time".
		[TestFixture]
		public class When_its_answer_is_converted_back_to_UTC
		{
			[TestCase("2026-10-31T12:00:00Z", "2026-11-01T06:30:00Z", Description = "fall change: the first 1:30 AM, still daylight time")]
			[TestCase("2026-10-31T12:00:00Z", "2026-11-01T07:30:00Z", Description = "fall change: the second 1:30 AM, standard time")]
			[TestCase("2026-10-31T12:00:00Z", "2026-11-02T05:30:00Z", Description = "the evening after the fall change")]
			[TestCase("2027-03-12T18:00:00Z", "2027-03-14T08:30:00Z", Description = "spring change: the first hour after 2 AM")]
			[TestCase("2027-03-12T18:00:00Z", "2027-03-14T12:00:00Z", Description = "the morning of the spring change")]
			public void Should_land_on_the_instant_it_was_read_at(string createdAtUtc, string readAtUtc)
			{
				var utcNow = Utc(createdAtUtc);
				var clock = new SystemService(() => utcNow, () => TimeZoneInfo.Local);
				TimeZoneInfo.ConvertTimeToUtc(clock.CurrentDateTime).ShouldBeEqualTo(utcNow);

				utcNow = Utc(readAtUtc);
				var result = clock.CurrentDateTime;

				result.Kind.ShouldBeEqualTo(DateTimeKind.Local);
				TimeZoneInfo.ConvertTimeToUtc(result).ShouldBeEqualTo(utcNow);
			}
		}

		[TestFixture]
		public class When_an_IoC_container_picks_a_constructor
		{
			[Test]
			public void Should_only_find_the_parameterless_one()
			{
				// containers build through the public constructor with the most parameters, and cannot supply the clock
				// and time zone the test constructor takes
				var constructors = typeof(SystemService).GetConstructors();

				constructors.Length.ShouldBeEqualTo(1);
				constructors[0].GetParameters().Length.ShouldBeEqualTo(0);
			}
		}

		private static DateTime Utc(string roundTrip)
		{
			return DateTime.Parse(roundTrip, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		}
	}
}