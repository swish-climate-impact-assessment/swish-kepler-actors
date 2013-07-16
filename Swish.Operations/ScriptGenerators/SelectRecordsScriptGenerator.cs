using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class SelectRecordsScriptGenerator: IScriptGenerator
	{
		public const string NameString = "SelectRecords";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.StringType + " " + StataScriptFunctions.ExpressionToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			lines.Add("keep if " + StataScriptFunctions.ExpressionToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}