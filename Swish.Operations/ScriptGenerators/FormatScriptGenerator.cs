using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class FormatScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Format";
		public string Name { get { return NameString; } }

		public const string FormatToken = "%Format%";
		public const string VariableNamesString = "%Variables%";

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + VariableNamesString);
			lines.Add("// define " + StataScriptFunctions.StringType + " " + FormatToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			StataScriptFunctions.Format(lines, VariableNamesString, FormatToken);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}



	}
}
