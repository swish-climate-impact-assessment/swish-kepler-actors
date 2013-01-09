using System.IO;
using Swish.Server.IO;
using System.Net.Sockets;
using System;
using System.Windows.Forms;
using System.IO.Compression;

namespace Swish.Server.ServerCommands
{
	public class SendFileServerCommand: IServerCommand
	{
		public const string Command = "WriteFile";
		public string Name { get { return Command; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			string directory = Application.StartupPath;
			SendFileServerCommand.ReceiveFile(stream, directory);
		}

		public static void SendFile(string host, string fileName, string directory)
		{
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + Command;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				SendFile(stream, fileName, directory);
			}
		}

		public static void ReceiveFile(Stream stream, string directory)
		{
			string relatativeFileName = RawStreamIO.ReadString(stream);
			byte[] remoteMd5 = RawStreamIO.ReadByteArray(stream);

			string localFileName = Path.Combine(Application.StartupPath, relatativeFileName);
			string newFileName = Path.Combine(directory, relatativeFileName);

			if (!(stream is MemoryStream))
			{
				if (File.Exists(localFileName))
				{
					byte[] localMd5 = FileFunctions.GetMd5(localFileName);

					if (EqualFunctions.Equal(remoteMd5, localMd5))
					{
						RawStreamIO.Write(stream, false);

						if (newFileName == localFileName)
						{
							return;
						}
						File.Copy(localFileName, newFileName);
						return;
					}
				}
				RawStreamIO.Write(stream, true);
			}

			if (newFileName == localFileName)
			{
				FileFunctions.DeleteFile(localFileName, null);
			}

			string destinationDirectory = Path.GetDirectoryName(newFileName);
			FileFunctions.CreateDirectory(destinationDirectory, null);

			long length = RawStreamIO.ReadLong(stream);

			if (length == 0)
			{
				using (File.Create(newFileName)) { }
				return;
			}

			using (ChunkStream __stream = new ChunkStream(stream, true, true))
			using (GZipStream _stream = new GZipStream(__stream, CompressionMode.Decompress, true))
			using (FileStream fileStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write))
			{
				StreamFunctions.CopyStream(fileStream, _stream, length);
			}
		}

		public static void SendFile(Stream stream, string fileName, string directory)
		{
			string relativeFileName = FileFunctions.GetRelativePath(fileName, directory);
			RawStreamIO.Write(stream, relativeFileName);

			byte[] md5 = FileFunctions.GetMd5(fileName);
			RawStreamIO.Write(stream, md5);

			if (!(stream is MemoryStream) && !RawStreamIO.ReadBool(stream))
			{
				return;
			}

			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				RawStreamIO.Write(stream, fileStream.Length);

				if (fileStream.Length == 0)
				{
					return;
				}

				using (ChunkStream __stream = new ChunkStream(stream, false, true))
				using (GZipStream _stream = new GZipStream(__stream, CompressionMode.Compress, true))
				{
					StreamFunctions.CopyStream(_stream, fileStream, fileStream.Length);
				}
			}
		}


	}
}