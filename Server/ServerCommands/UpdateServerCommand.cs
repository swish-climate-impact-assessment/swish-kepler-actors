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
		public const string Command = "UpdateServer2";
		public string Name { get { return Command; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			string directory = VersionFunctions.NextDirectory;

			if (Directory.Exists(directory))
			{
				Directory.Delete(directory, true);
			}
			Directory.CreateDirectory(directory);

			int fileCount = RawStreamIO.ReadInt(stream);
			for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
			{
				SendFileServerCommand.ReceiveFile(stream, directory);
			}

			ProcessFunctions.Run(VersionFunctions.NextExecutable, "HttpServer", null, true, TimeSpan.Zero, true, false, true);

			string endFileName = Path.Combine(Application.StartupPath, "End");
			File.WriteAllText(endFileName, string.Empty);

			TerminateServerCommand.Terminate(server);

			try
			{
				IanServerFunctions.WriteBlankOK(stream);
			} catch { }
		}

		public static void UpdateServer2(string host, string directory)
		{
			string versionDirectory = GetVersionServerCommand.GetVersion(host, GetVersionServerCommand.VersionType.Next);
			CreateDirectoryServerCommand.CreateDirectory(host, versionDirectory);

			List<string> files = GetBinaries(directory);
			for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
			{
				string fileName = files[fileIndex];
				SendFileServerCommand.SendFile(host, fileName, directory);
			}

			string executableName = Path.Combine(versionDirectory, "Swish.Server.exe");
			RunProcessCommand.Run(host, executableName, "HttpServer", null, true, TimeSpan.Zero, true, false, true);

			TerminateServerCommand.Terminate(host);
		}

		public static void UpdateServer(string host, string directory)
		{
			List<string> files = GetBinaries(directory);

			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + UpdateServerCommand.Command;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				RawStreamIO.Write(stream, files.Count);
				for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
				{
					string fileName = files[fileIndex];
					SendFileServerCommand.SendFile(stream, fileName, directory);
				}
			}

		}

		private static List<string> GetBinaries(string directory)
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
			return files;
		}
	}
}