using Swish.IO;
using Swish.Stata;

namespace Swish.Adapters
{
	public class RunDoScriptAdapter: IOperation
	{
		public string Name { get { return "doScript"; } }
		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.DefaultArgumentPrefix + "filename", true));
			string log = StataFunctions.RunScript(inputFileName, false);
			return log;
		}
	}
}
