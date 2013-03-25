using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FillDateAdapter: IAdapter
	{
		public string Name { get { return "FillDate"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string outputFileName = splitArguments.OutputFileName(SwishFunctions.DataFileExtension);
			string variableName = splitArguments.VariableName();
			DateTime startDate = splitArguments.Date("startDate", true);
			DateTime endDate = splitArguments.Date("endDate", true);

			Fill(inputFileName, outputFileName, variableName, startDate, endDate);
			Console.Write(outputFileName);
		}

		public static void Fill(string inputFileName, string outputFileName, string variableName, DateTime startDate, DateTime endDate)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("Variable missing");
			}

			string datesFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			DateRangeAdapter.GenerateDaily(datesFileName, variableName, startDate, endDate);

			string extension = Path.GetExtension(outputFileName);

			List<string> mergeVariables = new List<string>();
			mergeVariables.Add(variableName);

			TableFunctions.Merge(inputFileName, datesFileName, mergeVariables, outputFileName, false);
		}
	}
}