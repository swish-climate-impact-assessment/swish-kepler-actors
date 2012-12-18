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
				Arguments splitArguments = new Arguments(arguments);
				string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
				string variable = splitArguments.String(Arguments.DefaultArgumentPrefix + "variable", true);
				string operation = splitArguments.String(Arguments.DefaultArgumentPrefix + "operation", false);

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
				string message = Arguments.ErrorArgument + " " + ExceptionFunctions.Write(error, true);
				Console.Write(message);
				return -1;
			}
		}

	}
}
