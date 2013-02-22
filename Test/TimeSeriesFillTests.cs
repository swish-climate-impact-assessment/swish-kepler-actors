using System;
using System.Collections.Generic;
using Swish.Adapters;

namespace Swish.Tests
{
	class TimeSeriesFillTests
	{
		const string ValueVariableName = "value";
		const string CategoryAName = "factorA";
		const string CategoryBName = "factorB";

		public void Fill()
		{
			/// So this thing takes some data with a value column and one or more factor labels
			/// 
			/// each label is a categorical value and as a set of possible values
			/// the function combines all permutations of factors and fills in missing values
			/// 

			const string FillValue = "-1";

			List<string> categoryAValues = new List<string>(new string[] { "A", "B", "C", "D" });
			List<string> categoryBValues = new List<string>(new string[] { "1", "2", "3", });

			Csv table = new Csv();
			table.Header.Add(CategoryAName);
			table.Header.Add(CategoryBName);
			table.Header.Add(ValueVariableName);

			table.Records.Add(new List<string>(new string[] { categoryAValues[0], categoryBValues[0], "1", }));
			table.Records.Add(new List<string>(new string[] { categoryAValues[1], categoryBValues[1], "2", }));
			table.Records.Add(new List<string>(new string[] { categoryAValues[2], categoryBValues[2], "3", }));
			table.Records.Add(new List<string>(new string[] { categoryAValues[3], categoryBValues[0], "4", }));

			string inputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			CsvFunctions.Write(inputFileName, table);

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");

			List<Tuple<string, List<string>>> categories = new List<Tuple<string, List<string>>>();

			categories.Add(new Tuple<string, List<string>>(CategoryAName, categoryAValues));
			categories.Add(new Tuple<string, List<string>>(CategoryBName, categoryBValues));

			FillCategoryTimeSeriesAdapter.Fill(inputFileName, outputFileName, categories, ValueVariableName, FillValue);

			Csv newTable = CsvFunctions.Read(outputFileName);
			if (false
				|| newTable.Header.Count != 3
				|| !newTable.Header.Contains(ValueVariableName)
				|| !newTable.Header.Contains(CategoryAName)
				|| !newTable.Header.Contains(CategoryBName)
				|| newTable.Records.Count != categoryAValues.Count * categoryBValues.Count
				)
			{
				throw new Exception();
			}

			for (int categoryAIndex = 0; categoryAIndex < categoryAValues.Count; categoryAIndex++)
			{
				string valueA = categoryAValues[categoryAIndex];
				for (int categoryBIndex = 0; categoryBIndex < categoryBValues.Count; categoryBIndex++)
				{
					string valueB = categoryBValues[categoryBIndex];

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
			int aIndex = table.ColumnIndex(CategoryAName);
			if (aIndex <= 0)
			{
				throw new Exception();
			}

			int bIndex = table.ColumnIndex(CategoryBName);
			if (bIndex <= 0)
			{
				throw new Exception();
			}

			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				string recordValueA = record[aIndex];
				string recordValueB = record[bIndex];

				if (recordValueA == valueA && recordValueB == valueB)
				{
					int valueIndex = table.ColumnIndex(ValueVariableName);
					if (valueIndex <= 0)
					{
						throw new Exception();
					}

					value = record[valueIndex];
					return true;
				}
			}

			value = null;
			return false;
		}
	}
}
