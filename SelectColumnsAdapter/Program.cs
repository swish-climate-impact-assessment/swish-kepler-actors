using System;
using System.Collections.Generic;

namespace Swish.SelectColumnsAdapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string inputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true);
				List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
				string outputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.OutputArgument + "", splitArguments, false);
				if (string.IsNullOrWhiteSpace(outputFileName))
				{
					outputFileName = SwishFunctions.TempoaryOutputFileName(".csv");
				}

				StataFunctions.SelectColumns(inputFileName, outputFileName, variableNames);

				Console.Write(outputFileName);
				return 0;
			} catch (Exception error)
			{
				string message = ArgumentFunctions.ErrorArgument + " " + ExceptionFunctions.WriteException(error, true);
				Console.Write(message);
				return -1;
			}
		}
	}
}
