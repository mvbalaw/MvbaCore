//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using NUnit.Framework;

namespace MvbaCore.Tests
{
	public partial class NotificationTests
	{
		[TestFixture]
		public class When_asked_if_IsValid
		{
			[Test]
			public void Should_return_false_if_Messages_contains_only_messages_with_Error_Severity()
			{
				var notification = new Notification();
				var messageTest = new NotificationMessage(NotificationSeverity.Error, "");
				notification.Add(messageTest);

				Assert.IsFalse(notification.IsValid);
			}

			[Test]
			public void Should_return_false_if_Messages_contains_only_messages_with_Warning_Severity()
			{
				var notification = new Notification();
				var messageTest = new NotificationMessage(NotificationSeverity.Warning, "");
				notification.Add(messageTest);

				Assert.IsFalse(notification.IsValid);
			}

			[Test]
			public void Should_return_true_if_Messages_contains_only_messages_with_Info_Severity()
			{
				var notification = new Notification();
				var messageTest = new NotificationMessage(NotificationSeverity.Info, "");
				notification.Add(messageTest);
				Assert.IsTrue(notification.IsValid);
			}

			[Test]
			public void Should_return_true_if_Messages_is_empty()
			{
				var notification = new Notification();
				Assert.IsTrue(notification.IsValid);
			}
		}
	}
}