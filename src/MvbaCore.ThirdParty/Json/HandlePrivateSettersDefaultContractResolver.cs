//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MvbaCore.ThirdParty.Json
{
	/// <summary>
	/// see: http://daniel.wertheim.se/2010/11/06/json-net-private-setters/
	/// </summary>
	public class HandlePrivateSettersDefaultContractResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);

			if (!prop.Writable)
			{
				var property = member as PropertyInfo;
				if (property != null)
				{
					var hasPrivateSetter = property.GetSetMethod(true) != null;
					prop.Writable = hasPrivateSetter;
				}
			}

			return prop;
		}
	}
}