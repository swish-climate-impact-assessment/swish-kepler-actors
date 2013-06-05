using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class DeleteVariablesScriptGenerator: IScriptGenerator
	{
		public const string NameString = "DeleteVariables";
		public string Name { get { return NameString; } }

		public const string VariableNamesToken = "%Variables%";
		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + VariableNamesToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			lines.Add("drop " + VariableNamesToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}