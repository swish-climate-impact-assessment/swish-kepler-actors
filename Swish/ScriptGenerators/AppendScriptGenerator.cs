using System;
using System.Collections.Generic;
using System.IO;
using Swish.ScriptGenerators;

namespace Swish.ScriptGenerators
{
	public class AppendScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Append";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input1FileNameToken);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input2FileNameToken);

			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + StataScriptFunctions.IntermediateFileNameToken);

			string intermediateFileName = StataScriptFunctions.ConvertToStataFormat(lines, StataScriptFunctions.Input2FileNameToken);
			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input1FileNameToken);
			lines.Add("append using \"" + intermediateFileName + "\"");
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}


	}
}