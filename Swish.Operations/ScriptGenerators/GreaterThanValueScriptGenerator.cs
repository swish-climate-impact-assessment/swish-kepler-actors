using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class GreaterThanValueScriptGenerator: IScriptGenerator
	{
		public const string NameString = "GreaterThanValue";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + StataScriptFunctions.VariableNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + StataScriptFunctions.ResultVariableNameToken + " " + OperationFunctions.WorkingVariableName);
			lines.Add("// define " + StataScriptFunctions.DoubleType + " " + StataScriptFunctions.ValueToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			string expression = StataScriptFunctions.VariableNameToken + " > " + StataScriptFunctions.ValueToken;
			StataScriptFunctions.Generate(lines, StataDataType.Unknown, StataScriptFunctions.ResultVariableNameToken, expression);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
