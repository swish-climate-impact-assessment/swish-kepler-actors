﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Swish.CommandAdapter
{
	public static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				/// get the arguments

				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string inputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "", splitArguments, true);
				string outputFileName = ArgumentFunctions.GetArgument(ArgumentFunctions.OutputArgument + "", splitArguments, true);
				string command = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "command", splitArguments, true);

				if (!File.Exists(inputFileName))
				{
					throw new Exception("cannot find file \"" + inputFileName + "\"");
				}

				if (File.Exists(outputFileName))
				{
					File.Delete(outputFileName);
				}

				List<string> lines = CreateDoFile(inputFileName, outputFileName, command);

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

		public static List<string> CreateDoFile(string input, string output, string command)
		{
			/// create the do file
			List<string> lines = new List<string>();
			lines.Add("clear");
			string line = StataFunctions.LoadFileCommand(input);
			lines.Add(line);

			lines.Add(command);

			 line = StataFunctions.SaveFileCommand(output);
			lines.Add(line);
			return lines;
		}


	}
}
