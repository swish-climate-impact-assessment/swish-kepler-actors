using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class UniqueValuesAdapter: IAdapter
	{
		public string Name { get { return "UniqueValues"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string variableName = splitArguments.VariableName();
			List<string> values = Values(inputFileName, variableName);

			Console.WriteLine(string.Join(" " + Environment.NewLine + " ", values));
		}

		public static List<string> Values(string inputFileName, string variableName)
		{
			if (string.IsNullOrWhiteSpace(inputFileName))
			{
				throw new Exception("inputFileName missing");
			}

			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("Variable name missing");
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			lines.Add("tabulate " + variableName);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);

			List<string> values = ParseValues(log);

			return values;
		}

		private static List<string> ParseValues(string log)
		{
			List<string> values = new List<string>();
			List<string> lines = new List<string>(log.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

			/// Other  lines

			///    factorA |      Freq.     Percent        Cum.
			///------------+-----------------------------------
			///          A |          1       25.00       25.00
			///          B |          1       25.00       50.00
			///          C |          1       25.00       75.00
			///          D |          1       25.00      100.00
			///------------+-----------------------------------
			///      Total |          4      100.00

			/// Other lines

			int lineIndex = 0;
			List<string> linesRead;
			ReadUntilStart(lines, out linesRead, ref lineIndex, ". tabulate");
			ReadUntilStart(lines, out linesRead, ref lineIndex, "------------+-----------------------------------");
			ReadUntilStart(lines, out linesRead, ref lineIndex, "------------+-----------------------------------");
			if (linesRead.Count == 0)
			{
				throw new Exception("");
			}

			for (int variableIndex = 0; variableIndex < linesRead.Count; variableIndex++)
			{
				string line = linesRead[variableIndex].Trim();
				string[] fragments = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				string value = fragments[0];

				values.Add(value);
			}

			return values;
		}

		private static void ReadUntilStart(List<string> lines, out List<string> linesRead, ref int lineIndex, string searchLine)
		{
			linesRead = new List<string>();
			while (true)
			{
				if (lineIndex == lines.Count)
				{
					throw new Exception("Could not find line \"" + searchLine + "\"" + Environment.NewLine + string.Join(Environment.NewLine, lines));
				}

				string line = lines[lineIndex++];
				if (line.Trim().StartsWith(searchLine))
				{
					break;
				}
				linesRead.Add(line);
			}
		}
	}
}
