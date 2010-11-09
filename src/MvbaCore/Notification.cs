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
	public class Notification<T> : NotificationBase
	{
		public static readonly Notification<T> Null = new Notification<T>
		{
			IsNull = true
		};
		private T _item;

		public Notification()
		{
		}

		public Notification(NotificationMessage notificationMessage)
			: base(notificationMessage)
		{
		}

		public T Item
		{
			get { return _item; }
			set
			{
				if (!IsNull)
				{
					_item = value;
				}
			}
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
			Messages = new List<NotificationMessage>();
			if (!IsNull)
			{
				Messages.Add(notificationMessage);
			}
		}

		public bool IsNull { get; protected set; }
		public bool IsValid
		{
			get { return !Messages.Any(x => x.Severity != NotificationSeverity.Info); }
		}

		[NotNull]
		public List<NotificationMessage> Messages { get; private set; }

		public void Add([NotNull] Notification notification)
		{
			if (IsNull)
			{
				return;
			}
			foreach (var message in notification.Messages)
			{
				AddMessage(message);
			}
		}

		public void Add([NotNull] NotificationMessage message)
		{
			if (IsNull)
			{
				return;
			}
			AddMessage(message);
		}

		public void Add<K>([NotNull] Notification<K> notification)
		{
			if (IsNull)
			{
				return;
			}
			foreach (var message in notification.Messages)
			{
				AddMessage(message);
			}
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
			return Messages.Where(x => x.Severity != NotificationSeverity.Info).Select(x => x.Message).Join(Environment.NewLine);
		}
	}

	public class Notification : NotificationBase
	{
		public static readonly Notification Null = new Notification
		{
			IsNull = true
		};

		public Notification()
		{
		}

		public Notification(NotificationMessage notificationMessage)
			: base(notificationMessage)
		{
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
}