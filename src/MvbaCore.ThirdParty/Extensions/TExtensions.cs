//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System.IO;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class TExtensions
	{

		[NotNull]
		public static Stream SearializeToJsonStream<T>(this T itemToSerialize)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			SerializeToJson(itemToSerialize, writer);
			stream.Position = 0;
			return stream;
		}

		public static void SerializeToJson<T>(this T itemToSerialize, StreamWriter streamWriter, IContractResolver contractResolver = null)
		{
			var jsonWriter = new JsonTextWriter(streamWriter)
			{
				Formatting = Formatting.Indented
			};
			var serializer = new JsonSerializer
			{
				NullValueHandling = NullValueHandling.Ignore,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				TypeNameHandling = TypeNameHandling.All
			};
			if (contractResolver != null)
			{
				serializer.ContractResolver = contractResolver;
			}
			serializer.Serialize(jsonWriter, itemToSerialize);
			jsonWriter.Flush();
		}

		public static void SerializeToJsonFile<T>(this T itemToSerialize, string filePath)
		{
			using (var streamWriter = new StreamWriter(filePath))
			{
				SerializeToJson(itemToSerialize, streamWriter);
			}
		}

	}
}