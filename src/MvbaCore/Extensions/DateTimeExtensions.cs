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

namespace MvbaCore.Extensions
{
	public static class DateTimeExtensions
	{
		public static int MonthsSince(this DateTime current, DateTime other)
		{
			if (other > current)
			{
				return 0;
			}

			if (current.Year != other.Year)
			{
				return other.MonthsToEndOfYear() + ((current.YearsSince(other) - 1) * 12) + current.Month;
			}

			var currentMonth = current.Month;
			var otherMonth = other.Month;

			if (otherMonth == currentMonth)
			{
				return 1;
			}

			return currentMonth - otherMonth + 1;
		}

		public static int MonthsToEndOfYear(this DateTime current)
		{
			return (13 - current.Month);
		}

		public static int YearsSince(this DateTime current, DateTime other)
		{
			return current.Year - other.Year;
		}
	}
}