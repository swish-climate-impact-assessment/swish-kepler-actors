using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class StataCommandScriptGenerator: IScriptGenerator
	{
		public const string NameString = "StataCommand";
		public string Name { get { return NameString; } }

		public const string CommandToken = "%Command%";
		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.StringType + " " + CommandToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			lines.Add(CommandToken);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}
