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
using System.Collections.Generic;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace MvbaCore.Tests
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