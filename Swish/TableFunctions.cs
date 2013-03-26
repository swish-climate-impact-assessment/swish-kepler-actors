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
		public static void Merge(string inputFileNameA, string inputFileNameB, List<string> mergeVariables, string outputFileName, bool keepMergeColumn)
		{
			Arguments arguments = new Arguments();
			inputFileNameA = ConvertInput(inputFileNameA);
			inputFileNameB = ConvertInput(inputFileNameB);
			arguments.String(MergeScriptGenerator.Input1FileNameString, inputFileNameA);
			arguments.String(MergeScriptGenerator.Input2FileNameString, inputFileNameB);
			string intermediateFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension);
			arguments.String(MergeScriptGenerator.OutputFileNameString, intermediateFileName);
			string variables = StataScriptFunctions.VariableList(mergeVariables);
			arguments.String(MergeScriptGenerator.VariableNamesString, variables);
			arguments.String(MergeScriptGenerator.IntermediateFileNameString, FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension));

			if (!keepMergeColumn)
			{
				AdapterFunctions.RunOperation("merge", new AdapterArguments(arguments));
			} else
			{
				AdapterFunctions.RunOperation("mergeKeep", new AdapterArguments(arguments));
			}

			ConvertOutput(outputFileName, intermediateFileName);
		}

		private static void ConvertOutput(string destination, string source)
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
	}
}
