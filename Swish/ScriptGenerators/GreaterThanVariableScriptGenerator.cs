using System.Collections.Generic;
using System;
using Swish.Adapters;

namespace Swish.ScriptGenerators
{
	public class GreaterThanVariableScriptGenerator: IScriptGenerator
	{
		public const string NameString = "GreaterThanVariable";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + StataScriptFunctions.LeftVariableNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + StataScriptFunctions.RightVariableNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + StataScriptFunctions.ResultVariableNameToken + " " + AdapterFunctions.WorkingVariableName);
			lines.Add("// define " + StataScriptFunctions.DoubleType + " " + StataScriptFunctions.ValueToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			string expression = StataScriptFunctions.LeftVariableNameToken + " > " + StataScriptFunctions.RightVariableNameToken;
			StataScriptFunctions.Generate(lines, StataDataType.Double, StataScriptFunctions.ResultVariableNameToken, expression);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
