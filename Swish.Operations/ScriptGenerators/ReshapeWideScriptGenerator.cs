using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class ReshapeWideScriptGenerator: IScriptGenerator
	{
		public const string NameString = "ReshapeWide";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.TokenType + " " + ReshapeLongScriptGenerator.VariableNamePrefixToken);
			lines.Add("// define " + StataScriptFunctions.TokenType + " " + ReshapeLongScriptGenerator.IToken);
			lines.Add("// define " + StataScriptFunctions.TokenType + " " + ReshapeLongScriptGenerator.JToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			lines.Add("reshape wide " + ReshapeLongScriptGenerator.VariableNamePrefixToken + " , i(" + ReshapeLongScriptGenerator.IToken + ") j(" + ReshapeLongScriptGenerator.JToken + ")");

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}
