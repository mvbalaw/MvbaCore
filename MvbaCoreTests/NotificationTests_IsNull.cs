using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTest
	{
		[TestFixture]
		public class When_asked_if_IsNull
		{
			[Test]
			public void Should_return_false_if_the_Notification_is_not_Notification_Null()
			{
				Notification notification = new Notification();
				Assert.IsFalse(notification.IsNull);
			}

			[Test]
			public void Should_return_true_if_the_Notification_is_Notification_Null()
			{
				Notification notification = Notification.Null;
				Assert.IsTrue(notification.IsNull);
			}
		}
	}
}