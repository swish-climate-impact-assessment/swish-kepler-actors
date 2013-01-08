using System;
using System.IO;
using System.Windows.Forms;
using LibraryTypes.BootStrap;
using Swish.Server.IO;
using System.Net.Sockets;

namespace Swish.Server.ServerCommands
{
	public class RunCodeServerCommand: IServerCommand
	{
		public const string RunCodeCommand = "RunCode";
		public string Name { get { return RunCodeCommand; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			string code = RawStreamIO.ReadString(stream);
			string output = RunCode(code);
			RawStreamIO.Write(stream, output);
		}

		private static string directoryBase = Path.GetTempFileName() + "_dir";
		private static int directoryBaseCount = 0;
		public static string RunCode(string code)
		{
			string codeDirectory = directoryBase + directoryBaseCount.ToString();
			directoryBaseCount++;

			string baseFileName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);

			Directory.CreateDirectory(codeDirectory);
			string codeFileName = Path.Combine(codeDirectory, baseFileName + ".cs");
			File.WriteAllText(codeFileName, code);

			string exeName = Path.Combine(codeDirectory, baseFileName + ".exe");
			CSharpCompiler._Compile(codeDirectory, exeName, false);

			ProcessResult result = ProcessFunctions.RunProcess(exeName, string.Empty, null, false, new TimeSpan(0, 1, 0), false, true, true);

			return result.Output;
		}

		public static string RunCode(string host, string code)
		{
			string output;
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + RunCodeServerCommand.RunCodeCommand;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				RawStreamIO.Write(stream, code);

				output = RawStreamIO.ReadString(stream);
			}
			return output;
		}

	}
}