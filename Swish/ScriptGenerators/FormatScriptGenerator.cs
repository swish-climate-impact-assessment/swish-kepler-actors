using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class FormatScriptGenerator: IScriptGenerator
	{
		public const string NameString = "format";
		public string Name { get { return NameString; } }

		public const string FormatToken = "%Format%";
		public List<string> GenerateScript()
		{
			List<string> lines = new List<string>();

			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameString);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameString);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + StataScriptFunctions.IntermediateFileNameString);
			lines.Add("// define " + StataScriptFunctions.StringType + " " + FormatToken);

			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameString);

			lines.Add("format " + StataScriptFunctions.VariableNamesToken + " " + FormatToken);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameString);

			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}


	}
}
