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
				Arguments splitArguments = new Arguments(arguments);
				string operation = splitArguments.String(Arguments.OperationArgument, true);

				AdapterFunctions.RunOperation(operation, splitArguments);

				return 0;
			} catch (Exception error)
			{
				string message = Arguments.ErrorArgument + " " + ExceptionFunctions.Write(error, true);
				Console.Write(message);
				return -1;
			}
		}

	}
}
