using System;
using System.Collections.Generic;
using Swish.Adapters;
using System.IO;

namespace Swish.Tests
{
	class GenerateDateRangeOperationTests
	{
		internal void GenerateDateRange()
		{
			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			string variableName = "variable";
			DateTime startDate = new DateTime(2000, 1, 1);
			DateTime endDate = new DateTime(2000, 1, 31);

			Arguments arguments = new Arguments();
			arguments.String("variableName", variableName);
			arguments.Date("startDate", startDate);
			arguments.Date("endDate", endDate);

			AdapterArguments arguments2 = new AdapterArguments(arguments);
			arguments2.SetOutputFileName(outputFileName);

			if (File.Exists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			DateRangeAdapter adapter = new DateRangeAdapter();
			adapter.Run(arguments2);

			if (!File.Exists(outputFileName))
			{
				throw new Exception();
			}

			string[] lines = File.ReadAllLines(outputFileName);
			if (lines.Length != 32
				|| lines[0] != variableName
				|| lines[1] != "1/01/2000"
				|| lines[2] != "2/01/2000"
				|| lines[3] != "3/01/2000"
				|| lines[4] != "4/01/2000"
				|| lines[5] != "5/01/2000"
				|| lines[6] != "6/01/2000"
				|| lines[7] != "7/01/2000"
				|| lines[8] != "8/01/2000"
				|| lines[9] != "9/01/2000"
				|| lines[10] != "10/01/2000"
				|| lines[11] != "11/01/2000"
				|| lines[12] != "12/01/2000"
				|| lines[13] != "13/01/2000"
				|| lines[14] != "14/01/2000"
				|| lines[15] != "15/01/2000"
				|| lines[16] != "16/01/2000"
				|| lines[17] != "17/01/2000"
				|| lines[18] != "18/01/2000"
				|| lines[19] != "19/01/2000"
				|| lines[20] != "20/01/2000"
				|| lines[21] != "21/01/2000"
				|| lines[22] != "22/01/2000"
				|| lines[23] != "23/01/2000"
				|| lines[24] != "24/01/2000"
				|| lines[25] != "25/01/2000"
				|| lines[26] != "26/01/2000"
				|| lines[27] != "27/01/2000"
				|| lines[28] != "28/01/2000"
				|| lines[29] != "29/01/2000"
				|| lines[30] != "30/01/2000"
				|| lines[31] != "31/01/2000"
				)
			{
				throw new Exception();
			}

		}
	}
}
