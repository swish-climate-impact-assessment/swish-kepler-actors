using System;
using System.Collections.Generic;
using System.IO;
using Swish.Adapters;
using Swish.IO;
using Swish.ScriptGenerators;
using Swish.Stata;

namespace Swish.Tests
{
	public class AdapterTests
	{
		public void SelectExpression()
		{
			string expression = "head4>=10";
			// 17 records >= 10
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			Table table = CsvFunctions.Read(inputFileName);

			int head4Index = table.ColumnIndex("head4", true);
			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				if (!(int.Parse(record[head4Index]) >= 10))
				{
					table.Records.RemoveAt(recordIndex);
					recordIndex--;
				}
			}

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			TableFunctions.SelectRecords(inputFileName, outputFileName, expression);

			Table result = CsvFunctions.Read(outputFileName);

			if (!CsvFunctions.Equal(table, result))
			{
				throw new TestException();
			}
		}

		public void SelectColumns()
		{
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> variables = new List<string>();
			variables.Add("head4");
			variables.Add("head6");

			TableFunctions.SelectVariables(inputFileName, outputFileName, variables);

			Table result = CsvFunctions.Read(outputFileName);
			if (result.Headers.Count != 2 || !result.Headers.Contains("head4") || !result.Headers.Contains("head6"))
			{
				throw new TestException();
			}

			File.Delete(outputFileName);
		}

		public void MergeSorted()
		{
			/// this verifies that both tables used in a merge operation are sorted first
			/// evently this is important
			/// 
			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateMergeInputFiles(out  inputFileName1, out  inputFileName2, false);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);

			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			TableFunctions.Merge(inputFileName1, inputFileName2, variables, outputFileName, false);

			Table table = CsvFunctions.Read(outputFileName);
			string name = StataFunctionsTests.MergeVariable;
			int head4Index = table.ColumnIndex(name, true);

			List<int> values = table.ColunmAsInts(head4Index);
			for (int recordIndex = 0; recordIndex + 1 < table.Records.Count; recordIndex++)
			{
				int value0 = values[recordIndex];
				int value1 = values[recordIndex + 1];

				if (value1 <= value0)
				{
					throw new TestException("records not sorted");
				}
			}

		}

		public void RemoveMergeColoumn()
		{
			/// the this test verifies that the merge column is removed
			/// preforms a merge operation and then looks for the merge column
			/// leaving the merge column will just clutter things and may cause conflicts 
			/// if operating on two tables that where the result of previous merges

			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateMergeInputFiles(out  inputFileName1, out  inputFileName2, false);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			TableFunctions.Merge(inputFileName1, inputFileName2, variables, outputFileName, false);

			Table table = CsvFunctions.Read(outputFileName);

			string name = StataScriptFunctions.MergeColumnName;
			int head4Index = table.ColumnIndex(name, false);
			if (head4Index >= 0)
			{
				throw new Exception("");
			}
		}

		public void TransposeTable()
		{
			/// the this test verifies that the transpose occurs correctly
			/// actor simply read input change rows and columns and save 

			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			Table table = CsvFunctions.Read(inputFileName);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			TableFunctions.Transpose(inputFileName, outputFileName);

			Table result = CsvFunctions.Read(outputFileName);

			for (int x = 0; x < table.Headers.Count - 1; x++)
			{
				for (int y = 0; y < table.Records.Count; y++)
				{
					if (result.Records[x][y] != table.Records[y][x])
					{
						throw new TestException();
					}
				}
			}
		}

		public void CommandScript()
		{
			// this is a test that the outputted script contains the command

			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string command = "xpose, clear";

			StataCommandAdapter.StataCommand(inputFileName, outputFileName, command);
		}

		private bool LinesContain(string command, List<string> lines)
		{
			for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
			{
				string line = lines[lineIndex];
				if (line.Contains(command))
				{
					return true;
				}
			}
			return false;
		}

		public void Append()
		{
			// this tests that appending two tables results in a new table where 
			// new record count = sum ( input record counts )

			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateAppendInputFile(out inputFileName1, out inputFileName2);

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			TableFunctions.Append(inputFileName1, inputFileName2, outputFileName);

			Table table = CsvFunctions.Read(outputFileName);

			if (table.Records.Count != 14 - 1 + 15 - 1)
			{
				throw new TestException("append failed");
			}
		}

		public void Mean()
		{
			// this checks that the mean operation returns a valid 
			// number

			string inputFile = StataFunctionsTests.GenerateMeanInputFile();

			double value = CollapseAdapter.Collapse(inputFile, "head4", CollapseOpperation.Mean);
			if (double.IsNaN(value))
			{
				throw new TestException();
			}
		}

		public void Sort()
		{
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			List<string> variables = new List<string>();
			variables.Add("head4");
			TableFunctions.Sort(inputFileName, variables, outputFileName);
		}

		internal void Replace_1()
		{
			string inputFileName = StataFunctionsTests.GenerateReplaceInputFile();

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			string condition = "head2==4";
			string value = "head2=1";
			TableFunctions.Replace(inputFileName, outputFileName, condition, value);

			Table table = CsvFunctions.Read(outputFileName);

			int head2Index = table.ColumnIndex("head2", true);
			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				int result = int.Parse(record[head2Index]);
				if (result != 1)
				{
					throw new TestException();
				}
			}
		}

		internal void Replace_2()
		{
			string inputFileName = StataFunctionsTests.GenerateReplaceInputFile();

			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			string condition = "head2==4";
			string value = "head2=1";

			Arguments splitArguments = new Arguments();
			splitArguments.String(StataScriptFunctions.InputFileNameToken, inputFileName);
			splitArguments.String(StataScriptFunctions.OutputFileNameToken, outputFileName);
			splitArguments.String(ReplaceScriptGenerator.ConditionToken, condition);
			splitArguments.String(ReplaceScriptGenerator.ValueToken, value);

			OperationFunctions.RunOperation(ReplaceScriptGenerator.NameString, new OperationArguments(splitArguments));

			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new TestException();
			}
		}

		internal void MergeZero()
		{
			/// this verifies that merge occurs, missing values for missing records are zeroed
			/// used in final.do
			/// 
			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateMergeInputFiles(out  inputFileName1, out  inputFileName2, true);

			string intermediateFileName1 = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			string intermediateFileName2 = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);

			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			TableFunctions.Merge(inputFileName1, inputFileName2, variables, intermediateFileName1, true);

			//"matched (3)"
			//"master only (1)"
			//"using only (2)"

			//ReplaceAdapter.Replace(intermediateFileName1, intermediateFileName2, "_merge==\"using only (2)\"", "head2=0");
			TableFunctions.Replace(intermediateFileName1, intermediateFileName2, "_merge==2", "head2=0");

			//ReplaceAdapter.Replace(intermediateFileName2, outputFileName, "_merge==\"master only (1)\"", "head6=\"0\"");
			TableFunctions.Replace(intermediateFileName2, outputFileName, "_merge==1", "head6=\"0\"");

			Table table = CsvFunctions.Read(outputFileName);

			if (table.Records.Count != 26)
			{
				throw new TestException();
			}

			string name = StataFunctionsTests.MergeVariable;
			int head4Index = table.ColumnIndex(name, true);
			int head2Index = table.ColumnIndex("head2", true);
			int head6Index = table.ColumnIndex("head6", true);

			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];

				int value4 = int.Parse(record[head4Index]);
				string value = record[head2Index];
				double value2;
				if (!string.IsNullOrWhiteSpace(value))
				{
					value2 = double.Parse(value);
				} else
				{
					value2 = -1;
				}
				char value6;
				value = record[head6Index];
				if (value.StartsWith("\""))
				{
					value = value.Trim('\"');
				}
				if (value.Length > 0)
				{
					value6 = value[0];
				} else
				{
					value6 = (char)(char.MaxValue);
				}

				if (value4 == 0)
				{
					continue;
				}
				if ((value4 % 3) == 0)
				{
					if (value2 == 0 || value6 == '0')
					{
						throw new TestException();
					}
				} else if ((value4 % 2) == 0)
				{
					if (value2 != 0 || value6 == '0')
					{
						throw new TestException();
					}
				} else //  if ((value4 % 2 ) == 1)
				{
					if (value2 == 0 || value6 != '0')
					{
						throw new TestException();
					}
				}
			}

		}

		internal void Generate()
		{
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			string outputFileName = FileFunctions.TempoaryOutputFileName(SwishFunctions.CsvFileExtension);
			string variableName = "testVariable";

			TableFunctions.Generate(inputFileName, outputFileName, variableName, StataDataType.Unknown, "-head4");
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new TestException();
			}
			Table table = CsvFunctions.Read(outputFileName);

			int index = table.ColumnIndex(variableName, true);
			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];

				int value = int.Parse(record[index]);
				if (value > 0)
				{
					throw new TestException();
				}
			}
		}

	}
}
