using System;
using System.IO;
using System.Diagnostics;

namespace Swish.Adapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			Arguments splitArguments = null;
			try
			{
				if (arguments.Length == 1 && File.Exists(arguments[0]) && Path.GetExtension(arguments[0]).ToLower() == ".kar")
				{
					Process.Start(KeplerFunctions.ExecutablePath, arguments[0]);
					return 0;
				}

				splitArguments = new Arguments(arguments);
				ExceptionFunctions.ForceVerbose = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "verbose");

				string operation = splitArguments.String(Arguments.OperationArgument, true);

				string output = AdapterFunctions.RunOperation(operation, new Adapters.AdapterArguments(splitArguments));
				Console.Write(output);

				return 0;
			} catch (Exception error)
			{
				string message = string.Empty
					+ Arguments.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose) + Environment.NewLine
					+ "Arguments: " + string.Join(" ", arguments) + Environment.NewLine;
				//if (ExceptionFunctions.ForceVerbose)
				//{
				//    message += ProcessFunctions.WriteProcessHeritage() + Environment.NewLine;
				//    message += ProcessFunctions.WriteSystemVariables() + Environment.NewLine;
				//}
				Console.Write(message);
				if (ExceptionFunctions.ForceVerbose)
				{
					SwishFunctions.MessageTextBox(message, false);
				}
				return -1;
			}
		}
	}
}
