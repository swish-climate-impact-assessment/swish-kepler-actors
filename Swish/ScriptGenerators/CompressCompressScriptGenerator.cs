using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class CompressScriptGenerator: IScriptGenerator
	{
		public const string NameString = "compress";
		public string Name { get { return NameString; } }

		public List<string> GenerateScript()
		{
			List<string> lines = new List<string>();

			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameString);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameString);

			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameString);

			lines.Add("compress");

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameString);

			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}
	}
}

