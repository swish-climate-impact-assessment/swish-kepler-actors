using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class ReshapeLongScriptGenerator: IScriptGenerator
	{
		public const string NameString = "ReshapeLong";
		public string Name { get { return NameString; } }

		public const string VariableNamePrefixToken = "%VariableNamePrefix%";
		public const string IToken = "%I%";
		public const string JToken = "%J%";

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.TokenType + " " + VariableNamePrefixToken);
			lines.Add("// define " + StataScriptFunctions.TokenType + " " + IToken);
			lines.Add("// define " + StataScriptFunctions.TokenType + " " + JToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			lines.Add("reshape long " + VariableNamePrefixToken + " , i(" + IToken + ") j(" + JToken + ")");

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}
