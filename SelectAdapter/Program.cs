﻿using System;
using System.Collections.Generic;

namespace Swish.SelectAdapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string inputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true);
				string outputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.OutputArgument, splitArguments, true);
				string expression = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "expression", splitArguments, true);

				StataFunctions.Select(inputFileName, outputFileName, expression);

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
