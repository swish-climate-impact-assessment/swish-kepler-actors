using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Swish.SortAdapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				/// get the arguments

				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "", splitArguments, true));
				List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
				string outputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.OutputArgument + "", splitArguments, false));

				outputFileName = AdapterFunctions.Sort(inputFileName, variableNames, outputFileName);

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
