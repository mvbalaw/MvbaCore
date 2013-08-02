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

namespace MvbaCore.Extensions
{
	public static class Int32Extensions
	{
		public static void Times(this int count, Action action)
		{
			for (var i = 0; i < count; i++)
			{
				action();
			}
		}

		public static void Times(this int count, Action<int> action)
		{
			for (var i = 0; i < count; i++)
			{
				action(i);
			}
		}
	}
}