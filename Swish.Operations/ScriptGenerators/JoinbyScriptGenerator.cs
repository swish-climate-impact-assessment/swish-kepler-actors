using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class JoinbyScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Joinby";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input1FileNameToken);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input2FileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input1FileNameToken);
			lines.Add("joinby " + StataScriptFunctions.VariableNamesToken + " using \"" + StataScriptFunctions.Input2FileNameToken + "\"");
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);

		}
	}
}
