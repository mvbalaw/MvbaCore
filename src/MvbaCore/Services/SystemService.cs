//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.Security.Principal;

using JetBrains.Annotations;

namespace MvbaCore.Services
{
	public interface ISystemService
	{
		DateTime CurrentDateTime { get; }

		[NotNull]
		[Pure]
		string GetLoginName([NotNull] IPrincipal principal);
	}

	public class SystemService : ISystemService
	{
		private readonly Func<TimeZoneInfo> _getTimeZone;
		private readonly Func<DateTime> _getUtcNow;

		public SystemService()
			: this(() => DateTime.UtcNow, () => TimeZoneInfo.Local)
		{
		}

		// internal so that IoC containers, which build through the public constructor with the most parameters, keep
		// using the one above
		internal SystemService(Func<DateTime> getUtcNow, Func<TimeZoneInfo> getTimeZone)
		{
			_getUtcNow = getUtcNow;
			_getTimeZone = getTimeZone;
		}

		// The offset is worked out from the time zone's rules on every read. It used to be worked out once, when the
		// class was first used, so a long-running process kept that offset across a daylight saving time change and
		// answered an hour off until it was restarted - and for the first hour after the spring change answered a time
		// that does not exist, which throws when converted back to UTC. Converting into TimeZoneInfo.Local itself is
		// what makes the result DateTimeKind.Local and keeps the flag that tells the first 1 AM hour of the fall change
		// from the second, so converting it back to UTC lands on the instant it was read at.
		public DateTime CurrentDateTime
		{
			get { return TimeZoneInfo.ConvertTimeFromUtc(_getUtcNow(), _getTimeZone()); }
		}

		public string GetLoginName(IPrincipal principal)
		{
			var identity = principal.Identity;
			var fullNetworkIdentity = identity.Name;
			return fullNetworkIdentity.Substring(fullNetworkIdentity.IndexOf("\\") + 1);
		}
	}
}