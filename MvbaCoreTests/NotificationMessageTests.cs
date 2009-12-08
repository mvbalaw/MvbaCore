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
				NotificationMessage message = new NotificationMessage(NotificationSeverity.Warning, format, arg0, arg1);
				Assert.AreEqual(String.Format(format, arg0, arg1), message.Message);
			}

			[Test]
			public void Should_return_the_value_that_was_passed_in_the_constructor()
			{
				const string test = "test";
				NotificationMessage message = new NotificationMessage(NotificationSeverity.Warning, test);
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
				NotificationMessage message = new NotificationMessage(severity, "");
				Assert.AreEqual(severity, message.Severity);
			}
		}
	}
}