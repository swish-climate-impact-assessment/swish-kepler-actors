using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class SubtractVariablesScriptGenerator: IScriptGenerator
	{
		public const string NameString = "SubtractVariables";
		public string Name { get { return NameString; } }

		public const string LeftVariableNameToken = "%LeftVariable%";
		public const string RightVariableNameToken = "%RightVariable%";

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + LeftVariableNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + RightVariableNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + StataScriptFunctions.ResultVariableNameToken + " " + OperationFunctions.WorkingVariableName);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			string expression = LeftVariableNameToken + " - " + RightVariableNameToken;
			StataScriptFunctions.Generate(lines, StataDataType.Double, StataScriptFunctions.ResultVariableNameToken, expression);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
