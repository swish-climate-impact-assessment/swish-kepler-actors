using System;
using System.Collections.Generic;
using Swish.Adapters;

namespace Swish.StataOperations
{
	public interface IStataBasedOperation
	{
		string Name { get; }

		List<string> InputNames { get; }
		bool ProducesOutputFile { get; }

		List<string> GenerateScript(AdapterArguments adapterArguments, SortedList<string, string> inputFileNames, string outputFileName);

	}
}
