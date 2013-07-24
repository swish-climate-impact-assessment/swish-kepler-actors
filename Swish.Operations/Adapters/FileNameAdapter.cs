using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FileNameAdapter: IOperation
	{
		public string Name { get { return "fileName"; } }

		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string fileName = Path.GetFileName(inputFileName);
			return fileName;
		}
	}
}
