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
			Display(inputFileName);
		}

		public static void Display(string inputFileName)
		{
			string arguments = string.Join(" ", Arguments.DefaultArgumentPrefix +  Arguments.OperationArgument,
				DisplayTableClientAdapter.OperationName, Arguments.DefaultArgumentPrefix + Arguments.InputArgument, inputFileName);
			ProcessFunctions.Run(Application.ExecutablePath, arguments, Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, true);
		}

	}
}
