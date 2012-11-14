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
			string line = StataScriptFunctions.LoadFileCommand(fileName);

			if (!line.StartsWith("use"))
			{
				throw new Exception("");
			}

			fileName = "text.csv";
			line = StataScriptFunctions.LoadFileCommand(fileName);

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
			string line = StataScriptFunctions.LoadFileCommand(StataFunctionsTests.CarsDataFileName);
			lines.Add(line);

			string outputFileName = Path.GetTempFileName() + ".csv";
			if (File.Exists(outputFileName))
			{
				// Stata does not overwrite files
				File.Delete(outputFileName);
			}

			line = StataScriptFunctions.SaveFileCommand(outputFileName);
			lines.Add(line);

			string log = StataFunctions.RunScript(lines, false);
			if (!File.Exists(outputFileName))
			{
				throw new Exception("Output file was not created" + log);
			}

			Csv table = CsvFunctions.Read(outputFileName);
			if (!CsvFunctions.Equal(table, expectedTable))
			{
				throw new Exception("");
			}
		}


	}
}
