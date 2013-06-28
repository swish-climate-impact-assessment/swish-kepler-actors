using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class LegacyRemoveColumnsScriptGenerator: IScriptGenerator
	{
		public const string NameString = "RemoveColumns";
		public string Name { get { return NameString; } }

		public const string VariableNamesToken = "%Variables%";
		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + VariableNamesToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			lines.Add("drop " + VariableNamesToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}
