using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Tests
{
	class TransposeTests
	{
		internal void TransposeTable()
		{
			/// the this test verifies that the transpose occurs correctly
			/// actor simply read input change rows and columns and save 

			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			Csv table = CsvFunctions.Read(inputFileName);

			string outputFileName = Path.GetTempFileName() + ".csv";
			if (File.Exists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			StataFunctions.Transpose(inputFileName, outputFileName);

			Csv result = CsvFunctions.Read(outputFileName);

			for (int x = 0; x < table.Header.Count - 1; x++)
			{
				for (int y = 0; y < table.Records.Count; y++)
				{
					if (result.Records[x][y] != table.Records[y][x])
					{
						throw new Exception();
					}
				}
			}
		}
	}
}
