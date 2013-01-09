using System.IO;
using Swish.Server.IO;
using System.Net.Sockets;
using System;

namespace Swish.Server.ServerCommands
{
	class GetVersionServerCommand: IServerCommand
	{
		public const string Command = "GetVersion";
		public string Name { get { return Command; } }

		public enum VersionType
		{
			Unknown = 0,
			Best,
			Current,
			Next,
		}

		public void Run(string command, Stream stream, TcpServer server)
		{
			VersionType type = RawStreamIO.ReadEnum<VersionType>(stream);
			string version;
			switch (type)
			{
			case VersionType.Best:
				version = VersionFunctions.BestDirectory;
				break;

			case VersionType.Current:
				version = VersionFunctions.CurrentDirectory;
				break;

			case VersionType.Next:
				version = VersionFunctions.NextDirectory;
				break;

			default:
			case VersionType.Unknown:
				throw new Exception();
			}

			RawStreamIO.Write(stream, version);
		}

		public static string GetVersion(string host, VersionType type)
		{
			string version;
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + Command;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				RawStreamIO.WriteEnum<VersionType>(stream, type);

			version=	RawStreamIO.ReadString(stream);
			}
			return version;
		}

	}
}