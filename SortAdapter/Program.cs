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
				string inputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "", splitArguments, true);
				string outputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.OutputArgument + "", splitArguments, true);
				List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);

				if (!File.Exists(inputFileName))
				{
					throw new Exception("cannot find file \"" + inputFileName + "\"");
				}

				if (File.Exists(outputFileName))
				{
					File.Delete(outputFileName);
				}

				/// create the do file
				List<string> lines = new List<string>();
				lines.Add("clear");
				string line = StataFunctions.LoadFileCommand(inputFileName);
				lines.Add(line);

				/// sort varlist, stable
				/// add variables names
				line = StataFunctions.SortCommand(variableNames);
				lines.Add(line);

				line = StataFunctions.SaveFileCommand(outputFileName);
				lines.Add(line);

				if (File.Exists(outputFileName))
				{
					File.Delete(outputFileName);
				}

				StataFunctions.RunScript(lines, false);

				if (!File.Exists(outputFileName))
				{
					throw new Exception("Output file was not created");
				}

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
