using System.Collections.Generic;
using System;
using Swish.Adapters;

namespace Swish.ScriptGenerators
{
	public class NotVariableScriptGenerator: IScriptGenerator
	{
		public const string NameString = "NotVariable";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + StataScriptFunctions.VariableNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + StataScriptFunctions.ResultVariableNameToken + " " + AdapterFunctions.WorkingVariableName);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			string expression = "!" + StataScriptFunctions.VariableNameToken;
			StataScriptFunctions.Generate(lines, StataDataType.Double, StataScriptFunctions.ResultVariableNameToken, expression);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}