using System.IO;
using Swish.Server.IO;
using System.Net.Sockets;

namespace Swish.Server.ServerCommands
{
	class CreateDirectoryServerCommand: IServerCommand
	{
		public const string CreateDirectoryCommand = "CreateDirectory";
		public string Name { get { return CreateDirectoryCommand; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			string directory = RawStreamIO.ReadString(stream);
			Directory.CreateDirectory(directory);
		}

		public static void CreateDirectory(string host, string directory)
		{
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + CreateDirectoryCommand;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				RawStreamIO.Write(stream, directory);
			}
		}

	}
}