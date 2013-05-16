using System;
using System.Windows.Forms;

namespace Swish.Adapters
{
	public class GraphAdapter: IAdapter
	{
		public string Name { get { return "Graph"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			Display(inputFileName);
			Console.WriteLine(inputFileName);
		}

		public static void Display(string inputFileName)
		{
			string arguments = string.Join(" ", Arguments.DefaultArgumentPrefix + Arguments.OperationArgument,
				GraphClientAdapter.OperationName, Arguments.DefaultArgumentPrefix + Arguments.InputArgument, inputFileName);
			ProcessFunctions.Run(Application.ExecutablePath, arguments, Environment.CurrentDirectory, true, TimeSpan.Zero, false, false, true);
		}

	}
}
