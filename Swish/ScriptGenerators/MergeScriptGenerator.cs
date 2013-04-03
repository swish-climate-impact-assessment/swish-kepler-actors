using System;
using System.Collections.Generic;
using System.IO;
using Swish.Adapters;

namespace Swish.ScriptGenerators
{
	public class MergeScriptGenerator: IScriptGenerator
	{
		public const string NameString = "Merge";
		public string Name { get { return NameString; } }

		public void GenerateScript(List<string> lines)
		{
			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input1FileNameToken);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input2FileNameToken);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameToken);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + StataScriptFunctions.IntermediateFileNameToken);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input2FileNameToken);

			string line = StataScriptFunctions.SortCommand(StataScriptFunctions.VariableNamesToken);
			lines.Add(line);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.IntermediateFileNameToken);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input1FileNameToken);

			line = StataScriptFunctions.SortCommand(StataScriptFunctions.VariableNamesToken);
			lines.Add(line);


			line = "merge " + StataScriptFunctions.VariableNamesToken + ", using \"" + StataScriptFunctions.IntermediateFileNameToken + "\"";
			lines.Add(line);

			lines.Add("drop " + StataScriptFunctions.MergeColumnName);

			line = StataScriptFunctions.SortCommand(StataScriptFunctions.VariableNamesToken);
			lines.Add(line);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameToken);
		}

	}
}
