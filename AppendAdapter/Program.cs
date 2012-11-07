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
				string input1FileName = ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "1", splitArguments, true);
				string input2FileName = ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "2", splitArguments, true);
				string outputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.OutputArgument + "", splitArguments, true);

				if (!File.Exists(input1FileName))
				{
					throw new Exception("cannot find file \"" + input1FileName + "\"");
				}

				if (!File.Exists(input2FileName))
				{
					throw new Exception("cannot find file \"" + input2FileName + "\"");
				}

				if (File.Exists(outputFileName))
				{
					File.Delete(outputFileName);
				}

				List<string> lines = new List<string>();
				string intermediateFileName = StataFunctions.ConvertToStataFormat(lines,input2FileName);
				lines.Add("clear");
				string line = StataFunctions.LoadFileCommand(input1FileName);
				lines.Add(line);
				lines.Add("append using \"" + intermediateFileName + "\"");
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

				/// delete all the files not needed
				File.Delete(intermediateFileName);

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
