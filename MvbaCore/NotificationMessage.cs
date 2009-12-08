using System;

using JetBrains.Annotations;

namespace MvbaCore
{
	public class NotificationMessage : IEquatable<NotificationMessage>
	{
		[StringFormatMethod("messageFormatString")]
		public NotificationMessage(NotificationSeverity severity, [NotNull] string messageFormatString,
		                           params object[] messageParameters)
		{
			Severity = severity;
			Message = String.Format(messageFormatString, messageParameters);
		}

		public NotificationMessage(NotificationSeverity severity, [NotNull] string message)
		{
			Severity = severity;
			Message = message;
		}

		public string Message { get; private set; }
		public NotificationSeverity Severity { get; private set; }

		public virtual bool Equals(NotificationMessage other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Severity, Severity) && Equals(other.Message, Message);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(NotificationMessage))
			{
				return false;
			}
			return Equals((NotificationMessage)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Severity.GetHashCode() * 397) ^ (Message != null ? Message.GetHashCode() : 0);
			}
		}

		public override string ToString()
		{
			return Severity + ": " + Message;
		}
	}
}