using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Swish.Adapter
{
	static class Program
	{
		static int Main(string[] arguments)
		{
			try
			{
				List<Tuple<string, string>> splitArguments = ArgumentFunctions.SplitArguments(arguments);
				string operation = ArgumentFunctions.GetArgument(ArgumentFunctions.ArgumentCharacter + "operation", splitArguments, true);

				switch (operation)
				{
				case "temporaryFileName":
					TemporaryFileName(splitArguments);
					break;

				default:
					throw new Exception("Unknown operation");
				}
				return 0;
			} catch (Exception error)
			{
				string message = ArgumentFunctions.ErrorArgument + " " + ExceptionFunctions.WriteException(error, true);
				Console.Write(message);
				return -1;
			}
		}

		private static void TemporaryFileName(List<Tuple<string, string>> splitArguments)
		{
			splitArguments.Clear();
			string fileName = Path.GetTempFileName();
			if (SwishFunctions.FileExists(fileName))
			{
				File.Delete(fileName);
			}
			Console.Write(fileName);

		}

	}
}
