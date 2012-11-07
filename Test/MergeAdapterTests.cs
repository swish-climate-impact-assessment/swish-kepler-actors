using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Swish.Tests
{
	class MergeAdapterTests
	{
		internal void MergeSorted()
		{
			/// this verifies that both tables used in a merge operation are sorted first
			/// evently this is important
			/// 
			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateMergeInputFiles(out  inputFileName1, out  inputFileName2);

			string outputFileName = Path.GetTempFileName() + ".csv";
			if (File.Exists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			StataFunctions.Merge(inputFileName1, inputFileName2, variables, outputFileName);

			Csv table = CsvFunctions.Read(outputFileName);
			string name = StataFunctionsTests.MergeVariable;
			int head4Index = table.ColumnIndex(name);
			if (head4Index == -1)
			{
				throw new Exception("column not found");
			}

			List<int> values = table.ColunmAsInts(head4Index);
			for (int recordIndex = 0; recordIndex + 1 < table.Records.Count; recordIndex++)
			{
				int value0 = values[recordIndex];
				int value1 = values[recordIndex + 1];

				if (value1 <= value0)
				{
					throw new Exception("records not sorted");
				}
			}

		}

		internal void RemoveMergeColoumn()
		{
			/// the this test verifies that the merge column is removed
			/// preforms a merge operation and then looks for the merge column
			/// leaving the merge column will just clutter things and may cause conflicts 
			/// if operating on two tables that where the result of previous merges

			string inputFileName1;
			string inputFileName2;
			StataFunctionsTests.GenerateMergeInputFiles(out  inputFileName1, out  inputFileName2);

			string outputFileName = Path.GetTempFileName() + ".csv";
			if (File.Exists(outputFileName))
			{
				File.Delete(outputFileName);
			}
			List<string> variables = new List<string>();
			variables.Add(StataFunctionsTests.MergeVariable);
			StataFunctions.Merge(inputFileName1, inputFileName2, variables, outputFileName);

			Csv table = CsvFunctions.Read(outputFileName);

			string name =StataFunctions.MergeColumnName;
			int head4Index = table.ColumnIndex(name);
			if (head4Index != -1)
			{
				throw new Exception("merge column not removed");
			}
		}
	}
}
