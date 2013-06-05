using System;
using System.Windows.Forms;

namespace Swish.Adapters
{
	public class ViewGridAdapter: IAdapter
	{
		public string Name { get { return "ViewGrid"; } }

		public string Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			Display(inputFileName);
			return inputFileName;
		}

		public static void Display(string inputFileName)
		{
			string arguments = string.Join(" ", 
				Arguments.DefaultArgumentPrefix + Arguments.OperationArgument, ViewGridClientAdapter.OperationName, 
				Arguments.DefaultArgumentPrefix + Arguments.InputArgument, inputFileName
			);
			ProcessFunctions.Run(Application.ExecutablePath, arguments, Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, true);
		}

	}
}