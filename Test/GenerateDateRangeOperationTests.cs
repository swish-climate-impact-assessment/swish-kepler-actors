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
			string variableName = "testVar";
			DateTime startDate = new DateTime(2000, 1, 1);
			DateTime endDate = new DateTime(2000, 1, 31);

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			TableFunctions.DateRange(outputFileName, variableName, startDate, endDate);

			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception();
			}

			string[] lines = File.ReadAllLines(outputFileName);
			if (lines.Length != 32
				|| lines[0] != variableName
				|| DateTime.Parse(lines[1]) != new DateTime(2000, 01, 1)
				|| DateTime.Parse(lines[2]) != new DateTime(2000, 01, 2)
				|| DateTime.Parse(lines[3]) != new DateTime(2000, 01, 3)
				|| DateTime.Parse(lines[4]) != new DateTime(2000, 01, 4)
				|| DateTime.Parse(lines[5]) != new DateTime(2000, 01, 5)
				|| DateTime.Parse(lines[6]) != new DateTime(2000, 01, 6)
				|| DateTime.Parse(lines[7]) != new DateTime(2000, 01, 7)
				|| DateTime.Parse(lines[8]) != new DateTime(2000, 01, 8)
				|| DateTime.Parse(lines[9]) != new DateTime(2000, 01, 9)
				|| DateTime.Parse(lines[10]) != new DateTime(2000, 01, 10)
				|| DateTime.Parse(lines[11]) != new DateTime(2000, 01, 11)
				|| DateTime.Parse(lines[12]) != new DateTime(2000, 01, 12)
				|| DateTime.Parse(lines[13]) != new DateTime(2000, 01, 13)
				|| DateTime.Parse(lines[14]) != new DateTime(2000, 01, 14)
				|| DateTime.Parse(lines[15]) != new DateTime(2000, 01, 15)
				|| DateTime.Parse(lines[16]) != new DateTime(2000, 01, 16)
				|| DateTime.Parse(lines[17]) != new DateTime(2000, 01, 17)
				|| DateTime.Parse(lines[18]) != new DateTime(2000, 01, 18)
				|| DateTime.Parse(lines[19]) != new DateTime(2000, 01, 19)
				|| DateTime.Parse(lines[20]) != new DateTime(2000, 01, 20)
				|| DateTime.Parse(lines[21]) != new DateTime(2000, 01, 21)
				|| DateTime.Parse(lines[22]) != new DateTime(2000, 01, 22)
				|| DateTime.Parse(lines[23]) != new DateTime(2000, 01, 23)
				|| DateTime.Parse(lines[24]) != new DateTime(2000, 01, 24)
				|| DateTime.Parse(lines[25]) != new DateTime(2000, 01, 25)
				|| DateTime.Parse(lines[26]) != new DateTime(2000, 01, 26)
				|| DateTime.Parse(lines[27]) != new DateTime(2000, 01, 27)
				|| DateTime.Parse(lines[28]) != new DateTime(2000, 01, 28)
				|| DateTime.Parse(lines[29]) != new DateTime(2000, 01, 29)
				|| DateTime.Parse(lines[30]) != new DateTime(2000, 01, 30)
				|| DateTime.Parse(lines[31]) != new DateTime(2000, 01, 31)
				)
			{
				throw new Exception();
			}
		}
	}
}
