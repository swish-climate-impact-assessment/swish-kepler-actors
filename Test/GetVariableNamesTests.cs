using System;
using System.Collections.Generic;

namespace Swish.Tests
{
	class GetVariableNamesTests
	{
		internal void Names()
		{
			Csv table = new Csv();
			table.Headers.Add("Date");
			table.Headers.Add("CategoryA");
			table.Headers.Add("CategoryB");
			table.Headers.Add("Value");

			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 1).ToShortDateString(), "q", "w", "1", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 2).ToShortDateString(), "q", "w", "2", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 3).ToShortDateString(), "q", "w", "3", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 4).ToShortDateString(), "q", "w", "4", }));

			string inputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			CsvFunctions.Write(inputFileName, table);

			SortedList<string, string > variableInformation = Swish.Adapters.VariableNamesAdapter.VariableInformation(inputFileName);
			List<string> variableNames = new List<string>(variableInformation.Keys);

			if (variableNames.Count != 4
				|| !variableNames.Contains("Date".ToLower())
				|| !variableNames.Contains("CategoryA".ToLower())
				|| !variableNames.Contains("CategoryB".ToLower())
				|| !variableNames.Contains("Value".ToLower()))
			{
				throw new Exception();

			}
		}
	}
}
