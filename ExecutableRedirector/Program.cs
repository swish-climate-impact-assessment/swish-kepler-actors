using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Swish.ExecutableRedirector
{
	public static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				List<string> locations = FileFunctions.Locations();
				string executableName = Path.GetFileName(Application.ExecutablePath);

				string toolFileName = FileFunctions.ResloveFileName(executableName, locations, true, false);

				if (toolFileName == Application.ExecutablePath)
				{
					throw new Exception("Current executable path mistaken for path to " + executableName);
				}

				string runArguments = string.Join(" ", arguments);
				//TimeSpan timeOut = new TimeSpan(0, 0, 24);
				TimeSpan timeOut = TimeSpan.Zero;
				ProcessResult result = ProcessFunctions.Run(toolFileName, runArguments, Environment.CurrentDirectory, false, timeOut, false, true, true);

				Console.Write(result.Output);
				if (!string.IsNullOrWhiteSpace(result.Error))
				{
					using (StreamWriter _stream = new StreamWriter(Process.GetCurrentProcess().StandardError.BaseStream))
					{
						_stream.Write(result.Error);
					}
				}
				return result.ExitCode;
			} catch (Exception error)
			{
				string message = Arguments.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose);
				message += ProcessFunctions.WriteProcessHeritage();
				message += ProcessFunctions.WriteSystemVariables();
				SwishFunctions.MessageTextBox(message, false);
				Console.Write(message);
				return -1;
			}
		}


	}
}
