using System;
using System.IO;

namespace Swish.Adapters
{
	public class TemporaryFileNameAdapter: IAdapter
	{
		public string Name { get { return "temporaryFileName"; } }

		public string Run(AdapterArguments splitArguments)
		{
			string fileName = FileFunctions.TempoaryOutputFileName(string.Empty);
			if (FileFunctions.FileExists(fileName))
			{
				File.Delete(fileName);
			}
			string output = fileName;
			return output;
		}
	}
}
