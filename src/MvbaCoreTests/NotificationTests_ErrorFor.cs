using System.Linq;

using FluentAssert;

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTest
	{
		[TestFixture]
		public class When_asked_for_a_Notification_that_has_an_initial_message_with_Severity_of_Error
		{
			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_Error_Severity()
			{
				var notification = Notification.ErrorFor("text");

				notification.Messages.Count.ShouldBeEqualTo(1);
				notification.Messages.First().Severity.ShouldBeEqualTo(NotificationSeverity.Error);
			}

			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_the_given_messageText()
			{
				const string messageText = "text";
				var notification = Notification.ErrorFor(messageText);

				notification.Messages.Count.ShouldBeEqualTo(1);
				notification.Messages.First().Message.ShouldBeEqualTo(messageText);
			}
		}
	}
}