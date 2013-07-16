using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class SetTimeSeriesScriptGenerator: IScriptGenerator
	{
		public const string NameString = "SetTimeSeries";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			lines.Add("tsset  " + StataScriptFunctions.VariableNamesToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
