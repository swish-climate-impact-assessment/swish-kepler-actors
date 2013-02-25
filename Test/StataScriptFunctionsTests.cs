using System.Collections.Generic;
using System.IO;

namespace Swish.Tests
{
	public class StataScriptFunctionsTests
	{
		public void LoadDynamicFileFormat()
		{
			string fileName = "stata.dta";
			List<string> lines = new List<string>();
			StataScriptFunctions.LoadFileCommand(lines, fileName);
			string line = lines[1];
			if (!line.StartsWith("use"))
			{
				throw new TestException();
			}

			fileName = "text.csv";
			lines = new List<string>();
			StataScriptFunctions.LoadFileCommand(lines, fileName);
			line = lines[1];

			if (!line.StartsWith("insheet"))
			{
				throw new TestException();
			}
		}

		public void SaveDynamicFileFormat()
		{
			string fileName = "stata.dta";
			List<string> lines = new List<string>();
			StataScriptFunctions.SaveFileCommand(lines, fileName);

			if (!lines[0].StartsWith("save"))
			{
				throw new TestException();
			}

			fileName = "text.csv";
			lines = new List<string>();
			StataScriptFunctions.SaveFileCommand(lines, fileName);

			if (!lines[0].StartsWith("outsheet"))
			{
				throw new TestException();
			}
		}

		public void StataFileFormat()
		{
			/// This test verifies that scripts can be written using stata files as input
			/// 

			Csv expectedTable = StataFunctionsTests.CarData();

			List<string> lines = new List<string>();
			StataScriptFunctions.LoadFileCommand(lines, StataFunctionsTests.CarsDataFileName);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				// Stata does not overwrite files
				File.Delete(outputFileName);
			}

			StataScriptFunctions.SaveFileCommand(lines, outputFileName);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new TestException("Output file was not created" + log);
			}

			Csv table = CsvFunctions.Read(outputFileName);
			if (!CsvFunctions.Equal(table, expectedTable))
			{
				throw new TestException();
			}
		}

		private static void GenerateMergeDoFile(string doFileName)
		{
			List<string> variableNames = new List<string>();
			variableNames.Add("head4");
			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, @"C:\Swish\SampleData\MergeTable1.csv");

			string line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);
			StataScriptFunctions.SaveFileCommand(lines, @"C:\Swish\SampleData\MergeTableTemp.dta");

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, @"C:\Swish\SampleData\MergeTable2.csv");

			line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);
			lines.Add("merge 1:1 " + StataScriptFunctions.VariableList(variableNames) + " using \"" + @"C:\Swish\SampleData\MergeTableTemp.dta" + "\"");
			lines.Add("drop " + StataScriptFunctions.MergeColumnName);
			StataScriptFunctions.SaveFileCommand(lines, @"C:\Swish\SampleData\MergeTableOut.csv");

			StataScriptFunctions.WriteFooter(lines);

			File.WriteAllLines(doFileName, lines.ToArray());



		}

	}
}
