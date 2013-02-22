using System;
using System.IO;

namespace Swish.Adapters
{
	public class TemporaryFileNameAdapter
	{
		public string Name { get { return "temporaryFileName"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string fileName = FileFunctions.TempoaryOutputFileName(string.Empty);
			if (FileFunctions.FileExists(fileName))
			{
				File.Delete(fileName);
			}
			Console.Write(fileName);
		}
	}
}