using System;
using System.Collections.Generic;
using Swish.Adapters;

namespace Swish.Tests
{
	class TimeSeriesFillTests
	{
		const string DateVariableName = "Date";
		const string CategoryAName = "factorA";
		const string CategoryBName = "factorB";
		const string ValueVariableName = "value";

		public static List<string> CategoryAValues = new List<string>(new string[] { "A", "B", "C", "D" });
		public static List<string> CategoryBValues = new List<string>(new string[] { "1", "2", "3", });

		public void Fill()
		{
			/// So this thing takes some data with a value column and one or more factor labels
			/// 
			/// each label is a categorical value and as a set of possible values
			/// the function combines all permutations of factors and fills in missing values
			/// 

			const string FillValue = "-1";

			Csv table = new Csv();
			table.Header.Add(DateVariableName);
			table.Header.Add(CategoryAName);
			table.Header.Add(CategoryBName);
			table.Header.Add(ValueVariableName);

			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 1).ToShortDateString(), CategoryAValues[0], CategoryBValues[0], "1", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 2).ToShortDateString(), CategoryAValues[1], CategoryBValues[1], "2", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 3).ToShortDateString(), CategoryAValues[2], CategoryBValues[2], "3", }));
			table.Records.Add(new List<string>(new string[] { new DateTime(2000, 1, 4).ToShortDateString(), CategoryAValues[3], CategoryBValues[0], "4", }));

			string inputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			CsvFunctions.Write(inputFileName, table);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);

			List<Tuple<string, List<string>>> categories = new List<Tuple<string, List<string>>>();

			categories.Add(new Tuple<string, List<string>>(CategoryAName, CategoryAValues));
			categories.Add(new Tuple<string, List<string>>(CategoryBName, CategoryBValues));

			FillCategoryTimeSeriesAdapter.Fill(inputFileName, outputFileName, DateVariableName, categories, FillValue);

			Csv newTable = CsvFunctions.Read(outputFileName);
			if (false
				|| newTable.Header.Count != 3
				|| !newTable.Header.Contains(ValueVariableName)
				|| !newTable.Header.Contains(CategoryAName)
				|| !newTable.Header.Contains(CategoryBName)
				|| newTable.Records.Count != CategoryAValues.Count * CategoryBValues.Count
				)
			{
				throw new Exception();
			}

			for (int categoryAIndex = 0; categoryAIndex < CategoryAValues.Count; categoryAIndex++)
			{
				string valueA = CategoryAValues[categoryAIndex];
				for (int categoryBIndex = 0; categoryBIndex < CategoryBValues.Count; categoryBIndex++)
				{
					string valueB = CategoryBValues[categoryBIndex];

					string newValue;
					if (!TryGetValue(newTable, valueA, valueB, out newValue))
					{
						throw new Exception();
					}

					string value;
					if (!TryGetValue(table, valueA, valueB, out value))
					{
						if (newValue != FillValue)
						{
							throw new Exception();
						}
					} else
					{
						if (newValue != value)
						{
							throw new Exception();
						}
					}
				}
			}
		}

		private bool TryGetValue(Csv table, string valueA, string valueB, out string value)
		{
			int aIndex = table.ColumnIndex(CategoryAName, true);

			int bIndex = table.ColumnIndex(CategoryBName, true);

			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				string recordValueA = record[aIndex];
				string recordValueB = record[bIndex];

				if (recordValueA == valueA && recordValueB == valueB)
				{
					int valueIndex = table.ColumnIndex(ValueVariableName, true);

					value = record[valueIndex];
					return true;
				}
			}

			value = null;
			return false;
		}
	}
}


