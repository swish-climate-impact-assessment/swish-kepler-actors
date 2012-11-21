using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Swish
{
	public static class AdapterFunctions
	{
		public static void Transpose(string inputFileName, string outputFileName)
		{
			if (!SwishFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			lines.Add("xpose, clear");
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
		}

		public static void Select(string inputFileName, string outputFileName, string expression)
		{
			if (!SwishFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new Exception("Expression missing");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			lines.Add("keep if " + expression);
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
		}

		public static void SelectColumns(string inputFileName, string outputFileName, List<string> variableNames)
		{
			if (!SwishFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			string line = "keep " + StataScriptFunctions.VariableList(variableNames);
			lines.Add(line);
			line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
		}

		public static void StataCommand(string inputFileName, string outputFileName, string command)
		{
			if (string.IsNullOrWhiteSpace(outputFileName))
			{
				outputFileName = SwishFunctions.TempoaryOutputFileName(".csv");
			}

			if (!SwishFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			/// create the do file
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			lines.Add(command);

			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);


			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
		}

		public static void Merge(string input1FileName, string input2FileName, List<string> variableNames, string outputFileName)
		{
			if (!SwishFunctions.FileExists(input1FileName))
			{
				throw new Exception("cannot find file \"" + input1FileName + "\"");
			}

			if (!SwishFunctions.FileExists(input2FileName))
			{
				throw new Exception("cannot find file \"" + input2FileName + "\"");
			}

			if (
			Path.GetFullPath(input1FileName) == Path.GetFullPath(outputFileName)
			|| Path.GetFullPath(input2FileName) == Path.GetFullPath(outputFileName)
			)
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			if (input1FileName == input2FileName)
			{
				throw new Exception("Cannot merge the same tables");
			}

			if (variableNames.Count == 0)
			{
				throw new Exception("Variables missing");
			}

			string doOutputFileName = Path.GetTempFileName() + ".csv";
			if (SwishFunctions.FileExists(doOutputFileName))
			{
				// Stata does not overwrite files
				File.Delete(doOutputFileName);
			}

			string intermediateFileName = Path.GetTempFileName() + ".dta";
			if (SwishFunctions.FileExists(intermediateFileName))
			{
				File.Delete(intermediateFileName);
			}

			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, input2FileName);

			string line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);
			line = StataScriptFunctions.SaveFileCommand(intermediateFileName);
			lines.Add(line);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, input1FileName);

			line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);
			lines.Add("merge 1:1 " + StataScriptFunctions.VariableList(variableNames) + " using \"" + intermediateFileName + "\"");
			lines.Add("drop " + StataScriptFunctions.MergeColumnName);
			line = StataScriptFunctions.SaveFileCommand(doOutputFileName);
			lines.Add(line);

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(doOutputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}

			/// move the output file
			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			File.Move(doOutputFileName, outputFileName);

			/// delete all the files not needed
			File.Delete(intermediateFileName);
		}

		public static string Append(string input1FileName, string input2FileName, string outputFileName)
		{

			if (string.IsNullOrWhiteSpace(outputFileName))
			{
				outputFileName = SwishFunctions.TempoaryOutputFileName(".csv");
			}

			if (!SwishFunctions.FileExists(input1FileName))
			{
				throw new Exception("cannot find file \"" + input1FileName + "\"");
			}

			if (!SwishFunctions.FileExists(input2FileName))
			{
				throw new Exception("cannot find file \"" + input2FileName + "\"");
			}

			if (
				Path.GetFullPath(input1FileName) == Path.GetFullPath(outputFileName)
				|| Path.GetFullPath(input2FileName) == Path.GetFullPath(outputFileName)
				)
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> lines = new List<string>();

			StataScriptFunctions.WriteHeadder(lines);

			string intermediateFileName = StataScriptFunctions.ConvertToStataFormat(lines, input2FileName);
			StataScriptFunctions.LoadFileCommand(lines, input1FileName);
			lines.Add("append using \"" + intermediateFileName + "\"");
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}

			/// delete all the files not needed
			File.Delete(intermediateFileName);
			return outputFileName;
		}

		public static double Collapse(string inputFileName, string variable, CollapseOpperation operation)
		{
			if (!SwishFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			string doOutputFileName = Path.GetTempFileName() + ".csv";
			if (SwishFunctions.FileExists(doOutputFileName))
			{
				// Stata does not overwrite files
				File.Delete(doOutputFileName);
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			// collapse (mean) mean=head4 (median) medium=head4, by(head6)

			lines.Add("collapse " + "(" + StataScriptFunctions.Write(operation) + ") " + variable + "_" + StataScriptFunctions.Write(operation) + "=" + variable);

			string line = StataScriptFunctions.SaveFileCommand(doOutputFileName);
			lines.Add(line);

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(doOutputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}

			string[] resultLines = File.ReadAllLines(doOutputFileName);

			double result = double.Parse(resultLines[1]);

			/// delete all the files not needed
			File.Delete(doOutputFileName);
			return result;
		}

		public static string Sort(string inputFileName, List<string> variableNames, string outputFileName)
		{
			if (string.IsNullOrWhiteSpace(outputFileName))
			{
				outputFileName = SwishFunctions.TempoaryOutputFileName(".csv");
			}

			if (!SwishFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (Path.GetFullPath(inputFileName) == Path.GetFullPath(outputFileName))
			{
				throw new Exception("Output cannot be the same as input");
			}

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			/// create the do file
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			/// sort varlist, stable
			/// add variables names
			string line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);

			line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
			return outputFileName;
		}

	}
}
