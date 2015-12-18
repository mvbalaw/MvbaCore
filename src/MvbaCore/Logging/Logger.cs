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

using JetBrains.Annotations;

namespace MvbaCore.Logging
{
	public static class Logger
	{
		public static void Log(NotificationSeverity severity, [NotNull] string text, Exception exception = null)
		{
			var textWriter = Console.Out;
			if (severity == NotificationSeverity.Error)
			{
				textWriter = Console.Error;
			}

			var description = severity + " on " +Environment.MachineName+": "+ text;
			if (exception != null)
			{
				description += Environment.NewLine + exception;
			}
			textWriter.WriteLine(description);
		}
	}
}