using System;
using System.Collections.Generic;
using System.IO;
using Swish.Adapters;

namespace Swish.ScriptGenerators
{
	public class MergeKeepScriptGenerator: IScriptGenerator
	{
		public const string NameString = "mergeKeep";
		public string Name { get { return NameString; } }

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

			lines.Add("// define " + StataScriptFunctions.InputType + " " + Input1FileNameString);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + Input2FileNameString);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + OutputFileNameString);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + VariableNamesString);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + IntermediateFileNameString);

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

			line = StataScriptFunctions.SortCommand(VariableNamesString);
			lines.Add(line);

			StataScriptFunctions.SaveFileCommand(lines, OutputFileNameString);

			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}

	}
}
