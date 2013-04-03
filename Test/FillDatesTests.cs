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

			DateTime startDate = new DateTime(2000, 1, 1);
			DateTime endDate = new DateTime(2000, 12, 31);

			string dateVariableName = "Date";

			string inputFileName = GenerateInputData(startDate, endDate);
			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);

			FillDateAdapter.Fill(inputFileName, outputFileName, dateVariableName, startDate, endDate);

			Csv output = CsvFunctions.Read(outputFileName);

			if (output.Header.Count != 2)
			{
				throw new Exception();
			}
			int daysInYear = (int)((endDate - startDate).TotalDays) + 1;
			if (output.Records.Count != daysInYear)
			{
				throw new Exception();
			}
		}

		private static string GenerateInputData(DateTime startDate, DateTime endDate)
		{
			string inputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			Csv table = new Csv();
			table.Header.Add("Date");
			table.Header.Add("Other");

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
			return inputFileName;
		}
	}
}
