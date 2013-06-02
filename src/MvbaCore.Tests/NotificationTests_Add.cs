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

using NUnit.Framework;

namespace MvbaCore.Tests
{
	public partial class NotificationTests
	{
		[TestFixture]
		public class When_asked_to_Add_a_Notification
		{
			[Test]
			public void Should_copy_the_NotificationMessages_from_the_source_if_the_destination_is_not_Notification_Null()
			{
				var source = new Notification();
				var notification = new NotificationMessage(NotificationSeverity.Error, "");
				source.Add(notification);

				var destination = new Notification();
				destination.Add(notification);

				Assert.AreEqual(1, destination.Messages.Count());
				Assert.AreEqual(notification, destination.Messages.First());
			}

			[Test]
			public void Should_not_add_any_message_that_is_identical_to_a_message_already_contained_by_the_Notification()
			{
				var source = new Notification();
				source.Add(new NotificationMessage(NotificationSeverity.Warning, ""));
				source.Add(new NotificationMessage(NotificationSeverity.Error, "dup"));

				var destination = new Notification();
				destination.Add(new NotificationMessage(NotificationSeverity.Error, "dup"));
				destination.Add(source);

				Assert.AreEqual(2, destination.Messages.Count());
			}

			[Test]
			public void Should_not_overwrite_any_NotificationMessages_that_already_exist()
			{
				var source = new Notification();
				source.Add(new NotificationMessage(NotificationSeverity.Warning, ""));

				var destination = new Notification();
				destination.Add(new NotificationMessage(NotificationSeverity.Error, ""));
				destination.Add(source);

				Assert.AreEqual(2, destination.Messages.Count());
			}

			[Test]
			public void Should_succeed_if_the_Notification_being_added_has_no_messages()
			{
				var source = new Notification();
				var destination = new Notification();
				destination.Add(source);
				Assert.AreEqual(0, destination.Messages.Count());
			}
		}

		[TestFixture]
		public class When_asked_to_Add_a_NotificationMessage
		{
			[Test]
			public void Should_add_the_message_to_Messages_if_the_Notification_is_not_Notification_Null()
			{
				var notification = new Notification();
				var messageTest = new NotificationMessage(NotificationSeverity.Warning, "");
				notification.Add(messageTest);
				Assert.AreEqual(1, notification.Messages.Count());
				Assert.AreEqual(messageTest, notification.Messages.First());
			}

			[Test]
			public void Should_not_add_the_message_if_the_Notification_already_contains_an_identical_message()
			{
				var notification = new Notification();
				const NotificationSeverity notificationSeverity = NotificationSeverity.Warning;
				const string notificationText = "test";
				var messageTest = new NotificationMessage(notificationSeverity, notificationText);
				notification.Add(messageTest);
				var secondMessage = new NotificationMessage(notificationSeverity, notificationText);
				notification.Add(secondMessage);
				Assert.AreEqual(1, notification.Messages.Count());
				Assert.AreEqual(messageTest, notification.Messages.First());
			}
		}

		[TestFixture]
		public class When_asked_to_Add_a_Notification_T
		{
			[Test]
			public void Should_copy_the_NotificationMessages_from_the_source_if_the_destination_is_not_Notification_Null()
			{
				var source = new Notification<string>();
				var notification = new NotificationMessage(NotificationSeverity.Error, "");
				source.Add(notification);

				var destination = new Notification();
				destination.Add(notification);

				Assert.AreEqual(1, destination.Messages.Count());
				Assert.AreEqual(notification, destination.Messages.First());
			}

			[Test]
			public void Should_not_add_any_message_that_is_identical_to_a_message_already_contained_by_the_Notification()
			{
				var source = new Notification<string>();
				source.Add(new NotificationMessage(NotificationSeverity.Warning, ""));
				source.Add(new NotificationMessage(NotificationSeverity.Error, "dup"));

				var destination = new Notification();
				destination.Add(new NotificationMessage(NotificationSeverity.Error, "dup"));
				destination.Add(source);

				Assert.AreEqual(2, destination.Messages.Count());
			}

			[Test]
			public void Should_not_overwrite_any_NotificationMessages_that_already_exist()
			{
				var source = new Notification<string>();
				source.Add(new NotificationMessage(NotificationSeverity.Warning, ""));

				var destination = new Notification();
				destination.Add(new NotificationMessage(NotificationSeverity.Error, ""));
				destination.Add(source);

				Assert.AreEqual(2, destination.Messages.Count());
			}

			[Test]
			public void Should_succeed_if_the_Notification_being_added_has_no_messages()
			{
				var source = new Notification<string>();
				var destination = new Notification();
				destination.Add(source);
				Assert.AreEqual(0, destination.Messages.Count());
			}
		}
	}
}