using System.Collections.Generic;
using System;

namespace Swish.StataOperations
{
	public class GreaterThanValueOperation: IStataBasedOperation
	{
		public string Name { get { return "GreaterThanValue"; } }

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

		public List<string> GenerateScript(Arguments adapterArguments, SortedList<string, string> inputFileNames, string outputFileName)
		{
			List<string> lines = new List<string>();

			string inputFileName = inputFileNames[InputName];
			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			// this assumes that result does not exist

			double value = adapterArguments.Double("value", true);
			string variableName = adapterArguments.String("variable", true);

			string resultVariableName = adapterArguments.String("resultVariable", false);
			if (string.IsNullOrWhiteSpace(resultVariableName))
			{
				resultVariableName = "result";
			}

			string expression = variableName + " > " + value;
			StataScriptFunctions.Generate(lines, StataDataType.Byte, resultVariableName, expression);

			StataScriptFunctions.SaveFileCommand(lines, outputFileName);

			return lines;
		}

	}
}
