//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace MvbaCore.Tests
{
	public partial class NotificationTests
	{
		[TestFixture]
		public class When_asked_For_a_Notification
		{
			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_the_given_Severity()
			{
				const NotificationSeverity severity = NotificationSeverity.Warning;
				var notification = Notification.For(severity, "text");

				notification.Messages.Count().ShouldBeEqualTo(1);
				notification.Messages.First().Severity.ShouldBeEqualTo(severity);
			}

			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_the_given_messageText()
			{
				const string messageText = "text";
				var notification = Notification.For(NotificationSeverity.Warning, messageText);

				notification.Messages.Count().ShouldBeEqualTo(1);
				notification.Messages.First().Message.ShouldBeEqualTo(messageText);
			}
		}
	}
}