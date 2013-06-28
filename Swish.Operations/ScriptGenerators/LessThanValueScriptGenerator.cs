using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class LessThanValueScriptGenerator: IScriptGenerator
	{
		public const string NameString = "LessThanValue";
		public string Name { get { return NameString; } }

		public const string VariableNameToken = "%Variable%";
		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + VariableNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + StataScriptFunctions.ResultVariableNameToken + " " + OperationFunctions.WorkingVariableName);
			lines.Add("// define " + StataScriptFunctions.DoubleType + " " + StataScriptFunctions.ValueToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			string expression = VariableNameToken + " < " + StataScriptFunctions.ValueToken;
			StataScriptFunctions.Generate(lines, StataDataType.Double, StataScriptFunctions.ResultVariableNameToken, expression);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
