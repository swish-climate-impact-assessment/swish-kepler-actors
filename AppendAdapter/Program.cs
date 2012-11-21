using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Swish.AppendAdapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string input1FileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "1", splitArguments, true));
				string input2FileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "2", splitArguments, true));
				string outputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.OutputArgument + "", splitArguments, false));

				outputFileName = AdapterFunctions.Append(input1FileName, input2FileName, outputFileName);

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
