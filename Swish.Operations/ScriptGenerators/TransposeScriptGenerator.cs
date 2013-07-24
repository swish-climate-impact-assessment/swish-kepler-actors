using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class TransposeScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Transpose";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			lines.Add("xpose, clear");
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}
