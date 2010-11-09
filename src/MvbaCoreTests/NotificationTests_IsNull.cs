//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

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
				var notification = new Notification();
				Assert.IsFalse(notification.IsNull);
			}

			[Test]
			public void Should_return_true_if_the_Notification_is_Notification_Null()
			{
				var notification = Notification.Null;
				Assert.IsTrue(notification.IsNull);
			}
		}
	}
}