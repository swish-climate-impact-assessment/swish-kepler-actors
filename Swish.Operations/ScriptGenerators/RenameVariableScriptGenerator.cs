using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class RenameVariableScriptGenerator: IScriptGenerator
	{
		public const string NameString = "RenameVariable";
		public string Name { get { return NameString; } }

		public const string NewVariableNameToken = "%NewVariableName%";

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + StataScriptFunctions.VariableNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + NewVariableNameToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			StataScriptFunctions.DropVariable(lines, NewVariableNameToken);
			StataScriptFunctions.RenameVariable(lines, StataScriptFunctions.VariableNameToken, NewVariableNameToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}
