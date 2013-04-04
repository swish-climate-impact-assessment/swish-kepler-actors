using System;

namespace Swish.CollapseAdapterExecutable
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				Arguments splitArguments = new Arguments(arguments);
				ExceptionFunctions.ForceVerbose = splitArguments.Exists(Arguments.DefaultArgumentPrefix + "verbose");
				string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
				string variable = splitArguments.String(Arguments.DefaultArgumentPrefix + "variable", true);
				string operation = splitArguments.String(Arguments.DefaultArgumentPrefix + "operation", false);

				if (string.IsNullOrWhiteSpace(operation))
				{
					operation = "mean";
				}

				CollapseOpperation operationCode = (CollapseOpperation)Enum.Parse(typeof(CollapseOpperation), operation, true);

				double result = Swish.Adapters.CollapseAdapter.Collapse(inputFileName, variable, operationCode);

				Console.Write(result.ToString());
				return 0;
			} catch (Exception error)
			{
				string message = Arguments.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose);
				//if (ExceptionFunctions.ForceVerbose)
				//{
				//    message += ProcessFunctions.WriteProcessHeritage();
				//    message += ProcessFunctions.WriteSystemVariables();
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
