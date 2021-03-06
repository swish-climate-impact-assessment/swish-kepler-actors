﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Swish.Tests
{
	public class AdapterTests
	{
		public void SelectExpression()
		{
			string expression = "head4>=10";
			// 17 records >= 10
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			Csv table = CsvFunctions.Read(inputFileName);

			int head4Index = table.ColumnIndex("head4");
			for (int recordIndex = 0; recordIndex < table.Records.Count; recordIndex++)
			{
				List<string> record = table.Records[recordIndex];
				if (!(int.Parse(record[head4Index]) >= 10))
				{
					table.Records.RemoveAt(recordIndex);
					recordIndex--;
				}
			}

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			AdapterFunctions.Select(inputFileName, outputFileName, expression);

			Csv result = CsvFunctions.Read(outputFileName);

			if (!CsvFunctions.Equal(table, result))
			{
				throw new TestException();
			}
		}

		public void SelectColumns()
		{
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> variables = new List<string>();
			variables.Add("head4");
			variables.Add("head6");

			AdapterFunctions.SelectColumns(inputFileName, outputFileName, variables);

			Csv result = CsvFunctions.Read(outputFileName);
			if (result.Header.Count != 2 || !result.Header.Contains("head4") || !result.Header.Contains("head6"))
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

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");

			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			AdapterFunctions.Merge(inputFileName1, inputFileName2, variables, outputFileName, false, null);

			Csv table = CsvFunctions.Read(outputFileName);
			string name = StataFunctionsTests.MergeVariable;
			int head4Index = table.ColumnIndex(name);
			if (head4Index == -1)
			{
				throw new TestException("column not found");
			}

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

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			AdapterFunctions.Merge(inputFileName1, inputFileName2, variables, outputFileName, false, null);

			Csv table = CsvFunctions.Read(outputFileName);

			string name = StataScriptFunctions.MergeColumnName;
			int head4Index = table.ColumnIndex(name);
			if (head4Index != -1)
			{
				throw new TestException("merge column not removed");
			}
		}

		public void TransposeTable()
		{
			/// the this test verifies that the transpose occurs correctly
			/// actor simply read input change rows and columns and save 

			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();
			Csv table = CsvFunctions.Read(inputFileName);

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			AdapterFunctions.Transpose(inputFileName, outputFileName);

			Csv result = CsvFunctions.Read(outputFileName);

			for (int x = 0; x < table.Header.Count - 1; x++)
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
			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			string command = "xpose, clear";

			AdapterFunctions.StataCommand(inputFileName, outputFileName, command);
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

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			AdapterFunctions.Append(inputFileName1, inputFileName2, outputFileName);

			Csv table = CsvFunctions.Read(outputFileName);

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

			double value = AdapterFunctions.Collapse(inputFile, "head4", CollapseOpperation.Mean);
			if (double.IsNaN(value))
			{
				throw new TestException();
			}
		}

		public void Sort()
		{
			string inputFileName = StataFunctionsTests.GenerateMeanInputFile();

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			List<string> variables = new List<string>();
			variables.Add("head4");
			AdapterFunctions.Sort(inputFileName, variables, outputFileName);
		}

		internal void Replace_1()
		{
			string inputFileName = StataFunctionsTests.GenerateReplaceInputFile();

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			string condition = "head2==4";
			string value = "head2=1";
			AdapterFunctions.Replace(inputFileName, outputFileName, condition, value);

			Csv table = CsvFunctions.Read(outputFileName);

			int head2Index = table.ColumnIndex("head2");
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

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			string condition = "head2==4";
			string value = "head2=1";

			Arguments splitArguments = new  Arguments();
			splitArguments.SplitArguments.Add(new Tuple<string, string>(Arguments.InputArgument, inputFileName));
			splitArguments.SplitArguments.Add(new Tuple<string, string>(Arguments.DefaultArgumentPrefix + "output", outputFileName));
			splitArguments.SplitArguments.Add(new Tuple<string, string>(Arguments.DefaultArgumentPrefix + "condition", condition));
			splitArguments.SplitArguments.Add(new Tuple<string, string>(Arguments.DefaultArgumentPrefix + "value", value));

			AdapterFunctions.RunOperation(AdapterFunctions.ReplaceOperation, splitArguments);

			if (!File.Exists(outputFileName))
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

			string intermediateFileName1 = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(intermediateFileName1))
			{
				File.Delete(intermediateFileName1);
			}

			string intermediateFileName2 = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(intermediateFileName2))
			{
				File.Delete(intermediateFileName2);
			}

			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			if (FileFunctions.FileExists(outputFileName))
			{
				File.Delete(outputFileName);
			}

			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			AdapterFunctions.Merge(inputFileName1, inputFileName2, variables, intermediateFileName1, true, null);

			//"matched (3)"
			//"master only (1)"
			//"using only (2)"

			AdapterFunctions.Replace(intermediateFileName1, intermediateFileName2, "_merge==\"using only (2)\"", "head2=0");

			AdapterFunctions.Replace(intermediateFileName2, outputFileName, "_merge==\"master only (1)\"", "head6=\"0\"");

			Csv table = CsvFunctions.Read(outputFileName);

			if (table.Records.Count != 26)
			{
				throw new TestException();
			}

			string name = StataFunctionsTests.MergeVariable;
			int head4Index = table.ColumnIndex(name);
			int head2Index = table.ColumnIndex("head2");
			int head6Index = table.ColumnIndex("head6");
			if (head4Index == -1 || head2Index == -1 || head6Index == -1)
			{
				throw new TestException("column not found");
			}

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
			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			string variableName = "testVariable";
			AdapterFunctions.Generate(inputFileName, outputFileName, variableName, string.Empty, "-head4");
			if (!FileFunctions.FileExists(outputFileName))
			{
				throw new TestException();
			}
			Csv table = CsvFunctions.Read(outputFileName);

			int index = table.ColumnIndex(variableName);
			if (index < 0)
			{
				throw new TestException();
			}

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

		internal void MergeKeep()
		{
			string input1FileName;
			string input2FileName;
			StataFunctionsTests.GenerateMergeInputFiles(out input1FileName, out input2FileName, true);

			List<string> variableNames = new List<string>();
			variableNames.Add(StataFunctionsTests.MergeVariable);
			string outputFileName = FileFunctions.TempoaryOutputFileName(".csv");
			List<MergeRecordResult> keep = new List<MergeRecordResult>();
			keep.Add(MergeRecordResult.Match);

			AdapterFunctions.Merge(input1FileName, input2FileName, variableNames, outputFileName, false, keep);

			Csv table = CsvFunctions.Read(outputFileName);

			int columnIndex = table.ColumnIndex(StataFunctionsTests.MergeVariable);
			List<int> values = table.ColunmAsInts(columnIndex);


			if (table.Records.Count != 9
				|| !values.Contains(9)
				|| !values.Contains(15)
				|| !values.Contains(21)
				|| !values.Contains(3)
				|| !values.Contains(18)
				|| !values.Contains(24)
				|| !values.Contains(12)
				|| !values.Contains(0)
				|| !values.Contains(6)
				)
			{
				throw new TestException();
			}

		}
	}
}
