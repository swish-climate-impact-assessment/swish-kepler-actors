using System.Collections.Generic;
using System.IO;
using Swish.IO;
using Swish.Stata;

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

			Table expectedTable = GenerateTestData.CarData();

			List<string> lines = new List<string>();
			StataScriptFunctions.LoadFileCommand(lines, GenerateTestData.CarsDataFileName);

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

			Table table = CsvFunctions.Read(outputFileName, true);
			if (!EqualFunctions.Equal(table, expectedTable))
			{
				throw new TestException();
			}
		}

	}
}
