using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Swish
{
	public static class AdapterFunctions
	{
		public const string TemporaryFileNameOperation = "temporaryFileName";
		public const string ReplaceOperation = "replace";
		public const string TestOperation = "test";
		public const string DisplayOperation = "display";
		public const string AppendOperation = "append";
		public const string RemoveColumnsOperation = "removeColumns";
		public const string DoScriptOperation = "doScript";
		public const string CommandOperation = "stataCommand";
		public const string MergeOperation = "merge";
		public const string SaveOperation = "save";
		public const string SelectRecordsOperation = "selectRecords";
		public const string SelectCloumnsOperation = "selectCloumns";
		public const string SortOperation = "sort";
		public const string TransposeOperation = "transpose";

		public static void RunOperation(string operation, List<Tuple<string, string>> splitArguments)
		{
			switch (operation)
			{
			case TransposeOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.Transpose(inputFileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case SortOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "", splitArguments, true));
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					outputFileName = AdapterFunctions.Sort(inputFileName, variableNames, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case SelectCloumnsOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.SelectColumns(inputFileName, outputFileName, variableNames);
					Console.Write(outputFileName);
				}
				break;

			case SelectRecordsOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string expression = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "expression", splitArguments, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.Select(inputFileName, outputFileName, expression);
					Console.Write(outputFileName);
				}
				break;

			case SaveOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.SaveFile(inputFileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case MergeOperation:
				{
					string input1FileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "1", splitArguments, true));
					string input2FileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "2", splitArguments, true));
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);

					string keepMergeString = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "keepMerge", splitArguments, false));
					bool keepMerge;
					if (!string.IsNullOrWhiteSpace(keepMergeString))
					{
						keepMerge = bool.Parse(keepMergeString.ToLower());
					} else
					{
						keepMerge = false;
					}
					AdapterFunctions.Merge(input1FileName, input2FileName, variableNames, outputFileName, keepMerge);
					Console.Write(outputFileName);
				}
				break;

			case DoScriptOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "filename", splitArguments, true));
					string log = StataFunctions.RunScript(inputFileName, false);
					Console.Write(log);
				}
				break;

			case CommandOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "", splitArguments, true));
					string command = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "command", splitArguments, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					AdapterFunctions.StataCommand(inputFileName, outputFileName, command);
					Console.Write(outputFileName);
				}
				break;

			case AppendOperation:
				{
					string input1FileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "1", splitArguments, true));
					string input2FileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument + "2", splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					outputFileName = AdapterFunctions.Append(input1FileName, input2FileName, outputFileName);
					Console.Write(outputFileName);
				}
				break;

			case RemoveColumnsOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					List<string> variableNames = ArgumentFunctions.GetArgumentItems(ArgumentFunctions.ArgumentCharacter + "variables", splitArguments, true, true);
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					RemoveColumns(inputFileName, outputFileName, variableNames);
					Console.Write(outputFileName);
				}
				break;

			case DisplayOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					Display(inputFileName);
				}
				break;

			case TestOperation:
				{
					string argumentText = string.Empty;
					for (int argumentIndex = 0; argumentIndex < splitArguments.Count; argumentIndex++)
					{
						Tuple<string, string> argument = splitArguments[argumentIndex];
						argumentText += argument.Item1 + " " + argument.Item2 + " ";
					}
					SwishFunctions.MessageTextBox(argumentText);
				}
				break;

			case TemporaryFileNameOperation:
				{
					string fileName = Path.GetTempFileName();
					if (SwishFunctions.FileExists(fileName))
					{
						File.Delete(fileName);
					}
					Console.Write(fileName);
				}
				break;

			case ReplaceOperation:
				{
					string inputFileName = SwishFunctions.AdjustFileName(ArgumentFunctions.GetArgument(ArgumentFunctions.InputArgument, splitArguments, true));
					string outputFileName = ArgumentFunctions.GetOutputFileName(splitArguments);
					string condition = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "condition", splitArguments, true);
					string value = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "value", splitArguments, true);
					Replace(inputFileName, outputFileName, condition, value);
					Console.Write(outputFileName);
				}
				break;

			default:
				throw new Exception("Unknown operation \"" + operation + "\"");
			}
		}

		public static void Display(string inputFileName)
		{
			string extension = Path.GetExtension(inputFileName);
			string useFileName;
			if (extension.ToLower() == ".csv")
			{
				useFileName = inputFileName;
			} else
			{
				useFileName = SwishFunctions.TempoaryOutputFileName(".csv");
				SaveFile(inputFileName, useFileName);
			}

			string data = File.ReadAllText(useFileName).Replace(',', '\t');
			SwishFunctions.MessageTextBox("File: " + inputFileName + Environment.NewLine + data);
		}

		public static void RemoveColumns(string inputFileName, string outputFileName, List<string> variableNames)
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
			string line = "drop " + StataScriptFunctions.VariableList(variableNames);
			lines.Add(line);
			line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
		}

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

			StataScriptFunctions.WriteFooter(lines);

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

			StataScriptFunctions.WriteFooter(lines);

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

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
		}

		public static void StataCommand(string inputFileName, string outputFileName, string command)
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

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
		}

		public static void Merge(string input1FileName, string input2FileName, List<string> variableNames, string outputFileName, bool keepMergeColumn)
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
			lines.Add("merge 1:1 " + StataScriptFunctions.VariableList(variableNames) + " using \"" + intermediateFileName + "\", force ");
			if (!keepMergeColumn)
			{
				lines.Add("drop " + StataScriptFunctions.MergeColumnName);
			}
			line = StataScriptFunctions.SaveFileCommand(doOutputFileName);
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

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

			StataScriptFunctions.WriteFooter(lines);

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

			StataScriptFunctions.WriteFooter(lines);

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

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
			return outputFileName;
		}

		public static void Replace(string inputFileName, string outputFileName, string condition, string value)
		{
			if (!SwishFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(condition))
			{
				throw new Exception("condition missing");
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new Exception("value missing");
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
			//replace oldvar =exp [if] [in] [, nopromote]

			lines.Add("replace " + value + " if " + condition);
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}
		}

		public static void SaveFile(string inputFileName, string outputFileName)
		{
			if (!SwishFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			string inputFileExtension = Path.GetExtension(inputFileName);
			string outputFileExtension = Path.GetExtension(outputFileName);

			if (inputFileExtension.ToLower() == outputFileExtension.ToLower())
			{
				File.Copy(inputFileName, outputFileName);
				//return 0;
			}

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			if (SwishFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!SwishFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}

		}


	}
}
