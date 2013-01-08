using System;
using System.Collections.Generic;
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
				ProcessResult result = ProcessFunctions.RunProcess(toolFileName, runArguments, Environment.CurrentDirectory, false, TimeSpan.Zero, true, true, true);

				Console.Write(result.Output);
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
