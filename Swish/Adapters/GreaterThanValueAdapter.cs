using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class GreaterThanValueAdapter
	{
		public string Name { get { return "greaterThanValue"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
			string outputFileName = splitArguments.OutputFileName();
			string variableName = splitArguments.String(Arguments.DefaultArgumentPrefix + "variableName", true);
			string value = splitArguments.String(Arguments.DefaultArgumentPrefix + "value", true);
			List<string> keepVariableNames = splitArguments.StringList(Arguments.DefaultArgumentPrefix + "keepVariables", true, true);

			GreaterThan(inputFileName, outputFileName, variableName, value, keepVariableNames);
			Console.Write(outputFileName);
		}

		private static void GreaterThan(string inputFileName, string outputFileName, string variableName, string value, List<string> keepVariableNames)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(outputFileName))
			{
				throw new Exception("Variable missing");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("Variable missing");
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new Exception("Expression missing");
			}

			string extension = Path.GetExtension(outputFileName);
			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(extension);

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			if (DateTime.MinValue < DateTime.MaxValue)
			{
				throw new Exception(" do stuff here ...");
			}

			string line = "generate byte flag = " + variableName + " > " + value;
			lines.Add(line);
			line = "keep flag " + string.Join(" ", keepVariableNames);
			lines.Add(line);

			line = StataScriptFunctions.SaveFileCommand(intermaidateOutput);
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(intermaidateOutput))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			File.Move(intermaidateOutput, outputFileName);
		}

	}
}