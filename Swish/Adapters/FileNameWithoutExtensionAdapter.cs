using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FileNameWithoutExtensionAdapter
	{
		public string Name { get { return "fileNameWithoutExtension"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = FileFunctions.AdjustFileName(splitArguments.String(Arguments.InputArgument, true));
			string fileName = Path.GetFileNameWithoutExtension(inputFileName);
			Console.Write(fileName);
		}
	}
}