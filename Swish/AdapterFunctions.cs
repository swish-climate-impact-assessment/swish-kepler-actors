using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Swish.Adapters;
using Swish.StataOperations;

namespace Swish
{
	public static class AdapterFunctions
	{
		internal static bool RecordDoScript = false;

		private static List<InterfaceType> Adapters<InterfaceType>()
		{
			Type iAdapterType = typeof(InterfaceType);
			Assembly dll = iAdapterType.Assembly;
			Type[] exportedTypes = dll.GetExportedTypes();

			List<InterfaceType> adapters = new List<InterfaceType>();
			for (int typeIndex = 0; typeIndex < exportedTypes.Length; typeIndex++)
			{
				Type type = exportedTypes[typeIndex];

				if (TypeFunctions.TypeIsFormOf(type, iAdapterType) && type != iAdapterType && TypeFunctions.TypeHasDefaultConstructor(type))
				{
					InterfaceType adapter = (InterfaceType)Activator.CreateInstance(type);
					adapters.Add(adapter);
				}
			}
			return adapters;
		}

		public static void RunOperation(string operationName, Arguments splitArguments)
		{
			IAdapter adapter = FindAdapter(operationName);
			if (adapter != null)
			{
				AdapterArguments adapterArguments = new AdapterArguments(splitArguments);
				adapter.Run(adapterArguments);
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

		private static void RunStataOperation(IStataBasedOperation stataOperation, Arguments splitArguments)
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
					outputFileName = FileFunctions.TempoaryOutputFileName(".dta");
				}

				string outputExtension = Path.GetExtension(outputFileName);
				intermaidateOutput = FileFunctions.TempoaryOutputFileName(outputExtension);
			} else
			{
				outputFileName = string.Empty;
				intermaidateOutput = string.Empty;
			}

			SataScriptOutput scriptOutput = stataOperation.GenerateScript(splitArguments, inputFileNames, intermaidateOutput);

			List<string> lines = new List<string>();

			StataScriptFunctions.WriteHeadder(lines);
			lines.AddRange(scriptOutput.Lines);
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
			record.Operation = stataOperation.Name;

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
			List<IStataBasedOperation> operations = Adapters<IStataBasedOperation>();
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
			List<IAdapter> adapters = Adapters<IAdapter>();
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

