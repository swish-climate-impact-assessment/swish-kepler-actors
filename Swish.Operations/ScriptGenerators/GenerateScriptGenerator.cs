using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class GenerateScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Generate";
		public string Name { get { return NameString; } }

		public const string TypeToken = "%Type%";
		public const string ResultVariableNameToken = "%Variable%";

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + ResultVariableNameToken + " " + OperationFunctions.WorkingVariableName);
			lines.Add("// define " + StataScriptFunctions.StringType + " " + StataScriptFunctions.ExpressionToken);
			lines.Add("// define " + StataScriptFunctions.TokenType + " " + "optional" + " " + TypeToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			StataScriptFunctions.Generate(lines, TypeToken, ResultVariableNameToken, StataScriptFunctions.ExpressionToken);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
