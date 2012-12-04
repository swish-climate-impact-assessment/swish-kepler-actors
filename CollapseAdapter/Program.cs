using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Swish.CollapseAdapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string inputFileName = FileFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "", splitArguments, true));
				string variable = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "variable", splitArguments, true);
				string operation = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "operation", splitArguments, false);

				if (string.IsNullOrWhiteSpace(operation))
				{
					operation = "mean";
				}

				CollapseOpperation operationCode = (CollapseOpperation)Enum.Parse(typeof(CollapseOpperation), operation, true);

				double result = AdapterFunctions.Collapse(inputFileName, variable, operationCode);

				Console.Write(result.ToString());
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
