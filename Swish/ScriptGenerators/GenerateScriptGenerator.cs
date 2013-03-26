using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class GenerateScriptGenerator: IScriptGenerator
	{
		public const string NameString = "generate";
		public string Name { get { return NameString; } }

		public const string TypeToken = "%Type%";

		public List<string> GenerateScript()
		{
			List<string> lines = new List<string>();

			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameString);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameString);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + "optional" + " " + StataScriptFunctions.VariableNameToken + " " + AdapterFunctions.WorkingVariableName);
			lines.Add("// define " + StataScriptFunctions.ExpressionType + " " + StataScriptFunctions.ExpressionToken);
			lines.Add("// define " + StataScriptFunctions.TokenType + " " + "optional" + " " + TypeToken);

			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameString);

			StataScriptFunctions.Generate(lines, TypeToken, StataScriptFunctions.VariableNameToken, StataScriptFunctions.ExpressionToken);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameString);

			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}
	}
}
