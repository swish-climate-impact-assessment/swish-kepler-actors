using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Adapters
{
	public class FillCategoryTimeSeriesAdapter: IAdapter
	{
		public string Name { get { return "FillCategoryTimeSeries"; } }

		public string Run(AdapterArguments splitArguments)
		{
			throw new Exception();
		}

		public static void Fill(string inputFileName, string outputFileName, string dateVariableName, List<Tuple<string/* category variable name */, List<string>/* values */>> categories, string expression)
		{
			if (!FileFunctions.FileExists(inputFileName))
			{
				throw new Exception("cannot find file \"" + inputFileName + "\"");
			}

			if (string.IsNullOrWhiteSpace(dateVariableName))
			{
				throw new Exception("dateVariableName missing");
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new Exception("Expression missing");
			}

			if (categories == null || categories.Count == 0)
			{
				throw new Exception("Categories missing");
			}

			for (int categoryIndex = 0; categoryIndex < categories.Count; categoryIndex++)
			{
				string categoryVariableName = categories[categoryIndex].Item1;
				List<string> categoryValues = categories[categoryIndex].Item2;

				if (string.IsNullOrWhiteSpace(categoryVariableName))
				{
					throw new Exception("Unnamed category found");
				}
				if (categoryValues.Count == 0)
				{
					throw new Exception("Category \"" + categoryVariableName + "\" missing values");
				}

				for (int valueIndex = 0; valueIndex < categoryValues.Count; valueIndex++)
				{
					string value = categoryValues[valueIndex];
					if (string.IsNullOrWhiteSpace(value))
					{
						throw new Exception("Category \"" + categoryVariableName + "\" value missing");
					}
				}
			}
			string extension = Path.GetExtension(outputFileName);
			string intermaidateOutput = FileFunctions.TempoaryOutputFileName(extension);

			List<string> lines = new List<string>();
			StataScriptFunctions.WriteHeadder(lines);

			StataScriptFunctions.LoadFileCommand(lines, inputFileName);

			if (DateTime.MinValue < DateTime.MaxValue)
			{
				throw new Exception(" do stuff here ...");
			}

			StataScriptFunctions.SaveFileCommand(lines, intermaidateOutput);

			StataScriptFunctions.WriteFooter(lines);

			string log = StataFunctions.RunScript(lines, false);
			if (!FileFunctions.FileExists(intermaidateOutput))
			{
				throw new Exception("Output file was not created" + Environment.NewLine + log + Environment.NewLine + "Script lines: " + Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
			}

			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			File.Move(intermaidateOutput, outputFileName);
		}

	}
}
