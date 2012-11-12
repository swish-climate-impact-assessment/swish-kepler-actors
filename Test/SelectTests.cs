using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Tests
{
	class SelectTests
	{
		internal void SelectExpression()
		{
			string expression = "head4>=10";
			// 17 records >= 10
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			Csv table = CsvFunctions.Read(inputFileName);

			int head4Index = table.ColumnIndex("head4");
			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				if (!(int.Parse(record[head4Index]) >= 10))
				{
					table.Records.RemoveAt(recordIndex);
					recordIndex--;
				}
			}

			string outputFileName = Path.GetTempFileName() + ".csv";
			if (File.Exists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			StataFunctions.Select(inputFileName, outputFileName, expression);

			Csv result = CsvFunctions.Read(outputFileName);

			if (!CsvFunctions.Equal(table, result))
			{
				throw new Exception();
			}
		}

		internal void SelectColumns()
		{
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();

			string outputFileName = Path.GetTempFileName() + ".csv";
			if (File.Exists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> variables = new List<string>();
			variables.Add("head4");
			variables.Add("head6");

			StataFunctions.SelectColumns(inputFileName, outputFileName, variables);

			Csv result = CsvFunctions.Read(outputFileName);
			if (result.Header.Count != 2 || !result.Header.Contains("head4") || !result.Header.Contains("head6"))
			{
				throw new Exception();
			}

			File.Delete(outputFileName);

			variables = new List<string>();

			StataFunctions.SelectColumns(inputFileName, outputFileName, variables);

			result = CsvFunctions.Read(outputFileName);
			if (result.Header.Count != 0)
			{
				throw new Exception();
			}
		}

	}
}
