using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssert;

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTests
	{
		[TestFixture]
		public class When_asked_for_its_messages
		{
			[Test]
			[ExpectedException(typeof(NotSupportedException))]
			public void Should_not_be_able_to_change_the_Notification_by_adding_to_the_Messages_object()
			{
				var notification = Notification.Empty;
				var messages = (ICollection<NotificationMessage>)notification.Messages;
				messages.Add(new NotificationMessage(NotificationSeverity.Error, "foo"));
			}

			[Test]
			public void Should_not_return_an_ICollection()
			{
				var messagesProperty = typeof(Notification).GetMember("Messages").Single();
				typeof(ICollection<NotificationMessage>).IsAssignableFrom(messagesProperty.ReflectedType).ShouldBeFalse("Messages consumer must not receive a modifiable collection");
			}
		}
	}
}