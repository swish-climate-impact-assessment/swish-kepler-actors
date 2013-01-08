using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Swish
{
	public static class StreamFunctions
	{
		public static void ExportUntilFail(string fileName, Stream stream, ReportProgressFunction ReportProgress)
		{
			if (stream is System.Net.Sockets.NetworkStream)
			{
				throw new InvalidOperationException("Cannot use " + typeof(System.Net.Sockets.NetworkStream).FullName);
			}

			if (ReportProgress != null)
			{
				ReportProgress(-1, "Export: " + fileName);
			}

			using (FileStream destination = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				// CopyUntilFail(Stream destination, Stream source)
				{
					if (stream is MemoryStream && stream.Position == stream.Length)
					{
						throw new Exception("waiting on a MemoryStream will block execution indefinatly");
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

		public static string ToString(Stream source)
		{
			StringBuilder output = new StringBuilder();

			long position = source.Position;
			source.Position = 0;
			while (source.Position < source.Length)
			{
				output.Append((char)source.ReadByte());
			}

			return output.ToString();
		}

		public static void CopyStream(Stream destination, Stream source, long count)
		{
			int bufferSize = 8192;
			byte[] buffer = new byte[bufferSize];
			int readCount;
			while (count > 0)
			{
				if (count > bufferSize)
				{
					readCount = bufferSize;
				} else
				{
					readCount = (int)count;
				}

				int countRead = source.Read(buffer, 0, readCount);
				destination.Write(buffer, 0, countRead);
				if (countRead == 0)
				{
					Thread.Sleep(1);
				}
				count -= countRead;
			}
		}

		public static void ReadUntilComplete(Stream stream, byte[] buffer, int offset, int count)
		{
			if (stream is MemoryStream && stream.Length == stream.Position)
			{
				throw new Exception();
			}

			DateTime startTime = DateTime.Now;
			while (count > 0)
			{
				int countRead = stream.Read(buffer, offset, count);
				offset += countRead;
				count -= countRead;
				if (countRead == 0)
				{
					Thread.Sleep(1);
				}
			}
		}

		public static void Import(Stream stream, string fileName)
		{
			using (Stream fileIn = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				CopyStream(stream, fileIn, fileIn.Length);
			}
		}

	}
}