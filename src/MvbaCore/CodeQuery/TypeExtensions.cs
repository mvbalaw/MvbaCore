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

namespace CodeQuery
{
	public static class TypeExtensions
	{
		public static bool IsGenericAssignableFrom(this Type target, Type source)
		{
			if (!target.IsGenericType)
			{
				return false;
			}

			if (target.IsAssignableFrom(source))
			{
				return true;
			}

			if (!typeof(Nullable<>).IsAssignableFrom(target))
			{
				return false;
			}

			var genericParameters = target.GetGenericArguments();

			return genericParameters[0].IsAssignableFrom(source);
		}
	}
}