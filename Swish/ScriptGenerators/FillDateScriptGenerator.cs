using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class FillDateScriptGenerator: IScriptGenerator
	{
		public const string NameString = "FillDate";
		public string Name { get { return NameString; } }

		public const string VariableNameToken = "%Variable";

		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + VariableNameToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			string workingDateVariableName = StataScriptFunctions.TemporaryVariableName();
			string expression = "date(" + VariableNameToken + ", \"DMY\")";
			StataScriptFunctions.Generate(lines, StataDataType.Unknown, workingDateVariableName, expression);
			StataScriptFunctions.Format(lines, workingDateVariableName, "%td");
			lines.Add("tsset " + workingDateVariableName);
			lines.Add("tsfill");
			StataScriptFunctions.DropVariable(lines, VariableNameToken);
			StataScriptFunctions.RenameVariable(lines, workingDateVariableName, VariableNameToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}

