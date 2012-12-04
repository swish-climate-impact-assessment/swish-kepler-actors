using System;
using System.IO;
using System.Collections.Generic;

namespace Swish
{
	public static class StreamFunctions
	{
		public static void ExportUntilFail(string fileName, Stream stream)
		{
			if (stream is System.Net.Sockets.NetworkStream)
			{
				throw new Exception("");
			}

			using (FileStream destination = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				// CopyUntilFail(Stream destination, Stream source)
				{
					if (destination == null || stream == null)
					{
						throw new Exception("");
					}

					if (stream is MemoryStream && stream.Position == stream.Length)
					{
						throw new Exception("this will block execution");
					}

					int bufferSize = 8192;
					byte[] buffer = new byte[bufferSize];
					while (true)
					{
						int countRead = stream.Read(buffer, 0, bufferSize);
						destination.Write(buffer, 0, countRead);
						if (countRead != bufferSize)
						{
							break;
						}
					}
				}

				destination.Close();
			}
		}
	}
}