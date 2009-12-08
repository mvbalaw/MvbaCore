using System.Linq;

using FluentAssert;

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTest
	{
		[TestFixture]
		public class When_asked_for_a_Notification_that_has_an_initial_message_with_Severity_of_Info
		{
			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_Info_Severity()
			{
				Notification notification = Notification.InfoFor("text");

				notification.Messages.Count.ShouldBeEqualTo(1);
				notification.Messages.First().Severity.ShouldBeEqualTo(NotificationSeverity.Info);
			}

			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_the_given_messageText()
			{
				const string messageText = "text";
				Notification notification = Notification.InfoFor(messageText);

				notification.Messages.Count.ShouldBeEqualTo(1);
				notification.Messages.First().Message.ShouldBeEqualTo(messageText);
			}
		}
	}
}