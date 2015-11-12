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
			if (notification.HasErrors)
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
		private readonly List<NotificationMessage> _messages;

		protected NotificationBase()
		{
			_messages = new List<NotificationMessage>();
		}

		protected NotificationBase([NotNull] NotificationMessage notificationMessage)
			: this()
		{
			AddMessage(notificationMessage);
		}

		public string Errors
		{
			get { return !HasErrors ? "" : GetMessages(x => x.Severity == NotificationSeverity.Error); }
		}

		public string ErrorsAndWarnings
		{
			get { return !(HasErrors || HasWarnings) ? "" : GetMessages(x => x.Severity == NotificationSeverity.Error || x.Severity == NotificationSeverity.Warning); }
		}

// ReSharper disable MemberCanBeProtected.Global
		public bool HasErrors { get; private set; }
// ReSharper restore MemberCanBeProtected.Global

		public bool HasWarnings { get; private set; }

		public string Infos
		{
			get { return GetMessages(x => x.Severity == NotificationSeverity.Info); }
		}

		public bool IsValid
		{
			get { return !(HasErrors || HasWarnings); }
		}

		public IEnumerable<NotificationMessage> Messages
		{
			get { return _messages.ToArray(); }
		}
		public string Warnings
		{
			get { return !HasWarnings ? "" : GetMessages(x => x.Severity == NotificationSeverity.Warning); }
		}

		public void Add(Notification notification)
		{
			foreach (var message in notification.Messages)
			{
				AddMessage(message);
			}
		}

		public void Add(NotificationMessage message)
		{
			AddMessage(message);
		}

		private void AddMessage(NotificationMessage message)
		{
			if (!Messages.Any(x => x.Severity == message.Severity && x.Message == message.Message))
			{
				switch (message.Severity)
				{
					case NotificationSeverity.Error:
						HasErrors = true;
						break;
					case NotificationSeverity.Warning:
						HasWarnings = true;
						break;
				}
				_messages.Add(message);
			}
		}

		private string GetMessages(Func<NotificationMessage, bool> predicate)
		{
			return Messages.Where(predicate).Select(x => x.Message).Join(Environment.NewLine);
		}

		public override string ToString()
		{
			return ErrorsAndWarnings;
		}
	}

	public class Notification : NotificationBase
	{
		public Notification()
		{
		}

// ReSharper disable once MemberCanBeProtected.Global
		public Notification(NotificationMessage notificationMessage)
			: base(notificationMessage)
		{
		}

		public static Notification Empty
		{
			get { return new Notification(); }
		}

		public static Notification ErrorFor(string messageText)
		{
			return For(NotificationSeverity.Error, messageText);
		}

		[StringFormatMethod("messageFormatString")]
// ReSharper disable MethodOverloadWithOptionalParameter
		public static Notification ErrorFor(string messageFormatString, params object[] messageParameters)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			return For(NotificationSeverity.Error, messageFormatString, messageParameters);
		}

		public static Notification For(NotificationSeverity severity, string messageText)
		{
			return new Notification(new NotificationMessage(severity, messageText));
		}

		[StringFormatMethod("messageFormatString")]
// ReSharper disable MethodOverloadWithOptionalParameter
		private static Notification For(NotificationSeverity severity, string messageFormatString, params object[] messageParameters)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			return new Notification(new NotificationMessage(severity, messageFormatString, messageParameters));
		}

		public static Notification InfoFor(string messageText)
		{
			return For(NotificationSeverity.Info, messageText);
		}

		[StringFormatMethod("messageFormatString")]
// ReSharper disable MethodOverloadWithOptionalParameter
		public static Notification InfoFor(string messageFormatString, params object[] messageParameters)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			return For(NotificationSeverity.Info, messageFormatString, messageParameters);
		}

		public static Notification WarningFor(string messageText)
		{
			return For(NotificationSeverity.Warning, messageText);
		}

		[StringFormatMethod("messageFormatString")]
// ReSharper disable MethodOverloadWithOptionalParameter
		public static Notification WarningFor(string messageFormatString, params object[] messageParameters)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			return For(NotificationSeverity.Warning, messageFormatString, messageParameters);
		}
	}

	public static class NotificationExtensions
	{
		public static Notification<T> ToNotification<T>(this Notification notification)
		{
			// because we can't implicitly cast up from a base class
			return new Notification<T>(notification);
		}

		public static Notification<T> ToNotification<T>(this Notification notification, T item)
		{
			return new Notification<T>(notification, item);
		}
	}
}