using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FileNameAdapter: IAdapter
	{
		public string Name { get { return "fileName"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
			string fileName = Path.GetFileName(inputFileName);
			Console.Write(fileName);
		}
	}
}