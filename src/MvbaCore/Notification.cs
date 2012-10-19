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

using JetBrains.Annotations;

namespace MvbaCore
{
	public class Notification<T> : Notification
	{
		public Notification()
		{
		}

		public Notification(NotificationMessage notificationMessage)
			: base(notificationMessage)
		{
		}

		public Notification(Notification notification, T item = default(T))
		{
			Item = item;
			Add(notification);
		}

		public new static Notification<T> Empty
		{
			get { return new Notification<T>(); }
		}

		public T Item { get; set; }

		public static implicit operator T(Notification<T> notification)
		{
			if (!notification.IsValid)
			{
				throw new ArgumentNullException(string.Format("Cannot implicitly cast Notification<{0}> to {0} when there are errors.", typeof(T).Name));
			}
			return notification.Item;
		}

		public static implicit operator Notification<T>(T item)
		{
			return new Notification<T>
				       {
					       Item = item
				       };
		}
	}

	public abstract class NotificationBase
	{
		protected NotificationBase()
		{
			Messages = new List<NotificationMessage>();
		}

		protected NotificationBase([NotNull] NotificationMessage notificationMessage)
			: this()
		{
			Messages = new List<NotificationMessage>
				           {
					           notificationMessage
				           };
		}

		public string Errors
		{
			get { return Messages.Where(x => x.Severity != NotificationSeverity.Info).Select(x => x.Message).Join(Environment.NewLine); }
		}

		public string Infos
		{
			get { return Messages.Where(x => x.Severity == NotificationSeverity.Info).Select(x => x.Message).Join(Environment.NewLine); }
		}

		public bool IsValid
		{
			get { return Messages.All(x => x.Severity == NotificationSeverity.Info); }
		}

		[NotNull]
		public List<NotificationMessage> Messages { get; private set; }

		public void Add([NotNull] Notification notification)
		{
			foreach (var message in notification.Messages)
			{
				AddMessage(message);
			}
		}

		public void Add([NotNull] NotificationMessage message)
		{
			AddMessage(message);
		}

		private void AddMessage(NotificationMessage message)
		{
			if (!(Messages.Any(x => x.Severity == message.Severity && x.Message == message.Message)))
			{
				Messages.Add(message);
			}
		}

		public override string ToString()
		{
			return Errors;
		}
	}

	public class Notification : NotificationBase
	{
		public Notification()
		{
		}

		public Notification(NotificationMessage notificationMessage)
			: base(notificationMessage)
		{
		}

		public static Notification Empty
		{
			get { return new Notification(); }
		}

		[NotNull]
		public static Notification ErrorFor([NotNull] string messageText)
		{
			return For(NotificationSeverity.Error, messageText);
		}

		[StringFormatMethod("messageFormatString")]
		public static Notification ErrorFor([NotNull] string messageFormatString, params object[] messageParameters)
		{
			return new Notification(new NotificationMessage(NotificationSeverity.Error, messageFormatString, messageParameters));
		}

		[NotNull]
		public static Notification For(NotificationSeverity severity, [NotNull] string messageText)
		{
			return new Notification(new NotificationMessage(severity, messageText));
		}

		[NotNull]
		public static Notification InfoFor([NotNull] string messageText)
		{
			return For(NotificationSeverity.Info, messageText);
		}

		[StringFormatMethod("messageFormatString")]
		public static Notification InfoFor([NotNull] string messageFormatString, params object[] messageParameters)
		{
			return new Notification(new NotificationMessage(NotificationSeverity.Info, messageFormatString, messageParameters));
		}

		[NotNull]
		public static Notification WarningFor([NotNull] string messageText)
		{
			return For(NotificationSeverity.Warning, messageText);
		}

		[StringFormatMethod("messageFormatString")]
		public static Notification WarningFor([NotNull] string messageFormatString, params object[] messageParameters)
		{
			return new Notification(new NotificationMessage(NotificationSeverity.Warning, messageFormatString, messageParameters));
		}
	}

	public static class NotificationExtensions
	{
		public static Notification<T> ToNotification<T>(this Notification notification)
		{
			// becuase we can't implicitly cast up from a base class
			return new Notification<T>(notification);
		}

		public static Notification<T> ToNotification<T>(this Notification notification, T item)
		{
			return new Notification<T>(notification, item);
		}
	}
}