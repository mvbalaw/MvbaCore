using System.Linq;

using FluentAssert;

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTest
	{
		[TestFixture]
		public class When_asked_For_a_Notification
		{
			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_the_given_Severity()
			{
				const NotificationSeverity severity = NotificationSeverity.Warning;
				var notification = Notification.For(severity, "text");

				notification.Messages.Count.ShouldBeEqualTo(1);
				notification.Messages.First().Severity.ShouldBeEqualTo(severity);
			}

			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_the_given_messageText()
			{
				const string messageText = "text";
				var notification = Notification.For(NotificationSeverity.Warning, messageText);

				notification.Messages.Count.ShouldBeEqualTo(1);
				notification.Messages.First().Message.ShouldBeEqualTo(messageText);
			}
		}
	}
}