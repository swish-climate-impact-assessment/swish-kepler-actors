using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class MergeScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Merge";
		public string Name { get { return NameString; } }

		public const string VariableNamesToken = "%Variables%";

		public void GenerateScript(List<string> lines)
		{
			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input1FileNameToken);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input2FileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + VariableNamesToken);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + StataScriptFunctions.IntermediateFileNameToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input2FileNameToken);

			StataScriptFunctions.SortCommand(lines, VariableNamesToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.IntermediateFileNameToken);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input1FileNameToken);

			StataScriptFunctions.SortCommand(lines, VariableNamesToken);

			string line = "merge " + VariableNamesToken + ", using \"" + StataScriptFunctions.IntermediateFileNameToken + "\"";
			lines.Add(line);

			lines.Add("drop " + StataScriptFunctions.MergeColumnName);

			StataScriptFunctions.SortCommand(lines, VariableNamesToken);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
