using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Windows.Forms;
using Swish.Server.IO;

namespace Swish.Server.ServerCommands
{
	public class RunProcessCommand: IServerCommand
	{
		public const string Command = "RunProcess";
		public string Name { get { return Command; } }

		public void Run(string command, Stream stream, TcpServer server)
		{
			string executableName = RawStreamIO.ReadString(stream);
			string arguments = RawStreamIO.ReadString(stream);

			string workingDirectory = RawStreamIO.ReadString(stream);
			bool returnBeforeExit = RawStreamIO.ReadBool(stream);
			TimeSpan timeout = RawStreamIO.ReadTimeSpan(stream);
			bool console = RawStreamIO.ReadBool(stream);
			bool readOutput = RawStreamIO.ReadBool(stream);
			bool dotNet = RawStreamIO.ReadBool(stream);

			executableName = FileFunctions.AdjustFileName(executableName);
			ProcessResult result = ProcessFunctions.Run(executableName, arguments, workingDirectory, returnBeforeExit, timeout, console, readOutput, dotNet);

			if (returnBeforeExit)
			{
				return;
			}

			RawStreamIO.Write(stream, result.ExitCode);

			if (readOutput)
			{
				RawStreamIO.Write(stream, result.Output);
				RawStreamIO.Write(stream, result.Error);
			}
		}

		public static ProcessResult Run(string host, string executableName, string arguments, string workingDirectory, bool returnBeforeExit, TimeSpan timeout, bool console, bool readOutput, bool dotNet)
		{
			ProcessResult result;
			int port = 39390;
			using (TcpClient connection = new TcpClient(host, port))
			using (NetworkStream stream = connection.GetStream())
			{
				string url = "/command?" + UpdateServerCommand.Command;

				IanServerFunctions.WriteLine(stream, "GET" + " " + url + " " + "HTTP1.1");
				IanServerFunctions.WriteLine(stream, string.Empty);

				RawStreamIO.Write(stream, executableName);
				RawStreamIO.Write(stream, arguments);

				RawStreamIO.Write(stream, workingDirectory);
				RawStreamIO.Write(stream, returnBeforeExit);
				RawStreamIO.Write(stream, timeout);
				RawStreamIO.Write(stream, console);
				RawStreamIO.Write(stream, readOutput);
				RawStreamIO.Write(stream, dotNet);

				if (returnBeforeExit)
				{
					return null;
				}

				result = new ProcessResult();
				result.ExitCode = RawStreamIO.ReadInt(stream);

				if (readOutput)
				{
					result.Output = RawStreamIO.ReadString(stream);
					result.Error = RawStreamIO.ReadString(stream);
				}

			}
			return result;
		}
	}
}
