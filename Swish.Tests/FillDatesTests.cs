using System;
using System.Collections.Generic;
using Swish.IO;

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

			string dateVariableName = "date";

			string inputFileName = GenerateTestData.GenerateFillDateInputData(startDate, endDate);
			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);

			TableFunctions.FillDate(inputFileName, outputFileName, dateVariableName);

			Table output = CsvFunctions.Read(outputFileName, true);

			if (output.Headers.Count != 2)
			{
				throw new Exception();
			}
			int daysInYear = (int)((endDate - startDate).TotalDays) + 1;
			if (output.Records.Count != daysInYear)
			{
				throw new Exception();
			}
		}

	}
}
