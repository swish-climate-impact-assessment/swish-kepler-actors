using System;
using System.Collections.Generic;

namespace Swish.Tests
{
	class GetVariableNamesTests
	{
		internal void Names()
		{
			Csv table = new Csv();
			table.Header.Add("Date");
			table.Header.Add("CategoryA");
			table.Header.Add("CategoryB");
			table.Header.Add("Value");

			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 1).ToShortDateString(), "q", "w", "1", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 2).ToShortDateString(), "q", "w", "2", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 3).ToShortDateString(), "q", "w", "3", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 4).ToShortDateString(), "q", "w", "4", }));

			string inputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			CsvFunctions.Write(inputFileName, table);

			List<string> variables = Swish.Adapters.VariableNamesAdapter.VariableNames(inputFileName);

			if (variables.Count != 4 
				|| !variables.Contains("Date")
				|| !variables.Contains("CategoryA")
				|| !variables.Contains("CategoryB")
				|| !variables.Contains("Value"))
			{
				throw new Exception();

			}
		}
	}
}
