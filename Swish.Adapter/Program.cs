using System;
using System.Reflection;

namespace Swish.Adapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			Arguments splitArguments = null;
			try
			{
				splitArguments = new Arguments(arguments);

				ExceptionFunctions.ForceVerbose = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "verbose");

				string operation = splitArguments.String(Arguments.OperationArgument, true);

				string output = OperationFunctions.RunOperation(operation, new OperationArguments(splitArguments));
				Console.Write(output);

				return 0;
			} catch (Exception error)
			{
				string message = string.Empty
					+ Arguments.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose) + Environment.NewLine
					+ "Arguments: " + string.Join(" ", arguments) + Environment.NewLine;
				if (ExceptionFunctions.ForceVerbose)
				{
					string assemblyMessage = "Loaded assemblies" + Environment.NewLine;
					for (int assemblyIndex = 0; assemblyIndex < TypeFunctions.Assemblies.Count; assemblyIndex++)
					{
						Assembly assembly = TypeFunctions.Assemblies[assemblyIndex];
						assemblyMessage += assembly.FullName + Environment.NewLine;
					}

					message += assemblyMessage;
					message += ProcessFunctions.WriteProcessHeritage() + Environment.NewLine;
					message += ProcessFunctions.WriteSystemVariables() + Environment.NewLine;

				}
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
