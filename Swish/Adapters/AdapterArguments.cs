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

		public string OutputFileName(string extension)
		{
			string outputFileName = String(_arguments.ArgumentPrefix + "output" + "", false);
			outputFileName = FileFunctions.AdjustFileName(outputFileName);
			if (string.IsNullOrWhiteSpace(outputFileName) || outputFileName.ToLower() == "none" || outputFileName.ToLower() == "temp")
			{
				outputFileName = FileFunctions.TempoaryOutputFileName(extension);
			}
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			return outputFileName;
		}

		public void SetOutputFileName(string outputFileName)
		{
			_arguments.String(_arguments.ArgumentPrefix + "output", outputFileName);
		}

		internal List<string> StringList(string name, bool throwOnMissing, bool throwOnEmpty)
		{
			return _arguments.StringList(name, throwOnMissing, throwOnEmpty);
		}

		public string ArgumentString { get { return _arguments.ArgumentString; } }

		internal bool Exists(string name)
		{
			return _arguments.Exists(name);
		}

		public List<T> EnumList<T>(string name, bool throwOnMissing, bool throwOnEmpty)
		{
			return _arguments.EnumList<T>(name, throwOnMissing, throwOnEmpty);
		}

		internal bool Bool(string name, bool throwOnMissing)
		{
			return _arguments.Bool(name, throwOnMissing);
		}

		public T Enum<T>(string name, bool throwOnMissing)
		{
			return _arguments.Enum<T>(name, throwOnMissing);
		}

		public string ArgumentPrefix { get { return _arguments.ArgumentPrefix; } }

		internal DateTime Date(string name, bool throwOnMissing)
		{
			return _arguments.Date( name,  throwOnMissing);
		}
	}
}
