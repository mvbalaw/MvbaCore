//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.IO;

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace System
{
	public static class StreamExtensions
	{
		public static byte[] ReadAllBytes([NotNull] this Stream source)
		{
			// original from: http://geekswithblogs.net/sdorman/archive/2009/01/10/reading-all-bytes-from-a-stream.aspx
			var originalPosition = source.Position;
			source.Position = 0;

			try
			{
				var readBuffer = new byte[4096];
				var totalBytesRead = 0;
				int bytesRead;
				while ((bytesRead = source.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
				{
					totalBytesRead += bytesRead;
					if (totalBytesRead == readBuffer.Length)
					{
						var nextByte = source.ReadByte();
						if (nextByte != -1)
						{
							var temp = new byte[readBuffer.Length * 2];
							Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
							Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
							readBuffer = temp;
							totalBytesRead++;
						}
					}
				}
				var buffer = readBuffer;
				if (readBuffer.Length != totalBytesRead)
				{
					buffer = new byte[totalBytesRead];
					Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
				}
				return buffer;
			}
			finally
			{
				source.Position = originalPosition;
			}
		}
	}
}