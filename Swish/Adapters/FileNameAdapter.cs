using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FileNameAdapter: IAdapter
	{
		public string Name { get { return "fileName"; } }

		public string Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string fileName = Path.GetFileName(inputFileName);
			return fileName;
		}
	}
}
