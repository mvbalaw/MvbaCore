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
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MvbaCore.ThirdParty.Json
{
	public interface IJsonWebServiceClient
	{
		TOutput Post<TInput, TOutput>(string url, TInput data);
		TOutput Post<TOutput>(string url);
		TOutput PostDataContract<TInput, TOutput>(string url, TInput data);
	}

	public class JsonWebServiceClient : IJsonWebServiceClient
	{
		public TOutput Post<TInput, TOutput>(string url, TInput data)
		{
			var req = CreateWebRequest(url);
			req.Method = "POST";
			string content = JsonUtility.SerializeForWebRequest(data);
			SendRequest(req, content);
			return GetResponse<TOutput>(req);
		}

		public TOutput PostDataContract<TInput, TOutput>(string url, TInput data)
		{
			var req = CreateWebRequest(url);
			req.Method = "POST";
			var memoryStream = new MemoryStream();
			new DataContractJsonSerializer(typeof(TInput)).WriteObject(memoryStream, data);
			byte[] json = memoryStream.ToArray();
			memoryStream.Close();
			var content = Encoding.UTF8.GetString(json, 0, json.Length);
			SendRequest(req, content);
			return GetResponse<TOutput>(req);
		}

		public TOutput Post<TOutput>(string url)
		{
			var req = CreateWebRequest(url);
			req.Method = "POST";
			AddUserCredentials(req);
			req.GetRequestStream().Close();
			return GetResponse<TOutput>(req);
		}

		private static void AddUserCredentials(WebRequest req)
		{
			req.Credentials = CredentialCache.DefaultCredentials;
		}

		private static HttpWebRequest CreateWebRequest(string url)
		{
			var req = (HttpWebRequest)WebRequest.Create(url);
			req.ContentType = "application/json";
			return req;
		}

		private static Notification<TOutput> GetResponse<TOutput>(WebRequest req)
		{
			var response = req.GetResponse();
			var responseStream = response.GetResponseStream();
			if (responseStream == null)
			{
				return new Notification<TOutput>(Notification.ErrorFor("received null response stream"));
			}

			using (var reader = new StreamReader(responseStream))
			{
				string s = reader.ReadToEnd();
				TOutput output;
				try
				{
					output = JsonUtility.Deserialize<TOutput>(s);
				}
				catch (Exception exception)
				{
					var notification = new Notification<TOutput>( Notification.ErrorFor("caught exception deserializing:\n" + s + "\n" + exception.Message))
						                   {
							                   Item = default(TOutput)
						                   };
					return notification;
				}
				return output;
			}
		}

		private static void SendRequest(WebRequest req, string content)
		{
			AddUserCredentials(req);
			req.ContentLength = content.Length;

			using (var streamWriter = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
			{
				streamWriter.Write(content);
			}
		}
	}
}