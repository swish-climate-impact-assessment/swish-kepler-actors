using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.ScriptGenerators
{
	public class ConvertVariableToDateScriptGenerator: IScriptGenerator
	{
		public const string NameString = "ConvertVariableToDate";
		public string Name { get { return NameString; } }


		public void GenerateScript(List<string> lines)
		{
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.InputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNameType + " " + StataScriptFunctions.VariableNameToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.InputFileNameToken);
			string workingDateVariableName = StataScriptFunctions.TemporaryVariableName();
			string expression = "date(" + StataScriptFunctions.VariableNameToken + ", \"DMY\")";
			StataScriptFunctions.Generate(lines, StataDataType.Unknown, workingDateVariableName, expression);
			StataScriptFunctions.Format(lines, workingDateVariableName, "%td");
			StataScriptFunctions.DropVariable(lines, StataScriptFunctions.VariableNameToken);
			StataScriptFunctions.RenameVariable(lines, workingDateVariableName, StataScriptFunctions.VariableNameToken);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}
	}
}

