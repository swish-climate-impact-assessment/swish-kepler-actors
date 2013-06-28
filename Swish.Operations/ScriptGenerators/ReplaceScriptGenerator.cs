using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class ReplaceScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Replace";
		public string Name { get { return NameString; } }

		public const string ConditionToken = "%Condition%";
		public const string ValueToken = "%Value%";

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.StringType + " " + ConditionToken);
			lines.Add("// define " + StataScriptFunctions.StringType + " " + ValueToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			StataScriptFunctions.Replace(lines, ValueToken, ConditionToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}
