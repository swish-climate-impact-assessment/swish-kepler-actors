using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class CountOfPreviousDaysAdapter2: IAdapter
	{
		public string Name { get { return "CountOfPreviousDaysLag"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string variableName = splitArguments.VariableName();
			string dateVariableName = splitArguments.String(Arguments.DefaultArgumentPrefix + "dateVariable", true);
			int days = splitArguments.Int("days", true);
			string outputFileName = splitArguments.OutputFileName(SwishFunctions.DataFileExtension);
			string resultVariableName = splitArguments.ResultVariableName();

			Count(inputFileName, outputFileName, variableName, days, dateVariableName, resultVariableName);
			Console.Write(outputFileName);
		}

		public static void Count(string inputFileName, string outputFileName, string variableName, int days, string dateVariableName, string resultVariableName)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("Variable missing");
			}

			if (string.IsNullOrWhiteSpace(dateVariableName))
			{
				throw new Exception("Date variable missing");
			}

			string extension = Path.GetExtension(outputFileName);
			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(extension);

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			SortedList<string, string> variableTypes = VariableNamesAdapter.VariableInformation(inputFileName);

			if (!variableTypes.ContainsKey(dateVariableName))
			{
				throw new Exception("Date variable \"" + dateVariableName + "\" not found in data");
			}

			string dateVariableNameUsed;
			if (variableTypes[dateVariableName].StartsWith("str"))
			{
				dateVariableNameUsed = StataScriptFunctions.TemporaryVariableName();
				lines.Add(" generate " + dateVariableNameUsed + " = " + "date(" + dateVariableName + ", \"DMY\")");
				lines.Add("format " + dateVariableNameUsed + " %td");
			} else
			{
				dateVariableNameUsed = dateVariableName;
			}

			lines.Add("tsset " + dateVariableNameUsed + " ");

			string expression = string.Empty;
			for (int index = 0; index < days; index++)
			{
				expression += "L" + (index + 1).ToString() + "." + variableName;
				if (index + 1 < days)
				{
					expression += " + ";
				}
			}

			StataScriptFunctions.Generate(lines, StataDataType.Double, resultVariableName, expression);

			if (dateVariableNameUsed != dateVariableName)
			{
				lines.Add("drop " + dateVariableNameUsed);
			}

			StataScriptFunctions.SaveFileCommand(lines, intermaidateOutput);

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
