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

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input1FileNameToken);
			lines.Add("append using \"" + StataScriptFunctions.Input2FileNameToken + "\"");
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}


	}
}
