using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class FormatScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Format";
		public string Name { get { return NameString; } }

		public const string FormatToken = "%Format%";
		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);
			lines.Add("// define " + StataScriptFunctions.StringType + " " + FormatToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			lines.Add("format " + StataScriptFunctions.VariableNamesToken + " " + FormatToken);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}


	}
}
