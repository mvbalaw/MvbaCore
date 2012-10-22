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

using JetBrains.Annotations;

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTests
	{
		[UsedImplicitly]
		public class When_creating_a_Notification_via_WarningFor
		{
			[TestFixture]
			public class Given_an_initial_message_with_Severity_of_Warning
			{
				private const string MessageText = "text";
				private Notification _notification;

				[TestFixtureSetUp]
				public void Before_first_test()
				{
					_notification = Notification.WarningFor(MessageText);
				}

				[Test]
				public void ErrorsAndWarnings_should_return_the_message_text()
				{
					_notification.ErrorsAndWarnings.ShouldBeEqualTo(MessageText);
				}

				[Test]
				public void HasErrors_should_return_False()
				{
					_notification.HasErrors.ShouldBeFalse();
				}

				[Test]
				public void HasWarnings_should_return_True()
				{
					_notification.HasWarnings.ShouldBeTrue();
				}

				[Test]
				public void Should_return_a_Notification_with_a_message_that_has_Warning_Severity()
				{
					_notification.Messages.Count.ShouldBeEqualTo(1);
					_notification.Messages.First().Severity.ShouldBeEqualTo(NotificationSeverity.Warning);
				}

				[Test]
				public void Should_return_a_Notification_with_a_message_that_has_the_given_message_text()
				{
					_notification.Messages.Count.ShouldBeEqualTo(1);
					_notification.Messages.First().Message.ShouldBeEqualTo(MessageText);
				}

				[Test]
				public void Warnings_should_return_the_message_text()
				{
					_notification.Warnings.ShouldBeEqualTo(MessageText);
				}
			}
		}
	}
}