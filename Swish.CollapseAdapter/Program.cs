using System;
using Swish.IO;
using Swish.Stata;

namespace Swish.CollapseAdapterExecutable
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				Arguments splitArguments = ArgumentParser.Read(arguments);
				ExceptionFunctions.ForceVerbose = splitArguments.Exists(ArgumentParser.DefaultArgumentPrefix + "verbose");
				string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(ArgumentParser.InputArgument, true));
				string variable = splitArguments.String(ArgumentParser.DefaultArgumentPrefix + "variable", true);
				string operation = splitArguments.String(ArgumentParser.DefaultArgumentPrefix + ArgumentParser.OperationArgument, false);

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
				string message = ArgumentParser.ErrorArgument + " " + ExceptionFunctions.Write(error, !ExceptionFunctions.ForceVerbose);
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