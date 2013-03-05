using System.Collections.Generic;
using System;
using Swish.Adapters;

namespace Swish.StataOperations
{
	public class LessThanOrEqualVariableOperation: IStataBasedOperation
	{
		public string Name { get { return "LessThanOrEqualVariable"; } }

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
			List<string> lines = new List<string>();

			string inputFileName = inputFileNames[InputName];
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			string leftVariableName = adapterArguments.String("leftVariable", true);
			string rightVariableName = adapterArguments.String("rightVariable", true);

			string resultVariableName = adapterArguments.String("resultVariable", false);
			if (string.IsNullOrWhiteSpace(resultVariableName))
			{
				resultVariableName = AdapterFunctions.WorkingVariableName;
			}

			string expression = leftVariableName + " <= " + rightVariableName;
			StataScriptFunctions.Generate(lines, StataDataType.Double, resultVariableName, expression);

			StataScriptFunctions.SaveFileCommand(lines, outputFileName);

			return lines;
		}

	}
}
