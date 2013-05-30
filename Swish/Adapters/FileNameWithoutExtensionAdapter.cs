using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FileNameWithoutExtensionAdapter: IAdapter
	{
		public string Name { get { return "fileNameWithoutExtension"; } }

		public string Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string fileName = Path.GetFileNameWithoutExtension(inputFileName);
			return fileName;
		}
	}
}
