using System;
using System.IO;
using System.Collections.Generic;

namespace Swish.Adapters
{
	public class AdapterArguments
	{
		private Arguments _arguments;
		public AdapterArguments(Arguments splitArguments)
		{
			if (splitArguments == null)
			{
				throw new ArgumentNullException("splitArguments");
			}
			_arguments = splitArguments;
		}

		internal string String(string name, bool throwOnMissing)
		{
			return _arguments.String(name, throwOnMissing);
		}

		public string OutputFileName()
		{
			string outputFileName = String(_arguments.ArgumentPrefix + "output" + "", false);
			outputFileName = FileFunctions.AdjustFileName(outputFileName);
			if (string.IsNullOrWhiteSpace(outputFileName) || outputFileName.ToLower() == "none" || outputFileName.ToLower() == "temp")
			{
				outputFileName = FileFunctions.TempoaryOutputFileName(".dta");
			}
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			return outputFileName;
		}

		internal List<string> StringList(string name, bool throwOnMissing, bool throwOnEmpty)
		{
			return _arguments.StringList(name, throwOnMissing, throwOnEmpty);
		}
	}
}
