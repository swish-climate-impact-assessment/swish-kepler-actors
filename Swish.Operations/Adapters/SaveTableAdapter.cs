using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class SaveTableAdapter: IOperation
	{
		public string Name { get { return "save"; } }

		public string Run(OperationArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string outputFileName = splitArguments.OutputFileName(SwishFunctions.DataFileExtension);
			SwishFunctions.Save(inputFileName, outputFileName);
			return outputFileName;
		}

	}
}
