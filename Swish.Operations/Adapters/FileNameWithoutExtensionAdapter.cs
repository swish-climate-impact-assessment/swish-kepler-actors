using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FileNameWithoutExtensionAdapter: IOperation
	{
		public string Name { get { return "fileNameWithoutExtension"; } }

		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string fileName = Path.GetFileNameWithoutExtension(inputFileName);
			return fileName;
		}
	}
}
