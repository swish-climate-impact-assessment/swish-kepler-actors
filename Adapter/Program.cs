using System;

namespace Swish.Adapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				Arguments splitArguments = new Arguments(arguments);
				ExceptionFunctions.ForceVerbose = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "verbose");
				string operation = splitArguments.String(Arguments.OperationArgument, true);

				AdapterFunctions.RunOperation(operation, splitArguments);
				return 0;
			} catch (Exception error)
			{
				string message = Arguments.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose);
				if (ExceptionFunctions.ForceVerbose)
				{
					message += ProcessFunctions.WriteProcessHeritage();
					message += ProcessFunctions.WriteSystemVariables();
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
