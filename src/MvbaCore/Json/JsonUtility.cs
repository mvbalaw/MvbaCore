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
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MvbaCore.Json
{
	public static class JsonUtility
	{
		public static T Deserialize<T>(string s)
		{
			return JsonConvert.DeserializeObject<T>(s, GetJsonSerializerSettings());
		}

		public static object Deserialize(string s, Type type)
		{
			return JsonConvert.DeserializeObject(s, type, GetJsonSerializerSettings());
		}

		public static T DeserializeFromJsonFile<T>(string filePath)
		{
			return (T)DeserializeFromJsonFile(filePath, typeof(T));
		}

		public static object DeserializeFromJsonFile(string filePath, Type type)
		{
			var serializer = JsonSerializer.Create(GetJsonSerializerSettings());
			using (var reader = new StreamReader(filePath))
			{
				return serializer.Deserialize(reader, type);
			}
		}

		public static object DeserializeFromStream(Stream stream, Type type)
		{
			var serializer = JsonSerializer.Create(GetJsonSerializerSettings());
			using (var reader = new StreamReader(stream))
			{
				return serializer.Deserialize(reader, type);
			}
		}

		private static JsonSerializerSettings GetJsonSerializerSettings()
		{
			var contractResolver = new HandlePrivateSettersDefaultContractResolver();
			var settings = new JsonSerializerSettings
			{
				ContractResolver = contractResolver
			};
			return settings;
		}

		public static string SerializeForComparison<T>(T obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.None,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				Converters = new List<JsonConverter>
                        {
                            new IsoDateTimeConverter()
                        }
			});
		}

		public static string SerializeForWebRequest<T>(T obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			});
		}

		public static void SerializeToFile<T>(T obj, string filePath)
		{
			var serializer = new JsonSerializer
			{
				TypeNameHandling = TypeNameHandling.All
			};

			using (var streamWriter = new StreamWriter(filePath))
			{
				using (var jsonTextWriter = new JsonTextWriter(streamWriter))
				{
					jsonTextWriter.Formatting = Formatting.Indented;
					using (var writer = jsonTextWriter)
					{
						serializer.Serialize(writer, obj);
					}
				}
			}
		}

		public static void SerializeToStream<T>(T obj, TextWriter fileStream)
		{
			var serializer = new JsonSerializer
			{
				TypeNameHandling = TypeNameHandling.All,
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			};
			serializer.Converters.Add(new IsoDateTimeConverter());

			using (var streamWriter = fileStream)
			{
				using (var jsonTextWriter = new JsonTextWriter(streamWriter))
				{
					jsonTextWriter.Formatting = Formatting.Indented;
					using (var writer = jsonTextWriter)
					{
						serializer.Serialize(writer, obj);
					}
				}
			}
		}
	}
}