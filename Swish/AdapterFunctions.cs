﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Swish.Adapters;
using Swish.StataOperations;
using System.Windows.Forms;

namespace Swish
{
	public static class AdapterFunctions
	{
		public const string WorkingVariableName = "Working";
		internal static bool RecordDoScript = false;

		public static void RunOperation(string operationName, AdapterArguments splitArguments)
		{
			string scriptFile = FindScriptTemplate(operationName);
			if (!string.IsNullOrWhiteSpace(scriptFile))
			{
				RunScriptTemplate(operationName, scriptFile, splitArguments);
				return;
			}

			IAdapter adapter = FindAdapter(operationName);
			if (adapter != null)
			{
				adapter.Run(splitArguments);
				return;
			}

			IStataBasedOperation stataOperation = FindStataOperation(operationName);
			if (stataOperation != null)
			{
				RunStataOperation(stataOperation, splitArguments);
				return;
			}

			throw new Exception("No operation found for \"" + operationName + "\"");
		}

		private static void RunScriptTemplate(string operationName, string scriptFile, AdapterArguments adapterArguments)
		{
			string[] _lines = File.ReadAllLines(scriptFile);
			List<string> lines = new List<string>(_lines);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);

			string outputFileName;
			SortedList<string, string> inputFileNames;

			List<string> newLines;
			List<Tuple<string, string>> symbols;
			ResloveSymbols(out outputFileName, out inputFileNames, out newLines, out symbols, lines, intermediateFileName, adapterArguments);
			lines = SwishFunctions.ConvertLines(newLines, symbols, null);

			string tempScriptFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DoFileExtension);
			File.WriteAllLines(tempScriptFileName, lines);

			string log = StataFunctions.RunScript(tempScriptFileName, false);

			if (!FileFunctions.FileExists(intermediateFileName))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "\"" + scriptFile + "\"" + Environment.NewLine);
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			File.Move(intermediateFileName, outputFileName);

			/// delete script file
			File.Delete(tempScriptFileName);

			if (ExceptionFunctions.ForceVerbose)
			{
				ExportMetadata(operationName, adapterArguments, inputFileNames, outputFileName, lines);
			}

			Console.WriteLine(outputFileName);
		}

		public const string OutputType = "output";
		public const string InputType = "input";
		public const string VariableNamesType = "variableNames";
		public const string BoolType = "bool";
		public const string TemporaryFileType = "temporaryFile";

		private static void ResloveSymbols(
			out string outputFileName,
			out SortedList<string, string> inputFileNames,
			out List<string> newLines,
			out List<Tuple<string, string>> symbols,
			List<string> lines, string intermediateFileName, AdapterArguments adapterArguments)
		{
			newLines = new List<string>();
			symbols = new List<Tuple<string, string>>();
			symbols.Add(new Tuple<string, string>("%UserProfile%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
			symbols.Add(new Tuple<string, string>("%StartupPath%", Application.StartupPath));

			outputFileName = string.Empty;
			inputFileNames = new SortedList<string, string>();
			for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
			{
				string _line = lines[lineIndex];
				string line = _line;
				string buffer;
				StringIO.SkipWhiteSpace(out buffer, ref line);
				if (!StringIO.TryRead("//", ref line))
				{
					newLines.Add(line);
					continue;
				}

				StringIO.SkipWhiteSpace(out buffer, ref line);
				if (!StringIO.TryRead("define", ref line))
				{
					newLines.Add(line);
					continue;
				}

				StringIO.SkipWhiteSpace(out buffer, ref line);
				string type;
				if (!StringIO.TryReadUntill(out type, out buffer, new string[] { " ", "\t", "\r", "\n" }, ref line))
				{
					throw new Exception("Could not read line: \"" + _line + "\"");
				}

				bool optional;
				StringIO.SkipWhiteSpace(out buffer, ref line);
				optional = StringIO.TryRead("optional", ref line);

				StringIO.SkipWhiteSpace(out buffer, ref line);
				string name;
				if (StringIO.TryReadUntill(out buffer, out name, new string[] { " ", "\t", "\r", "\n" }, ref line))
				{
				} else
				{
					name = line;
					line = string.Empty;
				}
				if (string.IsNullOrWhiteSpace(name))
				{
					throw new Exception("Could not read line: \"" + _line + "\"");
				}

				string defaultValue;
				StringIO.SkipWhiteSpace(out buffer, ref line);
				if (StringIO.TryReadUntill(out buffer, out defaultValue, new string[] { " ", "\t", "\r", "\n" }, ref line))
				{
				} else
				{
					defaultValue = line;
				}

				switch (type)
				{
				case TemporaryFileType:
					{
						string fileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
						symbols.Add(new Tuple<string, string>(name, fileName));
					} break;

				case OutputType:
					{
						string _outputFileName = adapterArguments.String("%Output%" + "", false);
						if (string.IsNullOrWhiteSpace(_outputFileName) || _outputFileName.ToLower() == "none" || _outputFileName.ToLower() == "temp")
						{
							_outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
						} else
						{
							_outputFileName = FileFunctions.AdjustFileName(_outputFileName);
						}

						if (!string.IsNullOrWhiteSpace(outputFileName))
						{
							throw new Exception("outputFile defined multiple times");
						}
						outputFileName = _outputFileName;
						symbols.Add(new Tuple<string, string>(name, intermediateFileName));
					} break;

				case InputType:
					{
						if (optional)
						{
							throw new Exception("inputFile cannot be optional");
						}

						string fileName = adapterArguments.String(name, !optional);
						fileName = FileFunctions.AdjustFileName(fileName);
						if (!File.Exists(fileName))
						{
							throw new ArgumentException(fileName + " not found");
						}

						string usedFileName = TableFunctions.ConvertInput(fileName);
						symbols.Add(new Tuple<string, string>(name, usedFileName));
						inputFileNames.Add(name, fileName);
					} break;

				case VariableNamesType:
					{
						string stringValue;
						if (adapterArguments.Exists(name))
						{
							List<string> variableNames = adapterArguments.StringList(name, optional, !optional);
							stringValue = StataScriptFunctions.VariableList(variableNames);
						} else
						{
							stringValue = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(stringValue))
						{
							if (!optional)
							{
								throw new Exception("Variables missing");
							}
							stringValue = defaultValue;
						}

						if (!string.IsNullOrWhiteSpace(stringValue))
						{
							symbols.Add(new Tuple<string, string>(name, stringValue));
						}
					} break;

				case BoolType:
					{
						string stringValue;

						if (adapterArguments.Exists(name))
						{
							stringValue = adapterArguments.String(name, !optional);
						} else
						{
							stringValue = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(stringValue))
						{
							if (!optional)
							{
								throw new Exception("Variables missing");
							}
							stringValue = defaultValue;
						}

						bool value;
						if (!string.IsNullOrWhiteSpace(stringValue))
						{
							value = bool.Parse(stringValue.ToLower());
							symbols.Add(new Tuple<string, string>(name, stringValue));
						} else
						{
							value = false;
						}

						symbols.Add(new Tuple<string, string>(name, value.ToString()));
					} break;

				default:
					throw new Exception("Unknown argument type \"" + type + "\" ");
				}
			}
		}

		private static string FindScriptTemplate(string operationName)
		{
			string fileName = Path.Combine(@"C:\Swish\StataScripts", operationName + SwishFunctions.DoFileExtension);
			if (!File.Exists(fileName))
			{
				return string.Empty;
			}
			fileName = Path.GetFullPath(fileName);
			return fileName;
		}

		private static void RunStataOperation(IStataBasedOperation stataOperation, AdapterArguments splitArguments)
		{
			SortedList<string, string> inputFileNames = new SortedList<string, string>();
			for (int inputIndex = 0; inputIndex < stataOperation.InputNames.Count; inputIndex++)
			{
				string inputName = stataOperation.InputNames[inputIndex];

				string inputFileName = splitArguments.String(inputName, true);
				splitArguments.Remove(inputName);
				inputFileName = FileFunctions.AdjustFileName(inputFileName);
				if (!FileFunctions.FileExists(inputFileName))
				{
					throw new Exception("cannot find " + inputName + ": \"" + inputFileName + "\"");
				}
				inputFileNames.Add(inputName, inputFileName);
			}

			string outputFileName;
			string intermaidateOutput;
			if (stataOperation.ProducesOutputFile)
			{
				string outputArgumentName = splitArguments.ArgumentPrefix + "output" + "";
				outputFileName = splitArguments.String(outputArgumentName, false);
				splitArguments.Remove(outputArgumentName);
				outputFileName = FileFunctions.AdjustFileName(outputFileName);

				if (string.IsNullOrWhiteSpace(outputFileName) || outputFileName.ToLower() == "none" || outputFileName.ToLower() == "temp")
				{
					outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
				}

				string outputExtension = Path.GetExtension(outputFileName);
				intermaidateOutput = FileFunctions.TempoaryOutputFileName(outputExtension);
			} else
			{
				outputFileName = string.Empty;
				intermaidateOutput = string.Empty;
			}

			List<string> scriptOutput = stataOperation.GenerateScript(splitArguments, inputFileNames, intermaidateOutput);

			List<string> lines = new List<string>();

			StataScriptFunctions.WriteHeadder(lines);
			lines.AddRange(scriptOutput);
			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);

			if (stataOperation.ProducesOutputFile)
			{
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

			Console.WriteLine(outputFileName);

			if (ExceptionFunctions.ForceVerbose)
			{
				ExportMetadata(stataOperation.Name, splitArguments, inputFileNames, outputFileName, lines);
			}
		}

		private static void ExportMetadata(string operationName, AdapterArguments splitArguments, SortedList<string, string> inputFileNames, string outputFileName, List<string> lines)
		{
			List<MetadataRecord> metadata = new List<MetadataRecord>();

			MetadataRecord record = new MetadataRecord();
			for (int inputIndex = 0; inputIndex < inputFileNames.Count; inputIndex++)
			{
				string inputName = inputFileNames.Keys[inputIndex];
				string inputFileName = inputFileNames[inputName];

				if (MetadataFunctions.Exists(inputFileName))
				{
					List<MetadataRecord> inputMetadata = MetadataFunctions.Load(inputFileName);
					metadata.AddRange(inputMetadata);
				}

				record.Arguments.Add(new Tuple<string, string>(inputName, inputFileName));
			}

			record.Arguments.Add(new Tuple<string, string>("OutputFileName", outputFileName));
			if (AdapterFunctions.RecordDoScript)
			{
				record.Arguments.Add(new Tuple<string, string>("DoScript", string.Join(Environment.NewLine, lines)));
			}
			record.ComputerName = Environment.MachineName;
			record.Operation = operationName;

			record.Time = DateTime.Now;
			record.User = Environment.UserName;

			for (int argumentIndex = 0; argumentIndex < splitArguments.SplitArguments.Count; argumentIndex++)
			{
				string argumentName = splitArguments.SplitArguments[argumentIndex].Item1;
				string argumentValue = splitArguments.SplitArguments[argumentIndex].Item2;
				record.Arguments.Add(new Tuple<string, string>(argumentName, argumentValue));
			}

			metadata.Add(record);
			MetadataFunctions.Save(outputFileName, metadata);

		}

		private static IStataBasedOperation FindStataOperation(string operationName)
		{
			string lowercaseOperation = operationName.ToLower();
			List<IStataBasedOperation> operations = TypeFunctions.Interfaces<IStataBasedOperation>();
			for (int adapterIndex = 0; adapterIndex < operations.Count; adapterIndex++)
			{
				IStataBasedOperation adapter = operations[adapterIndex];
				if (adapter.Name.ToLower() == lowercaseOperation)
				{
					return adapter;
				}
			}

			return null;
		}

		private static IAdapter FindAdapter(string operation)
		{
			string lowercaseOperation = operation.ToLower();
			List<IAdapter> adapters = TypeFunctions.Interfaces<IAdapter>();
			for (int adapterIndex = 0; adapterIndex < adapters.Count; adapterIndex++)
			{
				IAdapter adapter = adapters[adapterIndex];

				if (adapter.Name.ToLower() == lowercaseOperation)
				{
					return adapter;
				}
			}

			return null;
		}



	}
}

