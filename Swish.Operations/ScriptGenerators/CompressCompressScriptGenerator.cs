using System.Collections.Generic;
using Swish.Stata;

namespace Swish.ScriptGenerators
{
	public class CompressScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Compress";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);

			lines.Add("compress");

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}

