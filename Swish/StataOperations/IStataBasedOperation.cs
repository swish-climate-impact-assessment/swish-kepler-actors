using System;
using System.Collections.Generic;

namespace Swish.StataOperations
{
	public interface IStataBasedOperation
	{
		string Name { get; }

		List<string> InputNames { get; }
		bool ProducesOutputFile { get; }

		SataScriptOutput GenerateScript(Arguments adapterArguments, SortedList<string, string> inputFileNames, string outputFileName);

	}
}
