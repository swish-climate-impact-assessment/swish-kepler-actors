using System.IO;
using Swish.Server.IO;
using System.Net.Sockets;

namespace Swish.Server.ServerCommands
{
	class CreateDirectoryServerCommand: IServerCommand
	{
		public const string Command = "CreateDirectory";
		public string Name { get { return Command; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			string directory = RawStreamIO.ReadString(stream);
			if (Directory.Exists(directory))
			{
				return;
			}
			Directory.CreateDirectory(directory);
		}

		public static void CreateDirectory(string host, string directory)
		{
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + Command;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				RawStreamIO.Write(stream, directory);
			}
		}

	}
}