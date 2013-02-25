using System;
using System.Collections.Generic;
using Swish.Adapters;
using System.IO;

namespace Swish.Tests
{
	class FillDatesTests
	{
		internal void Fill()
		{
			/// this is to take a data set and a date range then 
			/// merge the two togeather 
			/// so that the data set has the full date range
			///

			string dateVariableName = "Date";
			string otherVariableName = "Other";


			string inputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);

			DateTime startDate = new DateTime(2000, 1, 1);
			DateTime endDate = new DateTime(2000, 12, 31);

			Csv table = new Csv();
			table.Header.Add(dateVariableName);
			table.Header.Add(otherVariableName);

			DateTime date = startDate;
			Random random = new Random();
			while (date <= endDate)
			{
				double aValue = random.NextDouble();
				List<string> record = new List<string>();
				record.Add(date.ToShortDateString());
				record.Add(aValue.ToString());
				table.Records.Add(record);
				date = date.AddDays(1);
			}

			int daysInYear = (int)((endDate - startDate).TotalDays) + 1;
			while (table.Records.Count > daysInYear / 2)
			{
				int number = random.Next(table.Records.Count);
				table.Records.RemoveAt(number);
			}
			CsvFunctions.Write(inputFileName, table);

			string datesFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			DateRangeAdapter.GenerateDaily(datesFileName, dateVariableName, startDate, endDate);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			List<string> mergeVariables = new List<string>();
			mergeVariables.Add(dateVariableName);
			MergeAdapter.Merge(inputFileName, datesFileName, mergeVariables, outputFileName, false);

			if (!File.Exists(outputFileName))
			{
				throw new Exception();
			}

			Csv output = CsvFunctions.Read(outputFileName);

			if (output.Header.Count != 2)
			{
				throw new Exception();
			}
			if (output.Records.Count != daysInYear)
			{
				throw new Exception();
			}
		}
	}
}
