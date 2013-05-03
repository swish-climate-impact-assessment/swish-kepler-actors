using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class CountOfPreviousDaysAdapter: IAdapter
	{
		public string Name { get { return "CountOfPreviousDays"; } }

		public void Run(AdapterArguments splitArguments)
		{
			string inputFileName = splitArguments.InputFileName();
			string variableName = splitArguments.VariableName();
			int days = splitArguments.Int("days", true);
			string outputFileName = splitArguments.OutputFileName(SwishFunctions.DataFileExtension);

			Count(inputFileName, outputFileName, variableName, days);
			Console.Write(outputFileName);
		}

		public static void Count(string inputFileName, string outputFileName, string variableName, int days)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new Exception("Variable missing");
			}

			string intermediateInput = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			SaveTableAdapter.Save(inputFileName, intermediateInput);

			Csv table = CsvFunctions.Read(intermediateInput);

			int columnIndex = table.ColumnIndex(variableName, true);
			List<double> values = table.ColunmAsDoubles(columnIndex);
			List<int> results = new List<int>();
			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				int count = 0;
				int periodStart;
				if (recordIndex >= days)
				{
					periodStart = recordIndex - days + 1;
				} else
				{
					periodStart = 0;
				}
				for (int periodIndex = periodStart; periodIndex <= recordIndex; periodIndex++)
				{
					double value = values[periodIndex];
					if (value != 0)
					{
						count++;
					}
				}

				results.Add(count);
			}

			 if (table.ColumnExists(AdapterFunctions.WorkingVariableName))
			 {
				 table.Remove(AdapterFunctions.WorkingVariableName);
			 }

			 table.Add(AdapterFunctions.WorkingVariableName, results);
		
			string intermediateOutput= FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			CsvFunctions.Write(intermediateOutput, table);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			SaveTableAdapter.Save(intermediateOutput, outputFileName);
		}
	}
}
