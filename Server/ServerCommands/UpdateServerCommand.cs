using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Windows.Forms;
using Swish.Server.IO;

namespace Swish.Server.ServerCommands
{
	public class UpdateServerCommand: IServerCommand
	{
		public const string UpdateServer2Command = "UpdateServer2";
		public string Name { get { return UpdateServer2Command; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			string directoryBase = VersionFunctions.NextDirectory;

			if (Directory.Exists(directoryBase))
			{
				Directory.Delete(directoryBase, true);
			}
			Directory.CreateDirectory(directoryBase);

			int fileCount = RawStreamIO.ReadInt(stream);

			for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
			{
				ReceiveFile(stream, directoryBase);
			}

			ProcessFunctions.RunProcess(VersionFunctions.NextExecutable, "HttpServer", null, true, TimeSpan.Zero, true, false, true);

			string endFileName = Path.Combine(Application.StartupPath, "End");
			File.WriteAllText(endFileName, string.Empty);

			TerminateServerCommand.Terminate(server);

			try
			{
				IanServerFunctions.WriteBlankOK(stream);
			} catch { }
		}

		public static void ReceiveFile(Stream stream, string baseDirectory)
		{
			string relatativeFileName = RawStreamIO.ReadString(stream);


			byte[] remoteMd5 = RawStreamIO.ReadByteArray(stream);

			string localFileName = Path.Combine(Application.StartupPath, relatativeFileName);
			string newFileName = Path.Combine(baseDirectory, relatativeFileName);

			if (File.Exists(localFileName))
			{
				byte[] localMd5 = FileFunctions.GetMd5(localFileName);

				if (EqualFunctions.Equal(remoteMd5, localMd5))
				{
					Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Use local file: " + relatativeFileName);
					RawStreamIO.Write(stream, false);
					File.Copy(localFileName, newFileName);
					return;
				}
			}
			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Receive file: " + relatativeFileName);
			RawStreamIO.Write(stream, true);

			string destinationDirectory = Path.GetDirectoryName(newFileName);
			FileFunctions.CreateDirectory(destinationDirectory,null);

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

		public static void SendFile(Stream stream, string fileName)
		{
			string relativeFileName = FileFunctions.GetRelativePath(fileName, Application.StartupPath);
			Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Send file: " + relativeFileName);
			RawStreamIO.Write(stream, relativeFileName);

			byte[] md5 = FileFunctions.GetMd5(fileName);
			RawStreamIO.Write(stream, md5);

			if (!RawStreamIO.ReadBool(stream))
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

		public static void SendFiles(string host, string directory)
		{
			List<string> files = new List<string>();
			List<string> directories = new List<string>();

			FileFunctions.GetFilesAndDirectories(out directories, out files, directory, null);

			for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
			{
				string fileName = files[fileIndex];
				string fileType = Path.GetExtension(fileName).ToLower();
				if (fileType == ".pdb" || fileName.ToLower().EndsWith("vshost.exe"))
				{
					files.RemoveAt(fileIndex);
					fileIndex--;
				}
			}

			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + UpdateServerCommand.UpdateServer2Command;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				RawStreamIO.Write(stream, files.Count);
				for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
				{
					string fileName = files[fileIndex];
					SendFile(stream, fileName);
				}
			}

		}
	}
}