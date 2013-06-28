using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Swish.Adapters
{
	public class GraphSeriesAdapter: IOperation
	{
		public string Name { get { return "GraphSeries"; } }

		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			List<string> variableNames = splitArguments.VariableNames();
			Display(inputFileName, variableNames);
			return inputFileName;
		}

		public static void Display(string inputFileName, List<string> variableNames)
		{
			string arguments = string.Join(" ",
				Arguments.DefaultArgumentPrefix + Arguments.OperationArgument, GraphSeriesClientAdapter.OperationName,
				Arguments.DefaultArgumentPrefix + Arguments.InputArgument, inputFileName,
				Arguments.DefaultArgumentPrefix + "VariableNames", string.Join(" ", variableNames)
				);
			ProcessFunctions.Run(Application.ExecutablePath, arguments, Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, true);
		}

	}
}
