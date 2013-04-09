using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class UniqueValuesScriptGenerator: IScriptGenerator
	{
		public const string NameString = "UniqueValues";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + StataScriptFunctions.VariableNameToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			lines.Add("keep " + StataScriptFunctions.VariableNameToken);
			string countVariable = StataScriptFunctions.TemporaryVariableName();
			lines.Add("by " + StataScriptFunctions.VariableNameToken + ", sort: gen " + countVariable + "=_n");
			lines.Add("keep if " + countVariable + "==1");
			StataScriptFunctions.DropVariable(lines, countVariable);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
