using System;
using System.Collections.Generic;
using System.IO;
using Swish.IO;
using Swish.Stata;

namespace Swish.Adapters
{
	public class StataOutputCommandAdapter: IOperation
	{
		public string Name { get { return "StataOutputCommand"; } }

		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string command = splitArguments.String(ArgumentParser.DefaultArgumentPrefix + "command", true);
			string log = StataCommand(inputFileName, command);
			return log;
		}

		public static string StataCommand(string inputFileName, string command)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			/// create the do file
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			lines.Add(command);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);

			return log;
		}

	}
}
