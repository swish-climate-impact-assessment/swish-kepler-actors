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
			arguments.String(MergeScriptGenerator.Input1FileNameString, inputFileNameA);
			arguments.String(MergeScriptGenerator.Input2FileNameString, inputFileNameB);
			arguments.String(MergeScriptGenerator.OutputFileNameString, outputFileName);
			arguments.String(MergeScriptGenerator.VariableNamesString, StataScriptFunctions.VariableList(mergeVariables));
			arguments.String(MergeScriptGenerator.IntermediateFileNameString, FileFunctions.TempoaryOutputFileName(SwishFunctions.DataFileExtension));

			AdapterFunctions.RunOperation("merge", new AdapterArguments(arguments));
		}
	}
}
