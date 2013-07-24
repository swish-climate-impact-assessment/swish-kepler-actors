using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class MergeKeepScriptGenerator: IScriptGenerator
	{
		public const string NameString = "MergeKeep";
		public string Name { get { return NameString; } }

		public const string Input1FileNameString = "%Input1%";
		public const string Input2FileNameString = "%Input2%";
		public const string OutputFileNameString = "%Output%";
		public const string VariableNamesString = "%Variables%";
		public const string IntermediateFileNameString = "%TemporaryFile%";

		public void GenerateScript(List<string> lines)
		{
			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]

			lines.Add("// define " + StataScriptFunctions.InputType + " " + Input1FileNameString);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + Input2FileNameString);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + OutputFileNameString);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + VariableNamesString);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + IntermediateFileNameString);

			StataScriptFunctions.LoadFileCommand(lines, Input2FileNameString);

			StataScriptFunctions.SortCommand(lines, VariableNamesString);
			StataScriptFunctions.SaveFileCommand(lines, IntermediateFileNameString);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, Input1FileNameString);

			StataScriptFunctions.SortCommand(lines, VariableNamesString);

			string line = "merge " + VariableNamesString + ", using \"" + IntermediateFileNameString + "\"";
			lines.Add(line);

			StataScriptFunctions.SortCommand(lines, VariableNamesString);

			StataScriptFunctions.SaveFileCommand(lines, OutputFileNameString);
		}

	}
}
