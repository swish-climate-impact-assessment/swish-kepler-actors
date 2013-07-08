using System;
using System.Reflection;

namespace Swish.Adapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				Arguments splitArguments = ArgumentParser.Read(arguments);

				ExceptionFunctions.ForceVerbose = splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "verbose");

				string operation = splitArguments.String(ArgumentParser.OperationArgument, true);

				string output = OperationFunctions.RunOperation(operation, new OperationArguments(splitArguments));
				Console.Write(output);

				return 0;
			} catch (Exception error)
			{
				string message = string.Empty
					+ ArgumentParser.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose) + Environment.NewLine
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
