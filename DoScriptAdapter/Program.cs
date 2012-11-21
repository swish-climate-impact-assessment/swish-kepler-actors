using System;
using System.Collections.Generic;

namespace Swish.DoScriptAdapter
{
	public static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "filename", splitArguments, true));

				string log = StataFunctions.RunScript(inputFileName, false);
				Console.Write(log);

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
