using System.Collections.Generic;
using Swish.Stata;

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
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + StataScriptFunctions.ResultVariableNameToken + " " + OperationFunctions.WorkingVariableName);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			string expression = StataScriptFunctions.LeftVariableNameToken + " > " + StataScriptFunctions.RightVariableNameToken;
			StataScriptFunctions.Generate(lines, StataDataType.Double, StataScriptFunctions.ResultVariableNameToken, expression);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
