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

		[Pure]
		public static implicit operator T(Notification<T> notification)
		{
			if (notification.HasErrors)
			{
				throw new ArgumentNullException(string.Format("Cannot implicitly cast Notification<{0}> to {0} when there are errors.", typeof(T).Name));
			}
			return notification.Item;
		}

		[Pure]
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

		[NotNull]
		public string Errors
		{
			get { return !HasErrors ? "" : GetMessages(x => x.Severity == NotificationSeverity.Error); }
		}

		[NotNull]
		public string ErrorsAndWarnings
		{
			get { return !(HasErrors || HasWarnings) ? "" : GetMessages(x => x.Severity == NotificationSeverity.Error || x.Severity == NotificationSeverity.Warning); }
		}

// ReSharper disable MemberCanBeProtected.Global
		public bool HasErrors { get; private set; }
// ReSharper restore MemberCanBeProtected.Global

		public bool HasWarnings { get; private set; }

		[NotNull]
		public string Infos
		{
			get { return GetMessages(x => x.Severity == NotificationSeverity.Info); }
		}

		public bool IsValid
		{
			get { return !(HasErrors || HasWarnings); }
		}

		[NotNull]
		[ItemNotNull]
		public IEnumerable<NotificationMessage> Messages
		{
			get { return _messages.ToArray(); }
		}
		[NotNull]
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

		[NotNull]
		[Pure]
		private string GetMessages([NotNull] Func<NotificationMessage, bool> predicate)
		{
			return Messages.Where(predicate).Select(x => x.Message).Join(Environment.NewLine);
		}

		[Pure]
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

		[NotNull]
		public static Notification Empty
		{
			get { return new Notification(); }
		}

		[Pure]
		[NotNull]
		public static Notification ErrorFor([NotNull] string messageText)
		{
			return For(NotificationSeverity.Error, messageText);
		}

		[Pure]
		[StringFormatMethod("messageFormatString")]
		[NotNull]
// ReSharper disable MethodOverloadWithOptionalParameter
		public static Notification ErrorFor([NotNull] string messageFormatString, params object[] messageParameters)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			return For(NotificationSeverity.Error, messageFormatString, messageParameters);
		}

		[Pure]
		[NotNull]
		public static Notification For(NotificationSeverity severity, [NotNull] string messageText)
		{
			return new Notification(new NotificationMessage(severity, messageText));
		}

		[Pure]
		[StringFormatMethod("messageFormatString")]
		[NotNull]
// ReSharper disable MethodOverloadWithOptionalParameter
		private static Notification For(NotificationSeverity severity, [NotNull] string messageFormatString, params object[] messageParameters)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			return new Notification(new NotificationMessage(severity, messageFormatString, messageParameters));
		}

		[Pure]
		[NotNull]
		public static Notification InfoFor([NotNull] string messageText)
		{
			return For(NotificationSeverity.Info, messageText);
		}

		[Pure]
		[StringFormatMethod("messageFormatString")]
		[NotNull]
// ReSharper disable MethodOverloadWithOptionalParameter
		public static Notification InfoFor([NotNull] string messageFormatString, params object[] messageParameters)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			return For(NotificationSeverity.Info, messageFormatString, messageParameters);
		}

		[Pure]
		[NotNull]
		public static Notification WarningFor([NotNull] string messageText)
		{
			return For(NotificationSeverity.Warning, messageText);
		}

		[Pure]
		[StringFormatMethod("messageFormatString")]
		[NotNull]
// ReSharper disable MethodOverloadWithOptionalParameter
		public static Notification WarningFor([NotNull] string messageFormatString, params object[] messageParameters)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			return For(NotificationSeverity.Warning, messageFormatString, messageParameters);
		}
	}

	public static class NotificationExtensions
	{
		[Pure]
		[NotNull]
		public static Notification<T> ToNotification<T>([NotNull] this Notification notification)
		{
			// because we can't implicitly cast up from a base class
			return new Notification<T>(notification);
		}

		[Pure]
		[NotNull]
		public static Notification<T> ToNotification<T>([NotNull] this Notification notification, [NotNull] T item)
		{
			return new Notification<T>(notification, item);
		}
	}
}