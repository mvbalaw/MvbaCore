using System.Linq;

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTest
	{
		[TestFixture]
		public class When_asked_to_Add_a_Notification
		{
			[Test]
			public void Should_copy_the_NotificationMessages_from_the_source_if_the_destination_is_not_Notification_Null()
			{
				Notification source = new Notification();
				NotificationMessage notification = new NotificationMessage(NotificationSeverity.Error, "");
				source.Add(notification);

				Notification destination = new Notification();
				destination.Add(notification);

				Assert.AreEqual(1, destination.Messages.Count);
				Assert.AreEqual(notification, destination.Messages.First());
			}

			[Test]
			public void Should_not_add_any_message_that_is_identical_to_a_message_already_contained_by_the_Notification()
			{
				Notification source = new Notification();
				source.Add(new NotificationMessage(NotificationSeverity.Warning, ""));
				source.Add(new NotificationMessage(NotificationSeverity.Error, "dup"));

				Notification destination = new Notification();
				destination.Add(new NotificationMessage(NotificationSeverity.Error, "dup"));
				destination.Add(source);

				Assert.AreEqual(2, destination.Messages.Count);
			}

			[Test]
			public void Should_not_copy_the_NotificationMessages_from_the_source_if_the_destination_is_Notification_Null()
			{
				Notification source = new Notification();
				NotificationMessage notification = new NotificationMessage(NotificationSeverity.Error, "");
				source.Add(notification);

				Notification destination = Notification.Null;
				destination.Add(notification);

				Assert.AreEqual(0, destination.Messages.Count);
			}

			[Test]
			public void Should_not_overwrite_any_NotificationMessages_that_already_exist()
			{
				Notification source = new Notification();
				source.Add(new NotificationMessage(NotificationSeverity.Warning, ""));

				Notification destination = new Notification();
				destination.Add(new NotificationMessage(NotificationSeverity.Error, ""));
				destination.Add(source);

				Assert.AreEqual(2, destination.Messages.Count);
			}

			[Test]
			public void Should_succeed_if_the_Notification_being_added_has_no_messages()
			{
				Notification source = new Notification();
				Notification destination = new Notification();
				destination.Add(source);
				Assert.AreEqual(0, destination.Messages.Count);
			}
		}

		[TestFixture]
		public class When_asked_to_Add_a_Notification_T
		{
			[Test]
			public void Should_copy_the_NotificationMessages_from_the_source_if_the_destination_is_not_Notification_Null()
			{
				var source = new Notification<string>();
				NotificationMessage notification = new NotificationMessage(NotificationSeverity.Error, "");
				source.Add(notification);

				Notification destination = new Notification();
				destination.Add(notification);

				Assert.AreEqual(1, destination.Messages.Count);
				Assert.AreEqual(notification, destination.Messages.First());
			}

			[Test]
			public void Should_not_add_any_message_that_is_identical_to_a_message_already_contained_by_the_Notification()
			{
				var source = new Notification<string>();
				source.Add(new NotificationMessage(NotificationSeverity.Warning, ""));
				source.Add(new NotificationMessage(NotificationSeverity.Error, "dup"));

				Notification destination = new Notification();
				destination.Add(new NotificationMessage(NotificationSeverity.Error, "dup"));
				destination.Add(source);

				Assert.AreEqual(2, destination.Messages.Count);
			}

			[Test]
			public void Should_not_copy_the_NotificationMessages_from_the_source_if_the_destination_is_Notification_Null()
			{
				var source = new Notification<string>();
				NotificationMessage notification = new NotificationMessage(NotificationSeverity.Error, "");
				source.Add(notification);

				Notification destination = Notification.Null;
				destination.Add(notification);

				Assert.AreEqual(0, destination.Messages.Count);
			}

			[Test]
			public void Should_not_overwrite_any_NotificationMessages_that_already_exist()
			{
				var source = new Notification<string>();
				source.Add(new NotificationMessage(NotificationSeverity.Warning, ""));

				Notification destination = new Notification();
				destination.Add(new NotificationMessage(NotificationSeverity.Error, ""));
				destination.Add(source);

				Assert.AreEqual(2, destination.Messages.Count);
			}

			[Test]
			public void Should_succeed_if_the_Notification_being_added_has_no_messages()
			{
				var source = new Notification<string>();
				Notification destination = new Notification();
				destination.Add(source);
				Assert.AreEqual(0, destination.Messages.Count);
			}
		}

		[TestFixture]
		public class When_asked_to_Add_a_NotificationMessage
		{
			[Test]
			public void Should_add_the_message_to_Messages_if_the_Notification_is_not_Notification_Null()
			{
				Notification notification = new Notification();
				NotificationMessage messageTest = new NotificationMessage(NotificationSeverity.Warning, "");
				notification.Add(messageTest);
				Assert.AreEqual(1, notification.Messages.Count);
				Assert.AreEqual(messageTest, notification.Messages.First());
			}

			[Test]
			public void Should_not_add_the_message_if_the_Notification_already_contains_an_identical_message()
			{
				Notification notification = new Notification();
				const NotificationSeverity notificationSeverity = NotificationSeverity.Warning;
				const string notificationText = "test";
				NotificationMessage messageTest = new NotificationMessage(notificationSeverity, notificationText);
				notification.Add(messageTest);
				NotificationMessage secondMessage = new NotificationMessage(notificationSeverity, notificationText);
				notification.Add(secondMessage);
				Assert.AreEqual(1, notification.Messages.Count);
				Assert.AreEqual(messageTest, notification.Messages.First());
			}

			[Test]
			public void Should_not_add_the_message_to_Messages_if_the_Notification_is_Notification_Null()
			{
				Notification notification = Notification.Null;
				NotificationMessage messageTest = new NotificationMessage(NotificationSeverity.Warning, "");
				notification.Add(messageTest);
				Assert.AreEqual(0, notification.Messages.Count);
			}
		}
	}
}