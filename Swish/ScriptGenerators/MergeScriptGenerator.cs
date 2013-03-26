﻿using System;
using System.Collections.Generic;
using System.IO;
using Swish.Adapters;

namespace Swish.ScriptGenerators
{
	public class MergeScriptGenerator: IScriptGenerator
	{
		public const string NameString = "merge";
		public string Name { get { return NameString; } }

		public List<string> GenerateScript()
		{
			/// create the do file
			//  merge [varlist] using filename [filename ...] [, options]
			List<string> lines = new List<string>();

			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input1FileNameString);
			lines.Add("// define " + StataScriptFunctions.InputType + " " + StataScriptFunctions.Input2FileNameString);
			lines.Add("// define " + StataScriptFunctions.OutputType + " " + StataScriptFunctions.OutputFileNameString);
			lines.Add("// define " + StataScriptFunctions.VariableNamesType + " " + StataScriptFunctions.VariableNamesToken);
			lines.Add("// define " + StataScriptFunctions.TemporaryFileType + " " + StataScriptFunctions.IntermediateFileNameString);

			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input2FileNameString);

			string line = StataScriptFunctions.SortCommand(StataScriptFunctions.VariableNamesToken);
			lines.Add(line);
			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.IntermediateFileNameString);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, StataScriptFunctions.Input1FileNameString);

			line = StataScriptFunctions.SortCommand(StataScriptFunctions.VariableNamesToken);
			lines.Add(line);


			line = "merge " + StataScriptFunctions.VariableNamesToken + ", using \"" + StataScriptFunctions.IntermediateFileNameString + "\"";
			lines.Add(line);

			lines.Add("drop " + StataScriptFunctions.MergeColumnName);

			line = StataScriptFunctions.SortCommand(StataScriptFunctions.VariableNamesToken);
			lines.Add(line);

			StataScriptFunctions.SaveFileCommand(lines, StataScriptFunctions.OutputFileNameString);

			StataScriptFunctions.WriteFooter(lines);

			return lines;
		}

	}
}
