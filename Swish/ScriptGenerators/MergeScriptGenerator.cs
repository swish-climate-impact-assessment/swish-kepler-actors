using System;
using System.Collections.Generic;
using System.IO;
using Swish.Adapters;

namespace Swish.ScriptGenerators
{
	public class MergeScriptGenerator: IScriptGenerator
	{
		public string Name { get { return "merge"; } }

		public const string Input1FileNameString = "%Input1%";
		public const string Input2FileNameString = "%Input2%";
		public const string OutputFileNameString = "%Output%";
		public const string VariableNamesString = "%VariableNames%";
		public const string IntermediateFileNameString = "%TemporaryFile%";

		public List<string> GenerateScript()
		{
			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			List<string> lines = new List<string>();


			lines.Add("// define " + AdapterFunctions.InputType + " " + Input1FileNameString);
			lines.Add("// define " + AdapterFunctions.InputType + " " + Input2FileNameString);
			lines.Add("// define " + AdapterFunctions.OutputType + " " + OutputFileNameString);
			lines.Add("// define " + AdapterFunctions.VariableNamesType + " " + VariableNamesString);
			lines.Add("// define " + AdapterFunctions.TemporaryFileType + " " + IntermediateFileNameString);

			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, Input2FileNameString);

			string line = StataScriptFunctions.SortCommand(VariableNamesString);
			lines.Add(line);
			StataScriptFunctions.SaveFileCommand(lines, IntermediateFileNameString);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, Input1FileNameString);

			line = StataScriptFunctions.SortCommand(VariableNamesString);
			lines.Add(line);


			line = "merge " + VariableNamesString + ", using \"" + IntermediateFileNameString + "\"";
			lines.Add(line);

			//if (!keepMergeColumn)
			//{
			//    lines.Add("drop " + StataScriptFunctions.MergeColumnName);
			//}

			line = StataScriptFunctions.SortCommand(VariableNamesString);
			lines.Add(line);

			StataScriptFunctions.SaveFileCommand(lines, OutputFileNameString);

			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}

	}
}
