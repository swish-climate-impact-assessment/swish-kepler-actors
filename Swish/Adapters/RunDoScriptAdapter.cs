using System;

namespace Swish.Adapters
{
	public class RunDoScriptAdapter: IAdapter
	{
		public string Name { get { return "doScript"; } }
		public string Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.DefaultArgumentPrefix + "filename", true));
			string log = StataFunctions.RunScript(inputFileName, false);
			return log;
		}
	}
}
