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

namespace MvbaCore.Services
{
	public interface ISystemService
	{
		DateTime CurrentDateTime { get; }
		string GetLoginName(IPrincipal principal);
	}

	public class SystemService : ISystemService
	{
		private static readonly TimeSpan LocalUtcOffset;

		static SystemService()
		{
			LocalUtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
		}

		public DateTime CurrentDateTime
		{
			get { return DateTime.SpecifyKind(DateTime.UtcNow + LocalUtcOffset, DateTimeKind.Local); }
		}

		public string GetLoginName(IPrincipal principal)
		{
			var identity = principal.Identity;
			var fullNetworkIdentity = identity.Name;
			return fullNetworkIdentity.Substring(fullNetworkIdentity.IndexOf("\\") + 1);
		}
	}
}