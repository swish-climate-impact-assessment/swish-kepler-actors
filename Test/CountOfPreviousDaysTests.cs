using System;
using System.Collections.Generic;
using Swish.Adapters;
using System.IO;

namespace Swish.Tests
{
	public class CountOfPreviousDaysTests
	{
		internal void Count()
		{
			string variableName = "Value";
			int recordCount = 100;
			int periodCount = 5;
			Csv table = new Csv();
			table.Header.Add("Date");
			table.Header.Add(variableName);
			DateTime date = new DateTime(2000, 1, 1);
			for (int recordIndex = 0; recordIndex < recordCount; recordIndex++)
			{
				List<string> record = new List<string>();
				record.Add(date.AddDays(recordIndex).ToShortDateString());
				record.Add((recordIndex % 2).ToString());
				table.Records.Add(record);
			}

			string inputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			CsvFunctions.Write(inputFileName, table);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			CountOfPreviousDaysAdapter.Count(inputFileName, outputFileName, variableName, periodCount);

			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("");
			}

			table = CsvFunctions.Read(outputFileName);
			if (table.Header.Count != 3 || table.Records.Count != recordCount)
			{
				throw new Exception("");
			}

			int columnIndex = table.ColumnIndex(AdapterFunctions.WorkingVariableName, true);
			List<int> counts = table.ColunmAsInts(columnIndex);
			for (int recordIndex = 0; recordIndex < recordCount; recordIndex++)
			{
				int value = counts[recordIndex];
				if (recordIndex < periodCount)
				{
					continue;
				}
				if ((recordIndex % 2) == 0)
				{
					if (value != 2)
					{
						throw new Exception("");
					}
				} else
				{
					if (value != 3)
					{
						throw new Exception("");
					}
				}
			}
		}
	}
}
