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

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public class NotificationMessageTest
	{
		[TestFixture]
		public class When_asked_for_its_Message
		{
			[Test]
			public void Should_return_the_formatted_value_that_was_passed_in_the_constructor_if_a_format_string_was_used()
			{
				const string format = "test {0} now {1}";
				const int arg0 = 1;
				const string arg1 = "help";
				var message = new NotificationMessage(NotificationSeverity.Warning, format, arg0, arg1);
				Assert.AreEqual(String.Format(format, arg0, arg1), message.Message);
			}

			[Test]
			public void Should_return_the_value_that_was_passed_in_the_constructor()
			{
				const string test = "test";
				var message = new NotificationMessage(NotificationSeverity.Warning, test);
				Assert.AreEqual(test, message.Message);
			}
		}

		[TestFixture]
		public class When_asked_for_its_Severity
		{
			[Test]
			public void Should_return_the_value_that_was_passed_in_the_constructor()
			{
				const NotificationSeverity severity = NotificationSeverity.Warning;
				var message = new NotificationMessage(severity, "");
				Assert.AreEqual(severity, message.Severity);
			}
		}
	}
}