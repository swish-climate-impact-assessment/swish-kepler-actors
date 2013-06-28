using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class MergeAndFillZeroScriptGenerator: IScriptGenerator
	{
		public const string NameString = "MergeAndFillZero";
		public string Name { get { return NameString; } }

		public const string FillVariableToken = "%FillVariable%";
		public void GenerateScript(List<string> lines)
		{
			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input1FileNameToken);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input2FileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + StataScriptFunctions.IntermediateFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + FillVariableToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input2FileNameToken);

			StataScriptFunctions.SortCommand(lines, StataScriptFunctions.VariableNamesToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.IntermediateFileNameToken);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input1FileNameToken);

			StataScriptFunctions.SortCommand(lines, StataScriptFunctions.VariableNamesToken);

			string line = "merge " + StataScriptFunctions.VariableNamesToken + ", using \"" + StataScriptFunctions.IntermediateFileNameToken + "\"";
			lines.Add(line);

			StataScriptFunctions.SortCommand(lines, StataScriptFunctions.VariableNamesToken);

			//lines.Add("recast long " + FillVariableToken + ", force");

			//lines.Add("decode " + FillVariableToken );

			string expression = FillVariableToken + "=0";
			StataScriptFunctions.Replace(lines, expression, "missing(" + FillVariableToken + ")");

			StataScriptFunctions.DropVariable(lines, StataScriptFunctions.MergeColumnName);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
