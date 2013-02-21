using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Swish.Adapters
{
	public class DisplayTableAdapter
	{
		public string Name { get { return "display"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
			Display(inputFileName);
		}

		public static void Display(string inputFileName)
		{
			string arguments = string.Join(" ", Arguments.OperationArgument, DisplayTableClientAdapter.OperationName, Arguments.InputArgument, inputFileName);
			ProcessFunctions.Run(Application.ExecutablePath, arguments, Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, true);
		}

	}
}
