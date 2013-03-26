using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class ReplaceScriptGenerator: IScriptGenerator
	{
		public const string NameString = "replace";
		public string Name { get { return NameString; } }

		public const string ConditionToken = "%Condition%";
		public const string ValueToken = "%Value%";

		public List<string> GenerateScript()
		{
			List<string> lines = new List<string>();

			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameString);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameString);

			lines.Add("// define " + StataScriptFunctions.StringType + " " + ConditionToken);
			lines.Add("// define " + StataScriptFunctions.StringType + " " + ValueToken);

			StataScriptFunctions.WriteHeadder(lines);
			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameString);
			//replace oldvar =exp [if] [in] [, nopromote]

			lines.Add("replace " + ValueToken + " if " + ConditionToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameString);

			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}
	}
}