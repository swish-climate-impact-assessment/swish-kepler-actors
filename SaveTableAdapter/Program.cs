using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Swish.SaveTableAdapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string inputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true);
				string outputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.OutputArgument + "", splitArguments, true);

				if (!File.Exists(inputFileName))
				{
					throw new Exception("cannot find file \"" + inputFileName + "\"");
				}

				if (File.Exists(outputFileName))
				{
					File.Delete(outputFileName);
				}

				string inputFileExtension = Path.GetExtension(inputFileName);
				string outputFileExtension = Path.GetExtension(outputFileName);

				if (inputFileExtension.ToLower() == outputFileExtension.ToLower())
				{
					File.Copy(inputFileName, outputFileName);
					return 0;
				}

				List<string> lines = new List<string>();
				StataScriptFunctions.WriteHeadder(lines);
				StataScriptFunctions.LoadFileCommand(lines, inputFileName);
				string line = StataScriptFunctions.SaveFileCommand(outputFileName);
				lines.Add(line);

				if (File.Exists(outputFileName))
				{
					File.Delete(outputFileName);
				}

				string log = StataFunctions.RunScript(lines, false);
				if (!File.Exists(outputFileName))
				{
					throw new Exception("Output file was not created" + log);
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
