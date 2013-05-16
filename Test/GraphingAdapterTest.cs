using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Tests
{
	public class GraphingAdapterTest
	{
		internal void ManualGraph()
		{
			string inputFileName = StataFunctionsTests.GenerateRandomDateIndexIntegerDoubleData();
			List<string> variables = new List<string>();
			variables.Add("integerValue");
			variables.Add("doubleValue");

			TableFunctions.DisplayGraph(inputFileName, variables);
		}
	}
}
