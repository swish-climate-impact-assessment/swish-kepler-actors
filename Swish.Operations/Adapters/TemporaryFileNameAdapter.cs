using System.IO;
using Swish.IO;

namespace Swish.Adapters
{
	public class TemporaryFileNameAdapter: IOperation
	{
		public string Name { get { return "temporaryFileName"; } }

		public string Run(OperationArguments splitArguments)
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
