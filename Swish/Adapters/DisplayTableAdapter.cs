using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Swish.Adapters
{
	public class DisplayTableAdapter: IAdapter
	{
		public string Name { get { return "display"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			List<string> variableNames = splitArguments.VariableNames();
			Display(inputFileName, variableNames);
		}

		public static void Display(string inputFileName, List<string> variableNames)
		{
			string arguments = string.Join(" ",
				Arguments.DefaultArgumentPrefix + Arguments.OperationArgument,
				DisplayTableClientAdapter.OperationName,
				Arguments.InputArgument,
				inputFileName,
				Arguments.DefaultArgumentPrefix + "	VariableNames",
				string.Join(" ", variableNames)
			);
			ProcessFunctions.Run(Application.ExecutablePath, arguments, Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, true);
		}

	}
}
