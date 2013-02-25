using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class DateRangeAdapter: IAdapter
	{
		public string Name { get { return "DateRange"; } }

		public void Run(AdapterArguments splitArguments)
		{
			//string outputFileName, string variableName, DateTime start, DateTime end;

			string outputFileName = splitArguments.OutputFileName(SwishFunctions.CsvFileExtension);
			string variableName = splitArguments.String(Arguments.DefaultArgumentPrefix + "variableName", true);

			DateTime startDate = splitArguments.Date(Arguments.DefaultArgumentPrefix + "startDate", true);
			DateTime endDate = splitArguments.Date(Arguments.DefaultArgumentPrefix + "endDate", true);

			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(intermaidateOutput))
			{
				File.Delete(intermaidateOutput);
			}

			GenerateDaily(intermaidateOutput, variableName, startDate, endDate);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			File.Move(intermaidateOutput, outputFileName);

			Console.Write(outputFileName);
		}

		public static void GenerateDaily(string outputFileName, string variableName, DateTime startDate, DateTime endDate)
		{
			if (string.IsNullOrWhiteSpace(outputFileName))
			{
				throw new ArgumentNullException("outputFileName");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentNullException("variableName");
			}

			Csv table = new Csv();
			table.Header.Add(variableName);

			DateTime date = startDate.Date;
			DateTime end = endDate.Date;
			while (date <= end)
			{
				List<string> record = new List<string>();
				record.Add(date.ToShortDateString());
				table.Records.Add(record);
				date = date.AddDays(1);
			}

			CsvFunctions.Write(outputFileName, table);
		}

	}
}