using System;
using System.Collections.Generic;
using System.IO;
using Swish.ScriptGenerators;

namespace Swish.ScriptGenerators
{
	public class AppendScriptGenerator: IScriptGenerator
	{
		public const string NameString = "append";
		public string Name { get { return NameString; } }

		public List<string> GenerateScript()
		{
			List<string> lines = new List<string>();

			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input1FileNameString);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input2FileNameString);

			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameString);

			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + StataScriptFunctions.IntermediateFileNameString);

			StataScriptFunctions.WriteHeadder(lines);

			string intermediateFileName = StataScriptFunctions.ConvertToStataFormat(lines, StataScriptFunctions.Input2FileNameString);
			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input1FileNameString);
			lines.Add("append using \"" + intermediateFileName + "\"");
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameString);

			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}


	}
}