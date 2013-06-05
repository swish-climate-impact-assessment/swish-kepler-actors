using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Swish.Adapters;
using System.Windows.Forms;

namespace Swish
{
	public static class AdapterFunctions
	{
		public const string WorkingVariableName = "Working";
		internal static bool RecordDoScript = false;

		public static string RunOperation(string operationName, AdapterArguments splitArguments)
		{
			string scriptFile = FindScriptTemplate(operationName);
			if (!string.IsNullOrWhiteSpace(scriptFile))
			{
				string output = RunScriptTemplate(operationName, scriptFile, splitArguments);
				return output;
			}

			IAdapter adapter = FindAdapter(operationName);
			if (adapter != null)
			{
				string output = adapter.Run(splitArguments);
				return output;
			}

			throw new Exception("No operation found for \"" + operationName + "\"");
		}

		private static string FindScriptTemplate(string operationName)
		{
			string fileName = Path.Combine(@"C:\Swish\StataScripts", operationName + SwishFunctions.DoFileExtension);
			if (!FileFunctions.FileExists(fileName))
			{
				return string.Empty;
			}
			fileName = Path.GetFullPath(fileName);
			return fileName;
		}

		private static string RunScriptTemplate(string operationName, string scriptFile, AdapterArguments adapterArguments)
		{
			string[] _lines = File.ReadAllLines(scriptFile);
			List<string> lines = new List<string>(_lines);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);

			string outputFileName;
			SortedList<string, string> inputFileNames;

			List<string> newLines;
			List<Tuple<string, string>> symbols;
			StataScriptFunctions.ResloveSymbols(out outputFileName, out inputFileNames, out newLines, out symbols, lines, intermediateFileName, adapterArguments);
			lines = SwishFunctions.ConvertLines(newLines, symbols, null, true, false, false);

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

			return outputFileName;
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

