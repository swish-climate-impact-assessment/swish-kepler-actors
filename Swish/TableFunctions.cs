using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System;
using Swish.Adapters;
using Swish.ScriptGenerators;

namespace Swish
{
	public static class TableFunctions
	{
		public static void ConvertOutput(string destination, string source)
		{
			string sourceExtension = Path.GetExtension(source).ToLower();
			string destinationExtension = Path.GetExtension(destination).ToLower();

			if (File.Exists(destination))
			{
				File.Delete(destination);
			}

			if (sourceExtension == destinationExtension)
			{
				File.Move(source, destination);
				return;
			}

			SaveTableAdapter.Save(source, destination);
		}

		public static string ConvertInput(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLower();
			if (extension == SwishFunctions.DataFileExtension)
			{
				return fileName;
			}
			string usedFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			SaveTableAdapter.Save(fileName, usedFileName);
			return usedFileName;
		}

		public static void Merge(string inputFileNameA, string inputFileNameB, List<string> mergeVariables, string outputFileName, bool keepMergeColumn)
		{
			Arguments arguments = new Arguments();
			inputFileNameA = ConvertInput(inputFileNameA);
			inputFileNameB = ConvertInput(inputFileNameB);
			arguments.String(StataScriptFunctions.Input1FileNameString, inputFileNameA);
			arguments.String(StataScriptFunctions.Input2FileNameString, inputFileNameB);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameString, intermediateFileName);
			string variables = StataScriptFunctions.VariableList(mergeVariables);
			arguments.String(StataScriptFunctions.VariableNamesToken, variables);
			arguments.String(StataScriptFunctions.IntermediateFileNameString, FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension));

			string operation;
			if (!keepMergeColumn)
			{
				operation = MergeScriptGenerator.NameString;
			} else
			{
				operation = MergeKeepScriptGenerator.NameString;
			}

			AdapterFunctions.RunOperation(operation, new AdapterArguments(arguments));

			ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void Append(string inputFileNameA, string inputFileNameB, string outputFileName)
		{
			Arguments arguments = new Arguments();
			inputFileNameA = ConvertInput(inputFileNameA);
			inputFileNameB = ConvertInput(inputFileNameB);
			arguments.String(StataScriptFunctions.Input1FileNameString, inputFileNameA);
			arguments.String(StataScriptFunctions.Input2FileNameString, inputFileNameB);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameString, intermediateFileName);

			AdapterFunctions.RunOperation(AppendScriptGenerator.NameString, new AdapterArguments(arguments));

			ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void Generate(string inputFileName, string outputFileName, string variableName, StataDataType type, string expression)
		{
			Arguments arguments = new Arguments();
			inputFileName = ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.InputFileNameString, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameString, intermediateFileName);
			arguments.String(StataScriptFunctions.VariableNamesToken, variableName);
			arguments.String(StataScriptFunctions.ExpressionToken, expression);

			if (type != StataDataType.Unknown)
			{
				arguments.String(GenerateScriptGenerator.TypeToken, type.ToString().ToLower());
			}

			AdapterFunctions.RunOperation(GenerateScriptGenerator.NameString, new AdapterArguments(arguments));

			ConvertOutput(outputFileName, intermediateFileName);
		}

		public static void Replace(string inputFileName, string outputFileName, string condition, string value)
		{
			Arguments arguments = new Arguments();
			inputFileName = ConvertInput(inputFileName);
			arguments.String(StataScriptFunctions.Input1FileNameString, inputFileName);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(StataScriptFunctions.OutputFileNameString, intermediateFileName);
			arguments.String(ReplaceScriptGenerator.ConditionToken, condition);
			arguments.String(ReplaceScriptGenerator.ValueToken, value);

			AdapterFunctions.RunOperation(ReplaceScriptGenerator.NameString, new AdapterArguments(arguments));

			ConvertOutput(outputFileName, intermediateFileName);
		}
	}
}
