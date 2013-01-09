using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Server.ServerCommands
{
	public static class ServerCommandFunctions
	{
		private static List<IServerCommand> operations = new List<IServerCommand>();

		static ServerCommandFunctions()
		{
			operations.Add(new UpdateServerCommand());
			operations.Add(new RunCodeServerCommand());
			operations.Add(new CreateDirectoryServerCommand());
			operations.Add(new TerminateServerCommand());
			operations.Add(new SendFileServerCommand());
			operations.Add(new GetVersionServerCommand());
			operations.Add(new RunProcessCommand());
		}

		public static void ServeCommand(Stream stream, string url, TcpServer server)
		{
			int stringIndex = url.IndexOf("?");
			if (stringIndex < 0)
			{
				IanServerFunctions.Write404(stream);
				return;
			}

			string command = url.Substring(stringIndex + 1);

			bool run = false;
			for (int operationIndex = 0; operationIndex < operations.Count; operationIndex++)
			{
				IServerCommand runCommand = operations[operationIndex];
				if (runCommand.Name.ToLower() == command.ToLower())
				{
					string commandArguments = command.Substring(runCommand.Name.Length);

					runCommand.Run(commandArguments, stream, server);
					run = true;
					break;
				}
			}
			if (!run)
			{
				Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "Command not found: \"" + command + "\"");
			}

			return;
			//WriteOk(stream, "text/html");
			//WriteLine(stream, "<html><body>");
			//WriteLine(stream, command);
			//WriteLine(stream, "</body></html>");
		}


	}
}