using System.IO;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

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
				throw new Exception("");
			}

			fileName = "text.csv";
			lines = new List<string>();
			StataScriptFunctions.LoadFileCommand(lines, fileName);
			line = lines[1];

			if (!line.StartsWith("insheet"))
			{
				throw new Exception("");
			}
		}

		public void SaveDynamicFileFormat()
		{
			string fileName = "stata.dta";
			string line = StataScriptFunctions.SaveFileCommand(fileName);

			if (!line.StartsWith("save"))
			{
				throw new Exception("");
			}

			fileName = "text.csv";
			line = StataScriptFunctions.SaveFileCommand(fileName);

			if (!line.StartsWith("outsheet"))
			{
				throw new Exception("");
			}
		}

		public void StataFileFormat()
		{
			/// This test verifies that scripts can be written using stata files as input
			/// 

			Csv expectedTable = StataFunctionsTests.CarData();

			List<string> lines = new List<string>();
			StataScriptFunctions.LoadFileCommand(lines, StataFunctionsTests.CarsDataFileName);

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				// Stata does not overwrite files
				File.Delete(outputFileName);
			}

			string line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}

			Csv table = CsvFunctions.Read(outputFileName);
			if (!CsvFunctions.Equal(table, expectedTable))
			{
				throw new Exception("");
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
			line = StataScriptFunctions.SaveFileCommand(@"C:\Swish\SampleData\MergeTableTemp.dta");
			lines.Add(line);

			lines.Add("clear");
			StataScriptFunctions.LoadFileCommand(lines, @"C:\Swish\SampleData\MergeTable2.csv");

			line = StataScriptFunctions.SortCommand(variableNames);
			lines.Add(line);
			lines.Add("merge 1:1 " + StataScriptFunctions.VariableList(variableNames) + " using \"" + @"C:\Swish\SampleData\MergeTableTemp.dta" + "\"");
			lines.Add("drop " + StataScriptFunctions.MergeColumnName);
			line = StataScriptFunctions.SaveFileCommand(@"C:\Swish\SampleData\MergeTableOut.csv");
			lines.Add(line);

			StataScriptFunctions.WriteFooter(lines);

			File.WriteAllLines(doFileName, lines.ToArray());



		}

	}
}
