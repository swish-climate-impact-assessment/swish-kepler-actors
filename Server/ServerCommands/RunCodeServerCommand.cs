using System;
using System.IO;
using System.Net.Sockets;
using Swish.Server.IO;

namespace Swish.Server.ServerCommands
{
	public class RunCodeServerCommand: IServerCommand
	{
		public const string Command = "RunCode";
		public string Name { get { return Command; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			string code = RawStreamIO.ReadString(stream);
			string output = RunCode(code);
			RawStreamIO.Write(stream, output);
		}

		public static string RunCode(string code)
		{
			string exeName = CSharpCompiler.MakeExecutable(code, true);
			ProcessResult result = ProcessFunctions.Run(exeName, string.Empty, null, false, new TimeSpan(0, 1, 0), false, true, true);
			return result.Output;
		}

		public static string RunCode(string host, string code)
		{
			string output;
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + RunCodeServerCommand.Command;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				RawStreamIO.Write(stream, code);

				output = RawStreamIO.ReadString(stream);
			}
			return output;
		}

	}
}