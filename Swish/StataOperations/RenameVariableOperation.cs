using System.Collections.Generic;
using System;
using Swish.Adapters;

namespace Swish.StataOperations
{
	public class RenameVariableOperation: IStataBasedOperation
	{
		public string Name { get { return "RenameVariable"; } }

		public const string InputName = "input";
		public List<string> InputNames
		{
			get
			{
				List<string> inputs = new List<string>();
				inputs.Add(InputName);
				return inputs;
			}
		}

		public bool ProducesOutputFile { get { return true; } }

		public List<string> GenerateScript(AdapterArguments adapterArguments, SortedList<string, string> inputFileNames, string outputFileName)
		{
			string inputFileName = inputFileNames[InputName];
			string variableName = adapterArguments.VariableName();
			string newVariableName = adapterArguments.String("newVariableName", true);

			List<string> lines = GenerateScript(inputFileName, outputFileName, variableName, newVariableName);
			return lines;

		}

		public static List<string> GenerateScript(string inputFileName, string outputFileName, string variableName, string newVariableName)
		{
			List<string> lines = new List<string>();
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);
			lines.Add("rename " + variableName + " " + newVariableName + "");
			StataScriptFunctions.SaveFileCommand(lines, outputFileName);
			return lines;
		}

	}
}
