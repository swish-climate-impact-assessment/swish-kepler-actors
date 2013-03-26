using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class DeleteVariablesScriptGenerator: IScriptGenerator
	{
		public const string NameString = "deleteVariables";
		public string Name { get { return NameString; } }

		public List<string> GenerateScript()
		{
			List<string> lines = new List<string>();

			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameString);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameString);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);

			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameString);
			lines.Add("drop " + StataScriptFunctions.VariableNamesToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameString);
			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}

	}
}
